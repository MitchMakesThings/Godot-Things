extends State
class_name IdleState

func enter(_extra_params := []) -> void:
	.enter(_extra_params)
	assert(_slime)
	_slime.get_animated_sprite().play("idle")
