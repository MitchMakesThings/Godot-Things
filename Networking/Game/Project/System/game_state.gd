extends Node

signal state_changed

var button_press_count := 0:
	set(value):
		button_press_count = value
		state_changed.emit()

var mobs_killed := 0:
	set(value):
		mobs_killed = value
		state_changed.emit()
