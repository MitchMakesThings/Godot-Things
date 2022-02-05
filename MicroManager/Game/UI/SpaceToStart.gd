extends MarginContainer

func _ready():
	var connection = GameManager.connect("game_started", self, "_on_game_started")
	assert(connection == OK)
	
func _on_game_started():
	queue_free()
