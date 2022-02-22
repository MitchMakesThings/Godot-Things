extends CharacterBody2D
class_name Player

const SPEED = 300.0
const JUMP_FORCE = -400.0
const MAX_JUMP_FUEL = 100.0
const CAMERA_MAX_ZOOM := Vector2(2.5, 2.5)

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var jump_fuel := MAX_JUMP_FUEL

var is_jumping : bool:
	set(value):
		is_jumping = value
		if not is_local_authority():
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
	
	$Camera2D.current = is_local_authority()
	$UI.visible = is_local_authority()

func _process(_delta):
	$UI/TextureProgressBar.value = jump_fuel
	$UI/TextureProgressBar.visible = jump_fuel < MAX_JUMP_FUEL

func _physics_process(delta):
	if !is_local_authority():
		if not $Networking.processed_position:
			position = $Networking.sync_position
			$Networking.processed_position = true
		motion_velocity = $Networking.sync_motion_velocity
		is_jumping = $Networking.sync_is_jumping
		
		move_and_slide()
		return

	# Add the gravity.
	if not is_on_floor():
		motion_velocity.y += gravity * delta

	# Handle Jump.
	if Input.is_action_pressed("jump") and jump_fuel >= 0:
		motion_velocity.y = JUMP_FORCE
		jump_fuel -= 1
		is_jumping = true
		# If we drive particles based on is_jumping, our locally written value
		# gets overridden on the next packet from the server.
		# So we'll handle our particles locally, 
		# and let is_jumping manage the other clients.
		$JetpackParticles.emitting = true
		
		if $Camera2D.zoom.length() < CAMERA_MAX_ZOOM.length():
			$Camera2D.zoom = $Camera2D.zoom.lerp(CAMERA_MAX_ZOOM, 0.01)
	else:
		is_jumping = false
		$JetpackParticles.emitting = false
		var zoom = $Camera2D.zoom.length()
		if zoom > 1:
			$Camera2D.zoom = $Camera2D.zoom.lerp(Vector2(1, 1), zoom * 0.005)
	
	if is_on_floor():
		jump_fuel = MAX_JUMP_FUEL

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction = Input.get_axis("move_left", "move_right")
	if direction:
		motion_velocity.x = direction * SPEED
	else:
		motion_velocity.x = move_toward(motion_velocity.x, 0, SPEED)

	# Move locally
	move_and_slide()
	
	# Update sync variables, which will be replicated to everyone else
	$Networking.sync_position = position
	$Networking.sync_motion_velocity = motion_velocity
	$Networking.sync_is_jumping = is_jumping
