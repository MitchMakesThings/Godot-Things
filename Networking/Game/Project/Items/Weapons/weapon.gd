extends Sprite2D
class_name Weapon

func _ready():
	# TODO set reticule distance based on weapon config file
	# Preload damage etc
	pass

func shoot() -> void:
	print("Bang!")
	Input.start_joy_vibration(0, 0, 1, 0.1)
