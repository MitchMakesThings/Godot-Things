extends State
class_name DeathState

func _animation_finished():
	if _slime.get_animated_sprite().animation == "death":
		_state_machine.pop_state()

func enter():
	.enter()
	assert(_slime.get_animated_sprite())
	var connected = _slime.get_animated_sprite().connect("animation_finished", self, "_animation_finished")
	assert(connected == OK)
	_slime.get_animated_sprite().play("death")

func exit():
	.exit()
	_slime.queue_free()
