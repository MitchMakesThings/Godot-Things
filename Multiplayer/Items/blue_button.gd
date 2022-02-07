extends Area2D

@export
var pressed_texture : Texture2D

var isPressed : bool:
	set (newValue):
		if newValue:
			$Sprite2D.texture = pressed_texture
		isPressed = newValue

func _on_blue_button_body_entered(body):
	if isPressed:
		return
	if not body is Player:
		return
	isPressed = true
