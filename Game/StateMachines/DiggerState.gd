extends ActionState
class_name DiggerState

func enter(extra_params := []) -> void:
	if GameManager.digger_count <= 0:
		return
	.enter(extra_params)
	GameManager.digger_count -= 1

func process_finished():
	var dig_pos = Vector2(_slime.global_position.x, _slime.global_position.y + 10)
	dig_pos = dig_pos + (_slime.get_facing() * 20)
	GameManager.get_current_level().dig(dig_pos)
	.process_finished()
