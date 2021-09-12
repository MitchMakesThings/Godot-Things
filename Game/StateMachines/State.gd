extends Node
class_name State

var _slime : Slime
var _state_machine
var IsReady := false

func enter():
	_state_machine = get_parent()
	assert(_state_machine)
	_slime = _state_machine.get_slime()
	assert(_slime)
	IsReady = true

func exit():
	pass

func process(_delta : float) -> void:
	pass
