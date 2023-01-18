extends CharacterBody2D
class_name Mob

var SPEED := 80
var velocity_sync : Vector2
var global_pos_sync : Vector2

var health_sync := 3:
	set (value):
		var has_changed = value != health_sync
		health_sync = value
		if has_changed:
			flash_damage()
	get:
		return health_sync

func _physics_process(_delta : float):
	if not multiplayer.is_server():
		global_position = global_pos_sync
		velocity = velocity_sync
		move_and_slide()
		return
		
	if health_sync <= 0:
		velocity_sync = Vector2.ZERO
		return
	
	# TODO select a target location from the list, and try to move there
	var direction := (Vector2(1618, -1455) - global_position).normalized()
	
	velocity = direction * SPEED
	move_and_slide()
	velocity_sync = velocity
	global_pos_sync = global_position

func take_damage(amount : int):
	if multiplayer.is_server():
		health_sync -= amount

func flash_damage():
	var tween = get_tree().create_tween()
	tween.tween_property($Sprite2D, "modulate", Color.RED, 0.1)
	tween.tween_property($Sprite2D, "modulate", Color.WHITE, 0.3)
	
	# TODO explode in a cloud of red particles, then queue_free once they're done
	if health_sync <= 0:
		# Fade out of visibility and trigger blood particles
		tween.tween_property($Sprite2D, "modulate", Color("ffffff00"), 0.1)
		$GPUParticles2D.emitting = true
		
		if multiplayer.is_server():
			GameState.mobs_killed += 1
			# Note that we only queue_free on the server.
			# The MultiplayerSpawner will tell the clients to delete their instances
			tween.tween_callback(queue_free)
