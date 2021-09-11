extends State
class_name DefaultState

var _velocity := Vector2.ZERO

func enter() -> void:
	.enter()
	_animated_sprite.play("walking")

func process(delta : float) -> void:
	assert(_slime)
	if !_slime.is_on_floor():
		_velocity += Vector2.DOWN * delta * 300
	else:
		_velocity += _slime.get_facing() * delta * _slime.Speed
		_velocity.x = min(abs(_velocity.x), _slime.MaxSpeed) * _slime.get_facing().x

	if _slime.test_move(_slime.transform, _slime._facing):
		_slime.change_direction()
		return

	var fallingSpeed = _velocity.y
	_velocity = _slime.move_and_slide(_velocity, Vector2.UP)
	# If we were falling before move_and_slide, but now are on the floor, 
	# that means we just splatted to the ground.
	if fallingSpeed > 0 and _slime.is_on_floor():
		# If we splatted hard enough we die!
		if fallingSpeed > _slime.SplatSpeed:
			_state_machine.set_state("Death")
	# Check whether we collided with something on our insta-kill collision layer
	for i in _slime.get_slide_count():
		var collision = _slime.get_slide_collision(i)
		if collision.collider.collision_layer & 16 == 16:
			_state_machine.set_state("Death")
