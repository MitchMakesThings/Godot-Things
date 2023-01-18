extends CanvasLayer

func _ready():
	GameState.state_changed.connect(self.update_label)
	update_label()

func update_label() -> void:
	var string = "Button pressed " + str(GameState.button_press_count) + " times since server restart"
	if (GameState.mobs_killed > 0):
		string += "\nWhales killed: " + str(GameState.mobs_killed)
	$CenterContainer/Label.text = string
