extends Node2D


export var slime_scene : PackedScene

var _facing := Vector2(1, 0)

func _physics_process(delta):
	position += _facing * 100
	if position.x > 900 or position.x < 100:
		_facing *= -1


func _on_Timer_timeout():
	var slime = slime_scene.instance()
	slime.global_position = global_position
	get_parent().add_child(slime)
