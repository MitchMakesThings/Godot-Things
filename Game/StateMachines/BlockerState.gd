extends State
class_name BlockerState

func enter() -> void:
	.enter()
	_slime.set_collision_layer_bit(1, true)

func exit() -> void:
	.exit()
	_slime.set_collision_layer_bit(1, false)

func process(delta : float) -> void:
	# Blockers don't do anything!
	# TODO - add timer and count-down until blocker state expires
	pass
