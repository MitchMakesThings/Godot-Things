extends Node

var button_press_count := 0:
	set(value):
		print('button press updated: ', value)
		button_press_count = value
