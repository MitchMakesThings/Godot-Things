extends Node
class_name State

var _slime : Slime
var _state_machine
var _animated_sprite : AnimatedSprite
var IsReady := false

export var animated_sprite : NodePath

func enter():
	_state_machine = get_parent()
	assert(_state_machine)
	_slime = _state_machine.get_slime()
	assert(_slime)
	_animated_sprite = get_node(animated_sprite) as AnimatedSprite
	IsReady = true

func exit():
	pass

func process(delta : float) -> void:
	pass
