extends Area2D

@export
var pressed_texture : Texture2D
@export
var default_texture : Texture2D
@export
var reset_delay := 3.0

var isPressed : bool:
	set (newValue):
		if newValue:
			$Sprite2D.texture = pressed_texture
		else:
			$Sprite2D.texture = default_texture
		isPressed = newValue

func _on_blue_button_body_entered(body):
	if isPressed:
		return
	if not body is Character:
		return
	isPressed = true
	if multiplayer.is_server():
		GameState.button_press_count += 1
		
		# Murder a whale at random
		var mobs = get_tree().get_nodes_in_group("mobs")
		if not mobs.is_empty():
			mobs.pick_random().take_damage(100)
		
		# Reset the button after a delay
		await get_tree().create_timer(reset_delay).timeout
		isPressed = false
