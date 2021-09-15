extends MicroManagerLevel

func _input(event):
	if false and event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			var mapPos := world_to_map(to_local(event.position))
			set_cell(mapPos.x as int, mapPos.y as int, INVALID_CELL) # Clear the cell clicked on
			update_bitmask_area(mapPos) # Redraw auto-tiling
