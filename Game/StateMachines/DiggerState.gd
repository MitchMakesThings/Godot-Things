extends State
class_name DiggerState

var _progress : TextureProgress
var _internal_counter : float = 0

func enter(extra_params := []) -> void:
	.enter(extra_params)
	_slime.get_animated_sprite().play("idle")
	_progress = $Offset/TextureProgress
	assert(_progress)
	_progress.visible = true
	print(name, " you're a digger!")
	
func exit() -> void:
	.exit()
	_slime.set_collision_layer_bit(1, false)
	_progress.visible = false

func process(delta : float) -> void:
	# Blockers don't do anything!
	# TODO - add timer and count-down until blocker state expires
	.process(delta)
	_internal_counter += delta * 100
	if _internal_counter >= 1:
		_internal_counter = 0
		_progress.set_value(_progress.value + _progress.step)
		if _progress.value >= 100:
			GameManager.get_current_level().dig_under_unit(_slime)
			_state_machine.pop_state()
