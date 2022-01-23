extends CharacterBody2D
class_name Player

const SPEED = 300.0
const JUMP_FORCE = -400.0

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var sync_position : Vector2

func _ready():
	$Camera2D.current = is_local_authority()

func is_local_authority() -> bool:
	return name == str(multiplayer.get_unique_id())

func _physics_process(delta):
	if !is_local_authority():
		# TODO lerp to sync_position
		position = sync_position
		return

	# Add the gravity.
	if not is_on_floor():
		motion_velocity.y += gravity * delta

	# Handle Jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		motion_velocity.y = JUMP_FORCE

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction = Input.get_axis("move_left", "move_right")
	if direction:
		motion_velocity.x = direction * SPEED
	else:
		motion_velocity.x = move_toward(motion_velocity.x, 0, SPEED)
	
	# Move locally
	move_and_slide()
	
	# Sync position to the server
	rpc_id(1, StringName('push_to_server'), position)

@rpc(any_peer, unreliable_ordered)
func push_to_server(newPosition : Vector2):
	# Validate!
	if name != str(multiplayer.get_remote_sender_id()):
		print('someone being naughty! ', multiplayer.get_remote_sender_id(), ' tried to update ', name)
		return
	sync_position = newPosition
