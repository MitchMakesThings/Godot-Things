extends Area2D


func _on_Ladder_body_entered(body):
	var slime = body as Slime
	assert(slime)
	slime.get_state_machine().push_state("LadderClimb", [position + Vector2(0, -32)])
