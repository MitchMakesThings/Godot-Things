extends Node2D


export var _next_level : PackedScene

func _on_Button_pressed():
	var changed = get_tree().change_scene_to(_next_level)
	assert(changed == OK)
