extends MarginContainer

func _ready():
	GameManager.connect("game_started", self, "_on_game_started")
	
func _on_game_started():
	queue_free()
