extends CharacterBody2D
class_name Character

const SPEED = 600.0
const JUMP_FORCE = -400.0
const MAX_JUMP_FUEL = 100.0
const CAMERA_MAX_ZOOM := Vector2(0.5, 0.5)

var is_using_controller := false

@onready
var weapon = get_node("WeaponParent/Weapon")

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var jump_fuel := MAX_JUMP_FUEL

var is_jumping : bool:
	set(value):
		is_jumping = value
		# Note, this is a CPU Particles node!
		# Godot 4 currently has a bug where GPU Particles in global space are incorrectly offset
		# https://github.com/godotengine/godot/issues/56892
		$JetpackParticles.emitting = value

func is_local_authority():
	return $Networking/MultiplayerSynchronizer.get_multiplayer_authority() == multiplayer.get_unique_id()

func _ready():
	# int constructor taking a string is currently broken :(
	# https://github.com/godotengine/godot/issues/44407
	# https://github.com/godotengine/godot/issues/55284
	$Networking/MultiplayerSynchronizer.set_multiplayer_authority(str(name).to_int())
	
	if is_local_authority():
		$Camera2D.make_current()
	$UI.visible = is_local_authority()

func _process(_delta):
	$UI/TextureProgressBar.value = jump_fuel
	$UI/TextureProgressBar.visible = jump_fuel < MAX_JUMP_FUEL
	
	_rotate_weapon(_delta)

func _rotate_weapon(_delta : float) -> void:
	if not is_local_authority():
		$WeaponParent.rotation = $Networking.sync_weapon_rotation
		return
	var weapon_angle : float = 0
	var controller_input : Vector2 = Input.get_vector("aim_left", "aim_right", "aim_up", "aim_down")
	if controller_input.length_squared() > 0:
		weapon_angle = controller_input.angle()
		$WeaponParent.rotation = weapon_angle
		is_using_controller = true # First time we aim with a controller we activate controller-only mode
		return
	else:
		if is_using_controller:
			# We don't want to use the mouse aiming from here on.
			return
		weapon_angle = $WeaponParent.get_angle_to(get_global_mouse_position())
		$WeaponParent.rotate(weapon_angle)
	$Networking.sync_weapon_rotation = $WeaponParent.rotation


func _physics_process(delta):
	if !is_local_authority():
		if not $Networking.processed_position:
			position = $Networking.sync_position
			$Networking.processed_position = true
		velocity = $Networking.sync_velocity
		is_jumping = $Networking.sync_is_jumping
		
		move_and_slide()
		return

	# Add the gravity.
	if not is_on_floor():
		velocity.y += gravity * delta

	# Handle Jump.
	if Input.is_action_pressed("jump") and jump_fuel >= 0:
		velocity.y = JUMP_FORCE
		jump_fuel -= 1
		is_jumping = true
		
		Input.start_joy_vibration(0, .5, 0, 0.1)
		
		if $Camera2D.zoom.length() > CAMERA_MAX_ZOOM.length():
			$Camera2D.zoom = $Camera2D.zoom.lerp(CAMERA_MAX_ZOOM, 0.01)
	else:
		is_jumping = false
		$JetpackParticles.emitting = false
		var zoom = $Camera2D.zoom.length()
		if zoom < 1:
			$Camera2D.zoom = $Camera2D.zoom.lerp(Vector2(1, 1), zoom * 0.005)
	
	if is_on_floor():
		jump_fuel = MAX_JUMP_FUEL
		
	if Input.is_action_just_pressed("shoot_primary"):
		shoot()

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction = Input.get_axis("move_left", "move_right")
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)

	# Move locally
	move_and_slide()
	
	# Update sync variables, which will be replicated to everyone else
	$Networking.sync_position = position
	$Networking.sync_velocity = velocity
	$Networking.sync_is_jumping = is_jumping

func shoot():
	if weapon != null:
		weapon.shoot()
