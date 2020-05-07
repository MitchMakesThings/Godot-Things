extends RigidBody2D

export var explosion_radius : float = 20
export var explosion_scene : PackedScene

func _on_SlimeGreen_body_entered(_body):
	get_tree().call_group("destructibles", "destroy", global_position, explosion_radius)
	
	var explosion = explosion_scene.instance()
	explosion.global_position = global_position
	get_parent().add_child(explosion)

	queue_free()
