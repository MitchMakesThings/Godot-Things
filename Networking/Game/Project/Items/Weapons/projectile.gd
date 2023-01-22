extends CharacterBody2D

const SPEED = 80

func _physics_process(_delta : float):
	velocity = global_transform.x * SPEED
	var collision = move_and_collide(velocity)
	
	if collision == null:
		return
	
	var collider = collision.get_collider()
	if collider is Mob:
		collider.take_damage(1)
	queue_free()
