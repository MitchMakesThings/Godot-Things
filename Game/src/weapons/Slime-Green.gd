extends RigidBody2D

export var explosion_radius : float = 40

func _on_SlimeGreen_body_entered(_body):
	get_tree().call_group("destructibles", "destroy", global_position, explosion_radius)
	queue_free()
