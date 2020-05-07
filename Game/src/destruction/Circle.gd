extends Node2D

# Controls the size of the explosion
var radius = 80

# Private variables - please don't change below here

# Offset from the node origin that we should draw the circle.
# We don't want to mess with this, so that the circle can simply be moved
# using the global_position property.
var _center = Vector2(0, 0)

# Colour is hardcoded to black
var _color = Color.black

# Called every frame
func _draw():
	# Draw a circle at our current position using the built in CanvasItem method
	draw_circle( _center, radius, _color )
