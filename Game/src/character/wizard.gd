extends KinematicBody2D

export (Vector2) var _speed = Vector2(400, 600)
export (Vector2) var gravity = Vector2(0, 1200)

var _velocity : Vector2 = Vector2.ZERO

export var _reticule_anchor_node_path : NodePath
onready var reticule_anchor : Node2D = get_node(_reticule_anchor_node_path)

export var weapon_projectile : PackedScene
export var weapon_speed : float = 3

var _attack_power : float = 0
var _attack_scale : float = 3
var _attack_clicked : bool = false

# When _attack_power reaches this we'll force the shot. (ie, this is the max cap of power for any 1 shot)
onready var _auto_attack_power : float = (reticule_anchor.get_child_count() / _attack_scale)

func _process(_delta : float):
	_rotate_reticule()
	_redraw_power()


func _unhandled_input(event):
	# Click and drag - begin / end clicking
	if event is InputEventMouseButton && event.button_index == BUTTON_LEFT:
		if event.is_pressed():
			_attack_clicked = true
		else:
			# We're checking _attack_clicked because it gets set to false if
			# we auto-fire because the player held the button for too long.
			if _attack_clicked:
				shoot()
			_attack_clicked = false


func shoot():
	# Spawn projectile
	var new_projectile := weapon_projectile.instance() as RigidBody2D
	var reticule := reticule_anchor.find_node("Reticule")
	new_projectile.global_position = reticule.global_position
	new_projectile.linear_velocity = (reticule.global_position - global_position) * weapon_speed * (_attack_power * _attack_scale)
	get_parent().add_child(new_projectile)
	
	# Reset the power-improvement meter
	_attack_power = 0
	_attack_clicked = false


func _rotate_reticule():
	reticule_anchor.rotate(reticule_anchor.get_angle_to(get_global_mouse_position()))


func _physics_process(_delta : float):
	if _attack_clicked:
		_attack_power += _delta
	
	# If the player has been holding the attack button for too long, we'll shoot for them.5
	if _attack_power >= _auto_attack_power:
		shoot()
		
	var input_direction = _get_input_direction()
	
	_velocity = _calculate_move_velocity(_velocity, input_direction, _speed)
	
	_velocity = move_and_slide(_velocity, Vector2(0, -1))
	
	# Animation stuff
	if _velocity.x >= 1:
		$CharacterSprite.flip_h = false
	elif _velocity.x <= -1:
		$CharacterSprite.flip_h = true


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


func _redraw_power():
	var sprites_to_show : int = int(_attack_power * _attack_scale)
	
	# Note - we ignore the last child, as we don't want to hide the reticule!
	for i in range(reticule_anchor.get_child_count() - 1):
		if i < sprites_to_show:
			reticule_anchor.get_child(i).visible = true
		else:
			reticule_anchor.get_child(i).visible = false
