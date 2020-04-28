extends Node2D

var i = 0

export var thing : NodePath
export var CollisionHolderNodePath : NodePath

var world_size : Vector2

var CollisionHolder : Node2D
var _to_cull : Array

var _image_republish_texture := ImageTexture.new()

func _ready():
	CollisionHolder = get_node(CollisionHolderNodePath)
	
	world_size = (get_parent() as Sprite).get_rect().size
	
	# Match our destruction viewport to the regular viewport.
	# If we didn't do this we'd need to either hardcode a viewport size
	$Viewport.set_size(get_viewport_rect().size)
	
	# Passing 0 to duplicate, as we don't want to duplicate scripts/signals etc
	# We don't use 8 since we're going to delete our duplicate nodes after first render anyway
	var dup = get_parent().duplicate(0) as Node2D
	_to_cull.append(dup)
	# Shrink to match the ratio of our destruction viewport
	dup.scale = _world_to_viewport_scale(dup.scale)
	# Then reposition, so we're in the right spot
	dup.position = _world_to_viewport(dup.position)
	
	# Risky move:
	# Delete all children of the duplicate.
	# One of those children will be a duplicate of the Destructible
	for child in dup.get_children():
		dup.remove_child(child)
	$Viewport.add_child(dup)
		
	rebuild_texture()
	$CullTimer.start()

	
	
func _cull_foreground_duplicates():
	for dup in _to_cull:
		dup.queue_free()
	_to_cull = Array()

func _process(_delta):
	get_node(thing).visible = true
	get_node(thing).position = Vector2(get_node(thing).position.x + i, get_node(thing).position.y)
	
	if i < 3:
		rebuild_texture()
		i += 1
	
	# Debug
	OS.set_window_title(" | fps: " + str(Engine.get_frames_per_second()))
	

func rebuild_texture():
	# Force re-render to update our target viewport
	$Viewport.render_target_update_mode = Viewport.UPDATE_ONCE
	
	# Recalculate collisions
	if $CollisionRebuildTimer.is_stopped():
		$CollisionRebuildTimer.start()

func rebuild_collisions():
	var bitmap := BitMap.new()
	bitmap.create_from_image_alpha($Sprite.texture.get_data())
	
	# DEBUG:
	#$Sprite.get_texture().get_data().save_png("res://screenshots/subtractiveblend.png")
	#print("Saved")

	# This will generate polygons for the given coordinate rectangle within the bitmap
	# As an optimisation we should *only* recalculate polygons in an area around where the impact was
	var polygons = bitmap.opaque_to_polygons(Rect2(Vector2(0,0), bitmap.get_size()))
	
	# Delete all previous polygons
	for child in CollisionHolder.get_children():
		child.queue_free()
	
	# Now create a collision polygon for each polygon returned
	# For the most part there will probably only be one.... unless you have islands
	# And we'll add them as children of our DynamicTexture
	for polygon in polygons:
		var collider := CollisionPolygon2D.new()

		# Remap our points from the dynamic texture (which is small)
		# To our world, which might be more than a screen wide.
		# Depending on the size of our dynamic texture viewport, we'll get a different resolution
		# of collidability.
		# This can be much less fine-grained than our display!
		var newpoints := Array()
		for point in polygon:
			# Remap to world position
			newpoints.push_back(_viewport_to_world(point))
		collider.polygon = newpoints # Scaling example. Otherwise just use polygon
		CollisionHolder.add_child(collider)
	
	# Here's where things get wild
	# We need to multiply the foreground sprite against our destruction sprite
	# So that the foreground only shows where our destruction sprite is not alpha
	republish_sprite(bitmap)

func republish_sprite(bitmap : BitMap) -> void:

	var material : Material = get_parent().material 
	
	# Assume the image has changed, so we'll need to update our ImageTexture
	_image_republish_texture.create_from_image($Sprite.texture.get_data())

	# If our parent has the proper src/destruction/parent_material shader
	# We can set our destruction_mask parameter against it, 
	# which will carve out our destruction map!
	if material != null:
		material.set_shader_param("destruction_mask", _image_republish_texture)

func _viewport_to_world(var point : Vector2) -> Vector2:
	var dynamic_texture_size = $Viewport.get_size()
	return Vector2(
				((point.x + get_viewport_rect().position.x) / dynamic_texture_size.x) * world_size.x,
				((point.y + get_viewport_rect().position.y) / dynamic_texture_size.y) * world_size.y
			)

func _world_to_viewport(var point : Vector2) -> Vector2:
	var dynamic_texture_size = $Viewport.get_size()
	return Vector2(
				(point.x / world_size.x) * dynamic_texture_size.x + get_viewport_rect().position.x,
				(point.y / world_size.y) * dynamic_texture_size.y + get_viewport_rect().position.y
			)

func _viewport_to_world_scale_amount():
	var dynamic_texture_size = $Viewport.get_size()
	var x_scale = ((100.0 / dynamic_texture_size.x) * world_size.x) / 100.0
	var y_scale = ((100.0 / dynamic_texture_size.y) * world_size.y) / 100.0
	return Vector2(x_scale, y_scale)

func _world_to_viewport_scale(var original_scale : Vector2
		, var dynamic_texture_size : Vector2 = Vector2.ZERO) -> Vector2:
	if dynamic_texture_size == Vector2.ZERO	:
		dynamic_texture_size = $Viewport.get_size()
	var x_scale = ((100.0 / world_size.x) * dynamic_texture_size.x) / 100.0
	var y_scale = ((100.0 / world_size.y) * dynamic_texture_size.y) / 100.0
	return Vector2(original_scale.x * x_scale, original_scale.y * y_scale)
