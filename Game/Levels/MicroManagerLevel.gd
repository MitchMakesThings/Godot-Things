extends TileMap
class_name MicroManagerLevel

export var acceptable_losses := 0
export var next_level : PackedScene

func _ready():
	GameManager.initialise(self, next_level, acceptable_losses)

func dig_under_unit(unit : Node2D) -> void:
	var mapPos := world_to_map(to_local(unit.global_position))
	set_cell(mapPos.x as int, mapPos.y as int + 1, INVALID_CELL) # Clear the cell clicked on
	update_bitmask_area(mapPos) # Redraw auto-tiling
