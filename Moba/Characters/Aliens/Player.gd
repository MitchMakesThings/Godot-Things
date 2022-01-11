extends CharacterBody2D

const SPEED = 600.0
const JUMP_FORCE = -600.0

# Get the gravity from the project settings to be synced with RigidDynamicBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")


func _physics_process(delta):
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

	move_and_slide()
