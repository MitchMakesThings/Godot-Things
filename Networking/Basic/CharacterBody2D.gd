extends CharacterBody2D

const SPEED = 300.0
const JUMP_FORCE = -400.0

var sync_position := Vector2.ZERO

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

func is_local_authority() -> bool:
	return name == str(multiplayer.get_unique_id())

func _physics_process(delta):
	if not is_local_authority():
		position = sync_position
		return

	# Add the gravity.
	if not is_on_floor():
		velocity.y += gravity * delta

	# Handle Jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_FORCE

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction = Input.get_axis("ui_left", "ui_right")
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
	move_and_slide()
	
	# Tell the server our new position
	rpc_id(1, StringName('push_to_server'), position)

@rpc("any_peer", "unreliable_ordered")
func push_to_server(newPosition : Vector2):
	if not multiplayer.is_server():
		return
	if name != str(multiplayer.get_remote_sender_id()):
		print('someone being naughty! ', multiplayer.get_remote_sender_id(), ' tried to update ', name)
		return
	sync_position = newPosition
