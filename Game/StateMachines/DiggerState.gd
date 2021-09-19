extends ActionState
class_name DiggerState

func enter(extra_params := []) -> void:
	if GameManager.digger_count <= 0:
		return
	.enter(extra_params)
	GameManager.digger_count -= 1

func process_finished():
	GameManager.get_current_level().dig_under_unit(_slime)
	.process_finished()
