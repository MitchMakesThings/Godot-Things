extends Node2D

var i = 0

export var thing : NodePath
export var CollisionHolderNodePath : NodePath

var world_size : Vector2

var CollisionHolder : Node2D
var _to_cull : Array

var _image_republish_texture := ImageTexture.new()

var _parent_material : Material
var _destruction_threads := Array()

func _ready():
	add_to_group("destructibles")
	
	CollisionHolder = get_node(CollisionHolderNodePath)
	world_size = (get_parent() as Sprite).get_rect().size
	_parent_material = get_parent().material
	
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
	$CollisionRebuildTimer.start() # DEBUG

func _exit_tree():
	for thread in _destruction_threads:
		thread.wait_to_finish()

func destroy(position : Vector2, radius : float):
	var viewport_position = _world_to_viewport(position)
	
	# Collision rebuild thread!
	var thread := Thread.new()
	var error = thread.start(self, "rebuild_collisions_from_geometry", [position, radius])
	if error != OK:
		print("Error creating destruction thread: ", error)
	_destruction_threads.push_back(thread)
	
	# This stuff does the bad-idea rebuild using images
	get_node(thing).radius = radius / 2
	get_node(thing).global_position = viewport_position
	get_node(thing).update() # Redraw the circle, now that we've updated it's radius

	rebuild_texture()
	yield(VisualServer, "frame_post_draw")
	republish_sprite()


func _cull_foreground_duplicates():
	for dup in _to_cull:
		dup.queue_free()
	_to_cull = Array()


func _process(_delta):
	get_node(thing).visible = true
	get_node(thing).position = Vector2(get_node(thing).position.x + i, get_node(thing).position.y)
	
	# Debug
	OS.set_window_title(" | fps: " + str(Engine.get_frames_per_second()))
	

func rebuild_texture():
	# Force re-render to update our target viewport
	$Viewport.render_target_update_mode = Viewport.UPDATE_ONCE


# Improved collision rebuilding!
func rebuild_collisions_from_geometry(arguments : Array):
	var position : Vector2 = arguments[0]
	var radius : float = arguments[1]

	var nb_points = 8
	var points_arc = PoolVector2Array()
	points_arc.push_back(position)
	
	for i in range(nb_points + 1):
		var angle_point = deg2rad(i * 360 / nb_points)
		points_arc.push_back(position + Vector2(cos(angle_point), sin(angle_point)) * radius * 2)

	for collision_polygon in $CollisionHolder.get_children():
		var clipped_polygons = Geometry.clip_polygons_2d(collision_polygon.polygon, points_arc)
		for i in range(clipped_polygons.size()):
			var clipped_collision = clipped_polygons[i]
			
			# Ignore clipped polygons that are too small to actually create
			# These are awkward single or two-point floaters.
			# If we can't at least make a triangle from it, we don't care about it
			if clipped_collision.size() < 3:
				continue
			
			# God knows why, but creating a PoolVector2Array from the Geometry Array fails
			# ie, PoolVector2Array(Geometry.clip_polygons_2d(points_arc, collision_polygon.polygon))
			# Doesn't give you a PoolVector2Array with all the points!
			var points = PoolVector2Array()
			# So we'll iterate through and manually copy them ourselves :(
			for point in clipped_collision:
				points.push_back(point)
			
			# Update the existing polygon if possible
			if i == 0:
				collision_polygon.call_deferred("set", "polygon", points)
				
			else:
				# Otherwise, our clipping created independent islands!
				# We'll need to add a CollisionPolygon for each of them
				var collider := CollisionPolygon2D.new()
				collider.polygon = points
				CollisionHolder.call_deferred("add_child", collider)


func rebuild_collisions_from_image():
	var bitmap := BitMap.new()
	bitmap.create_from_image_alpha($Sprite.texture.get_data())
	
	# DEBUG:
	#$Sprite.get_texture().get_data().save_png("res://screenshots/debug.png")
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
	republish_sprite()

func republish_sprite() -> void:
	# Assume the image has changed, so we'll need to update our ImageTexture
	_image_republish_texture.create_from_image($Sprite.texture.get_data())

	# If our parent has the proper src/destruction/parent_material shader
	# We can set our destruction_mask parameter against it, 
	# which will carve out our destruction map!
	if _parent_material != null:
		_parent_material.set_shader_param("destruction_mask", _image_republish_texture)


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

func _world_to_viewport_scale(var original_scale : Vector2
		, var dynamic_texture_size : Vector2 = Vector2.ZERO) -> Vector2:
	if dynamic_texture_size == Vector2.ZERO	:
		dynamic_texture_size = $Viewport.get_size()
	var x_scale = ((100.0 / world_size.x) * dynamic_texture_size.x) / 100.0
	var y_scale = ((100.0 / world_size.y) * dynamic_texture_size.y) / 100.0
	return Vector2(original_scale.x * x_scale, original_scale.y * y_scale)
