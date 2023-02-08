extends CharacterBody2D
class_name Mob

var SPEED := 100
var global_pos_sync : Vector2
var is_dead := false

@export
var targets : Array[Vector2] = []
var target_sync : Vector2

var health_sync := 3:
	# We're using a setget variable here. It's updated by the MultiplayerSynchronizer
	# This way when the server broadcasts a change in health all clients can show the damage flash animation.
	set (value):
		# Because MultiplayerSynchronizer sends the health_sync value ~60 times a second
		# (See MultiplayerSynchronizer nodes Replication Interval)
		# We need to check to see if it's actually changed before flashing damage!
		# An alternative (better!) approach would be to make flash_damage an @rpc
		# Then the server could call flash_damage when the health changes. All clients would show the animation.
		# But I thought it was good to show off how you could use a setget variable :)
		var has_changed = value != health_sync
		health_sync = value
		if has_changed:
			flash_damage()
	get:
		return health_sync
		
func _ready():
	# Remember: _ready() runs on _all_ computers - server & all clients
	pick_new_target()

func _physics_process(_delta : float):
	if not multiplayer.is_server():
		global_position = global_position.lerp(global_pos_sync, 0.1)
		return
	
	# Move in a straight line towards our target
	# Note that we're not pathfinding, so we can't work our way around obstacles.
	# You can see this in the example scene, where mobs will slide along obstacles.
	var direction := (target_sync - global_position)
	velocity = direction.normalized() * SPEED
	move_and_slide()
	global_pos_sync = global_position
	
	# If we've moved close enough to our target destination pick a new one
	# This would let you implement guard waypoint pathing etc.
	# This size should be adjusted based on the size of your mobs
	if direction.length() <= 80:
		pick_new_target()

func pick_new_target() -> void:
	# We only want the server to pick a target. It'll then sync with all the clients.
	# If we didn't check that we were the server different clients could pick different targets
	# Since we're syncing position directly, it wouldn't cause problems in this example
	# But it might catch you out in your projects!
	if multiplayer.is_server():
		target_sync = targets[randi() % len(targets)]

func take_damage(amount : int):
	# Note that our projectile always calls take_damage, on the server & all clients
	# We only want to process damage on the server, since it's our source of truth.
	if multiplayer.is_server():
		health_sync -= amount

func flash_damage():
	var tween = get_tree().create_tween()
	tween.tween_property($Sprite2D, "modulate", Color.RED, 0.1)
	tween.tween_property($Sprite2D, "modulate", Color.WHITE, 0.3)
	
	if health_sync <= 0:
		# Fade out of visibility and trigger blood particles
		tween.tween_property($Sprite2D, "modulate", Color("ffffff00"), 0.1)
		$GPUParticles2D.emitting = true
		
		if multiplayer.is_server() and not is_dead:
			is_dead = true
			GameState.mobs_killed += 1
			# Note that we only queue_free on the server.
			# The MultiplayerSpawner will tell the clients to delete their instances
			tween.tween_callback(queue_free)
