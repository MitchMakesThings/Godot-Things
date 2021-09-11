extends KinematicBody2D

var _facing := Vector2.LEFT
var _velocity := Vector2.ZERO

export var Speed : float = 150
export var MaxSpeed : float = 100

func _physics_process(delta : float):
	if !is_on_floor():
		_velocity += Vector2.DOWN * delta * 300
	else:
		_velocity += _facing * delta * Speed
		_velocity.x = min(abs(_velocity.x), MaxSpeed) * _facing.x

	if test_move(self.transform, _facing):
		change_direction()
		return

	_velocity = move_and_slide(_velocity, Vector2.UP)

func change_direction():
	_facing.x *= -1
	$AnimatedSprite.flip_h = !$AnimatedSprite.flip_h
