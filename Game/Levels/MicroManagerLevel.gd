extends TileMap
class_name MicroManagerLevel

export var acceptable_losses := 0
export var next_level : PackedScene

export var blocker_count := 1
export var digger_count := 1
export var ladder_count := 1

func _ready():
	GameManager.initialise(self, next_level, acceptable_losses, blocker_count, digger_count, ladder_count)

func dig(global_pos : Vector2) -> void:
	var mapPos := world_to_map(to_local(global_pos))
	set_cell(mapPos.x as int, mapPos.y as int , INVALID_CELL) # Clear the cell clicked on
	update_bitmask_area(mapPos) # Redraw auto-tiling
	update_dirty_quadrants()
