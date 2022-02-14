extends TileMap

func _ready():
	if multiplayer.is_server():
		# Spawn test button
		var btn = preload("res://Items/blue_button.tscn").instantiate()
		btn.position = Vector2(386, 351)
		$NetworkedNodes.add_child(btn)
