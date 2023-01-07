extends Node

signal button_press_count_changed(newValue : int)

var button_press_count := 0:
	set(value):
		print('button press updated: ', value)
		button_press_count = value
		emit_signal(StringName("button_press_count_changed"), value)
