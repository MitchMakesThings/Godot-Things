extends TileMap
class_name MicroManagerLevel

export var acceptable_losses := 0
export var next_level : PackedScene

func _ready():
	GameManager.initialise(next_level, acceptable_losses)
