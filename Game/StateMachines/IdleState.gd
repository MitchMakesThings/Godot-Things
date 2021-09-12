extends State
class_name IdleState

func enter(_extra_params := []) -> void:
	.enter(_extra_params)
	assert(_slime)
	GameManager.connect("game_started", self, "_on_game_started")
	_slime.get_animated_sprite().play("idle")

func _on_game_started():
	_slime.get_state_machine().set_state("Default")
