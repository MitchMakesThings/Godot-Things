extends State
class_name DeathState

func _animation_finished():
	if _animated_sprite.animation == "death":
		_state_machine.pop_state()

func enter():
	.enter()
	assert(_animated_sprite)
	_animated_sprite.connect("animation_finished", self, "_animation_finished")
	_animated_sprite.play("death")

func exit():
	.exit()
	_slime.queue_free()
