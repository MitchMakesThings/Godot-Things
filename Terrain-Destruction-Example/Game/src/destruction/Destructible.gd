extends Node2D

export var viewport_destruction_nodepath : NodePath
export var collision_holder_node_path : NodePath

var world_size : Vector2

var collision_holder : Node2D
var _to_cull : Array

var _image_republish_texture := ImageTexture.new()

var _parent_material : Material
var _destruction_threads := Array()
var _viewport_destruction_node : Node

func _ready():
	add_to_group("destructibles")
	
	collision_holder = get_node(collision_holder_node_path)
	world_size = (get_parent() as Sprite).get_rect().size
	_parent_material = get_parent().material
	_viewport_destruction_node = get_node(viewport_destruction_nodepath)
	
	# Set our viewport size. We don't know this until run time, since its from our parent.
	$Viewport.set_size(world_size)
	
	# Passing 0 to duplicate, as we don't want to duplicate scripts/signals etc
	# We don't use 8 since we're going to delete our duplicate nodes after first render anyway
	var dup = get_parent().duplicate(0) as Node2D
	_to_cull.append(dup)
	
	# Then reposition, so we're in the right spot
	dup.position = _world_to_viewport(dup.position)

	# Add to the viewport, so that our destructible viewport has our starting point
	$Viewport.add_child(dup)
	
	# Start the timer, so it can delete our duplicated parent info
	$CullTimer.start()
	
	# Wait for all viewports to re-render before we build our image
	yield(VisualServer, "frame_post_draw")
	build_collisions_from_image()


func _exit_tree():
	for thread in _destruction_threads:
		thread.wait_to_finish()


func _unhandled_input(event):
	if (event.is_action_pressed("ui_accept")):
		# DEBUG:
		var bitmap := BitMap.new()
		bitmap.create_from_image_alpha($Sprite.texture.get_data())
		$Sprite.get_texture().get_data().save_png("res://screenshots/debug" + get_parent().name + ".png")


func destroy(position : Vector2, radius : float):
	# Collision rebuild thread!
	var thread := Thread.new()
	var error = thread.start(self, "rebuild_collisions_from_geometry", [position, radius])
	if error != OK:
		print("Error creating destruction thread: ", error)
	_destruction_threads.push_back(thread)
	
	# Move our subtractive-circle so that our Viewport deletes pixels that were in our explosion
	_viewport_destruction_node.reposition(_world_to_viewport(position), radius)
	# Re-render the viewport into our texture
	rebuild_texture()

	# Wait until all viewports have re-rendered before pushing our viewport to the destruction shader.
	yield(VisualServer, "frame_post_draw")
	republish_sprite()


func _cull_foreground_duplicates():
	for dup in _to_cull:
		dup.queue_free()
	_to_cull = Array()


func rebuild_texture():
	# Force re-render to update our target viewport
	$Viewport.render_target_update_mode = Viewport.UPDATE_ONCE


# Improved collision rebuilding!
func rebuild_collisions_from_geometry(arguments : Array):
	var position : Vector2 = arguments[0]
	var radius : float = arguments[1]

	# Convert world coordinates of the collision point to local coordinates
	# We need to do this because the collision polygon coordinates are only available in local space
	position = position - global_position

	var nb_points = 8
	var points_arc = PoolVector2Array()
	points_arc.push_back(position)

	for i in range(nb_points + 1):
		var angle_point = deg2rad(i * 360 / nb_points)
		points_arc.push_back(position + Vector2(cos(angle_point), sin(angle_point)) * radius)

	for collision_polygon in collision_holder.get_children():
		var clipped_polygons = Geometry.clip_polygons_2d(collision_polygon.polygon, points_arc)
		
		# If the clip failed, we're almost certainly trying to delete the last few
		# remnants of an 'island'
		if clipped_polygons.size() == 0:
			collision_polygon.queue_free()
		
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
			# So we'll iterate through and manually copy them ourselves :(
			var points = PoolVector2Array()
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
				collision_holder.call_deferred("add_child", collider)


func build_collisions_from_image():	
	# Create bitmap from the Viewport (which projects into our sprite)
	var bitmap := BitMap.new()
	bitmap.create_from_image_alpha($Sprite.texture.get_data())
	
	# DEBUG:
	#$Sprite.get_texture().get_data().save_png("res://screenshots/debug" + get_parent().name + ".png")
	#print("Saved")

	# This will generate polygons for the given coordinate rectangle within the bitmap
	# In our case, our given coordinates are the entire image.
	var polygons = bitmap.opaque_to_polygons(Rect2(Vector2(0,0), bitmap.get_size()))

	# Now create a collision polygon for each polygon returned
	# For the most part there will probably only be one.... unless you have islands
	for polygon in polygons:
		var collider := CollisionPolygon2D.new()

		# Remap our points from the viewport coordinates back to world coordinates.
		var newpoints := Array()
		for point in polygon:
			newpoints.push_back(_viewport_to_world(point))
		collider.polygon = newpoints
		collision_holder.add_child(collider)


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
	var parent_offset = get_parent().position
	return Vector2(
		((point.x - parent_offset.x ) / world_size.x) * dynamic_texture_size.x + get_viewport_rect().position.x,
		((point.y - parent_offset.y ) / world_size.y) * dynamic_texture_size.y + get_viewport_rect().position.y
	)
