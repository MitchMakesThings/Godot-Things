extends ActionState
class_name BlockerState

func enter(extra_params := []) -> void:
	if GameManager.blocker_count <= 0:
		return
	.enter(extra_params)
	_slime.set_collision_layer_bit(1, true)
	GameManager.blocker_count -= 1

func exit() -> void:
	.exit()
	_slime.set_collision_layer_bit(1, false)
