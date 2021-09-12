extends State
class_name BlockerState

var _previous_animation : String

func enter(extra_params := []) -> void:
	.enter(extra_params)
	_slime.set_collision_layer_bit(1, true)
	_previous_animation = _slime.get_animated_sprite().animation
	_slime.get_animated_sprite().play("idle")
	

func exit() -> void:
	.exit()
	_slime.set_collision_layer_bit(1, false)
	_slime.get_animated_sprite().play(_previous_animation)

func process(delta : float) -> void:
	# Blockers don't do anything!
	# TODO - add timer and count-down until blocker state expires
	.process(delta)
