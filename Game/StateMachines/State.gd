extends Node2D
class_name State

var _slime : Slime
var _state_machine
var _velocity := Vector2.ZERO
var IsReady := false

func enter(_extra_params := []) -> void:
	_state_machine = get_parent()
	assert(_state_machine)
	_slime = _state_machine.get_slime()
	assert(_slime)
	IsReady = true

func exit() -> void:
	pass

func process(delta : float) -> void:
	if !_slime.is_on_floor():
		_velocity += Vector2.DOWN * delta * 300
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
