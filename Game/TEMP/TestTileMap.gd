extends TileMap


func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			var mapPos := world_to_map(to_local(event.position))
			set_cell(mapPos.x, mapPos.y, INVALID_CELL) # Clear the cell clicked on
			update_bitmask_area(mapPos) # Redraw auto-tiling
