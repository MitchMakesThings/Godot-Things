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
	if not body is Player:
		return
	isPressed = true
	if multiplayer.is_server():
		GameState.button_press_count += 1
		await get_tree().create_timer(reset_delay).timeout
		isPressed = false
