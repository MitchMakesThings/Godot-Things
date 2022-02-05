extends Area2D

func _on_Door_body_entered(body):
	assert(body is Slime)
	body.get_state_machine().set_state("Escape", [position + Vector2(0, -16)])
