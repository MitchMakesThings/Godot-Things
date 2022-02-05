extends Camera2D


var _previous_position : Vector2 = Vector2.ZERO
var _move_camera := false

func _unhandled_input(event):
	# Click and drag - begin / end clicking
	if event is InputEventMouseButton && event.button_index == BUTTON_RIGHT:
		get_tree().set_input_as_handled()
		if event.is_pressed():
			_previous_position = event.position
			_move_camera = true
		else:
			_move_camera = false
	
	# Click and drag - dragging
	elif event is InputEventMouseMotion && _move_camera:
		get_tree().set_input_as_handled()
		position += (_previous_position - event.position)
		_previous_position = event.position

	# Zoom
	elif event is InputEventMouseButton:
		var new_zoom := Vector2.ZERO
		if event.button_index == BUTTON_WHEEL_UP:
			new_zoom = zoom.linear_interpolate(Vector2(0.5, 0.5), 0.2)
		elif event.button_index == BUTTON_WHEEL_DOWN:
			new_zoom = zoom.linear_interpolate(Vector2(4,4), 0.2)
		
		if (new_zoom != Vector2.ZERO):
			get_tree().set_input_as_handled()
			zoom = new_zoom
			
