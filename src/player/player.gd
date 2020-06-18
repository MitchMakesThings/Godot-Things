extends KinematicBody2D
class_name Player

export (Vector2) var _speed = Vector2(400, 600)
export (Vector2) var gravity = Vector2(0, 1200)

var _velocity : Vector2 = Vector2.ZERO

func _physics_process(delta : float):
	var input_direction = _get_input_direction()
	_velocity = _calculate_move_velocity(_velocity, input_direction, _speed)
	_handle_animation(_velocity)
	
	_velocity = move_and_slide(_velocity, Vector2(0, -1))

func _get_input_direction() -> Vector2:
	return Vector2(
			Input.get_action_strength("move_right") - Input.get_action_strength("move_left"), 
			-1 if Input.is_action_just_pressed("jump") and is_on_floor() else 0
		)

func _calculate_move_velocity(
		linear_velocity: Vector2,
		direction: Vector2,
		speed: Vector2
	):
		var new_velocity := linear_velocity
		new_velocity.x = speed.x * direction.x
		
		# Apply gravity
		new_velocity += gravity * get_physics_process_delta_time()
		
		# If player is jumping
		if direction.y == -1:
			new_velocity.y = speed.y * direction.y
		
		return new_velocity

func _handle_animation(linear_velocity : Vector2):
	# Arrange directional facing
	# Note, we're not using $Sprite.flip_h because the animations aren't all symmetrical
	# ie, our run and idle need the hitboxes to be in different places. The animation player handles the offsets,
	# but offsets to the collision shapes aren't reflected when using flip_h
	# Instead, we're flipping the sprite node ourself, it then inherits all the offsets etc correctly.
	if linear_velocity.x >= 1:
		$Sprite.set_scale(Vector2(abs($Sprite.get_scale().x), $Sprite.get_scale().y))
	elif linear_velocity.x <= -1:
		$Sprite.set_scale(Vector2(-abs($Sprite.get_scale().x), $Sprite.get_scale().y))

	# Handle horizontal movement
	if linear_velocity.x >= 1:
		$AnimationPlayer.play("run")
	elif linear_velocity.x <= -1:
		$AnimationPlayer.play("run")
	else:
		$AnimationPlayer.play("idle")
