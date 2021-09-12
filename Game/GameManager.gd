extends Node

var _units := []
var _score := 0

func add_unit(unit) -> void:
	_units.append(unit)

func remove_unit(unit) -> void:
	var index = _units.find(unit)
	if index >= 0:
		_units.remove(index)

func count_units():
	return _units.size()

func add_score() -> void:
	_score += 1

func get_score() -> int:
	return _score

func _physics_process(_delta):
	# TODO move somewhere better than physics process!
	if count_units() <= 0:
		print("Game over! Score: ", _score)
