extends State
class_name IdleState

func enter(_extra_params := []) -> void:
	.enter(_extra_params)
	assert(_slime)
	if !GameManager.is_connected("game_started", self, "_on_game_started"):
		var connection = GameManager.connect("game_started", self, "_on_game_started")
		assert(connection == OK)
	_slime.get_animated_sprite().play("idle")

func _on_game_started():
	_slime.get_state_machine().set_state("Default")
