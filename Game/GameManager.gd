extends Node

var _units := []

func add_unit(unit) -> void:
	_units.append(unit)

func remove_unit(unit) -> void:
	var index = _units.find(unit)
	if index >= 0:
		_units.remove(index)

func count_units():
	return _units.size()

func _physics_process(delta):
	# TODO move somewhere better than physics process!
	if count_units() <= 0:
		print("Game over!")
