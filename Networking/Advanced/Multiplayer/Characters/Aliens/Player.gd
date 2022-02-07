extends CharacterBody2D
class_name Player

const SPEED = 300.0
const JUMP_FORCE = -400.0
const MAX_JUMP_FUEL = 100.0
const CAMERA_MAX_ZOOM := Vector2(2.5, 2.5)

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var jump_fuel := MAX_JUMP_FUEL

var sync_position : Vector2:
	set(value):
		sync_position = value
		if not is_multiplayer_authority():
			position = value

var sync_motion_velocity : Vector2
var sync_is_jumping : bool:
	set(value):
		sync_is_jumping = value
		if not is_multiplayer_authority():
			# Note, this is a CPU Particles node!
			# Godot 4 currently has a bug where GPU Particles in global space are incorrectly offset
			# https://github.com/godotengine/godot/issues/56892
			$JetpackParticles.emitting = value

func _ready():
	# int constructor taking a string is currently broken :(
	# https://github.com/godotengine/godot/issues/44407
	# https://github.com/godotengine/godot/issues/55284
	set_multiplayer_authority(str(name).to_int())
	
	$Camera2D.current = is_multiplayer_authority()
	$UI.visible = is_multiplayer_authority()

func _process(_delta):
	$UI/TextureProgressBar.value = jump_fuel
	$UI/TextureProgressBar.visible = jump_fuel < MAX_JUMP_FUEL

func _physics_process(delta):
	if !is_multiplayer_authority():
		# TODO look at these numbers in depth!
		# We're trying to smoothly sync the local position of other clients to the server
		if position.distance_squared_to(sync_position) > 100:
			position = position.lerp(sync_position, 0.8)
			print(position.distance_squared_to(sync_position))
		motion_velocity = sync_motion_velocity
		move_and_slide()
		return

	# Add the gravity.
	if not is_on_floor():
		motion_velocity.y += gravity * delta

	# Handle Jump.
	if Input.is_action_pressed("jump") and jump_fuel >= 0:
		motion_velocity.y = JUMP_FORCE
		jump_fuel -= 1
		sync_is_jumping = true
		# If we drive particles based on sync_is_jumping, our locally written value
		# gets overridden on the next packet from the server.
		# So we'll handle our particles locally, 
		# and let sync_is_jumping manage the other clients.
		$JetpackParticles.emitting = true
		
		if $Camera2D.zoom.length() < CAMERA_MAX_ZOOM.length():
			$Camera2D.zoom = $Camera2D.zoom.lerp(CAMERA_MAX_ZOOM, 0.01)
	else:
		sync_is_jumping = false
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
	
	# Sync position to the server
	rpc_id(1, StringName('push_to_server'), position, motion_velocity, sync_is_jumping)
	
	# Move locally
	move_and_slide()

@rpc(unreliable_ordered)
func push_to_server(newPosition : Vector2, motion : Vector2, is_jumping : bool):
	# Validate!
	if not multiplayer.is_server():
		print('someone being naughty! ', multiplayer.get_remote_sender_id(), ' tried to update ', name)
		return
	sync_position = newPosition
	sync_is_jumping = is_jumping
	sync_motion_velocity = motion
