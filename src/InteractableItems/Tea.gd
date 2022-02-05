extends ConsumableItem

func interaction_interact(interactionComponentParent : Node) -> void:
	print("Drank tea!")
	queue_free()
