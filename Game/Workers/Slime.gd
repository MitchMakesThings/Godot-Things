extends KinematicBody2D

var _facing := Vector2.LEFT
var _velocity := Vector2.ZERO

export var Speed : float = 150
export var MaxSpeed : float = 100
export var SplatSpeed : float = 300

func _ready():
	$AnimatedSprite.play("walking")

func _physics_process(delta : float):
	if is_dying():
		return
	if !is_on_floor():
		_velocity += Vector2.DOWN * delta * 300
	else:
		_velocity += _facing * delta * Speed
		_velocity.x = min(abs(_velocity.x), MaxSpeed) * _facing.x

	if test_move(self.transform, _facing):
		change_direction()
		return

	var fallingSpeed = _velocity.y
	_velocity = move_and_slide(_velocity, Vector2.UP)
	# If we were falling before move_and_slide, but now are on the floor, 
	# that means we just splatted to the ground.
	if fallingSpeed > 0 and is_on_floor():
		if fallingSpeed > SplatSpeed:
			$AnimatedSprite.play("death")

func change_direction():
	_facing.x *= -1
	$AnimatedSprite.flip_h = !$AnimatedSprite.flip_h

func _on_AnimatedSprite_animation_finished():
	if is_dying():
		queue_free()

func is_dying():
	return $AnimatedSprite.animation == "death"
