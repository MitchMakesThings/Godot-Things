extends Area2D

@export
var pressed_texture : Texture2D
var isPressedProcessed := false

var isPressed : bool:
	set (newValue):
		if newValue == isPressed:
			return
		if newValue:
			$Sprite2D.texture = pressed_texture
		isPressed = newValue

func _process(delta):
	print(isPressed)

func _on_blue_button_body_entered(body):
	if not is_multiplayer_authority():
		return
	if isPressed:
		return
	if not body is Player:
		return
	isPressed = true
