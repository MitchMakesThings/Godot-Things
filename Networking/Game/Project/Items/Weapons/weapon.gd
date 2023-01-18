extends Sprite2D
class_name Weapon

@export
var projectile_scene : PackedScene

var character : Character

func _ready():
	# TODO set reticule distance based on weapon config file
	# Preload damage etc
	character = get_parent().get_parent()

# Called when the player pulls the trigger (shoot_primary)
func shoot() -> void:
	if not character.is_local_authority():
		print("This should never happen!")
		return
	rpc_id(1, "shoot_server", global_position, get_parent().global_rotation)
	shoot_impl(global_position, get_parent().global_rotation)

@rpc(any_peer) # Secretly only called on the server, but using any_peer because we're fudging authority.
func shoot_server(pos : Vector2, rot : float) -> void:
	var caller_id = multiplayer.get_remote_sender_id()
	if str(character.name).to_int() != caller_id:
		print("Illegally calling shoot_server! The culprit is: " + str(caller_id))
		return
	rpc("shoot_client", pos, rot)
	shoot_impl(pos, rot) # Also do visuals etc on server. It might not always be headless!

@rpc # Called on _all_ clients
func shoot_client(pos : Vector2, rot : float) -> void:
	if character.is_local_authority():
		# Don't do anything on the client that initiated the shot
		# They will already have called the implementation to show bullets etc
		return
	shoot_impl(pos, rot)

# Handle all the effects (visual & audio)
# This is called on all clients, including the server & the client initiating the shot
func shoot_impl(pos : Vector2, rot : float) -> void:
	Input.start_joy_vibration(0, 0, 1, 0.1)
	# TODO Sound effect
	
	var projectile = projectile_scene.instantiate()
	projectile.add_collision_exception_with(character)
	get_tree().current_scene.add_child(projectile)
	projectile.global_position = pos
	projectile.global_rotation = rot
