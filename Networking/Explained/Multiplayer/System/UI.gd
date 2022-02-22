extends CanvasLayer

func _ready():
	GameState.button_press_count_changed.connect(self.update_label)

func update_label(newScore : int) -> void:
	$CenterContainer/Label.text = "Button pressed " + str(newScore) + " times since server restart"
