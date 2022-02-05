extends ConsumableItem

var remaining_uses := 3

func interaction_interact(interactionComponentParent : Node) -> void:
	print("Drank coffee!")
	remaining_uses -= 1
	if (remaining_uses <= 0):
		queue_free()
