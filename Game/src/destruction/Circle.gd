extends Node2D

# Controls the size of the explosion
var _radius = 40


# Reposition and redraw the circle
func reposition(position : Vector2, radius : float):
	# Ensure we're visible
	visible = true

	# Reposition and resize
	global_position = position
	_radius = radius

	# Now that we've been repositioned and resized we need to call update to force a redraw
	update()


# Note - this is only called once after the node is added to the scene.
# In order to redraw we need our update() method to be called!
func _draw():
	# Draw a circle at our current position using the built in CanvasItem method
	# We set our position for the circle to zero, so it draws exactly where our global_position is.
	draw_circle( Vector2.ZERO , _radius, Color.black )
 
