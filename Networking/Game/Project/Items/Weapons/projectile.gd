extends CharacterBody2D

const SPEED = 800

func _physics_process(_delta : float):
	velocity = global_transform.x * SPEED
	if move_and_slide():
		queue_free()
