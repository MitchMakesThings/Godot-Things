extends State
class_name DefaultState

func enter() -> void:
	.enter()
	_slime.get_animated_sprite().play("walking")

func process(delta : float) -> void:
	assert(_slime)
	if _slime.is_on_floor():
		_velocity += _slime.get_facing() * delta * _slime.Speed
		_velocity.x = min(abs(_velocity.x), _slime.MaxSpeed) * _slime.get_facing().x

	if _slime.test_move(_slime.transform, _slime._facing):
		_slime.change_direction()
		return

	# Call main state process function.
	# It applies gravity, and checks collisions against insta-kill areas
	.process(delta)
