extends Node

var _units := []
var _score := 0
var _total_units := 0 # Total number of units that were added to the scene

var _is_game_running := false

func add_unit(unit) -> void:
	_units.append(unit)
	_total_units += 1

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

# This needs to be called to start the game!
# Any initialisation stuff can start here
# Then we'll ask each unit to proceed to their next state
func start_game() -> void:
	for unit in _units:
		var slime = unit as Slime
		assert(slime)
		slime.get_state_machine().next_state()
	_is_game_running = true

# TODO move this somewhere sensible!
func _process(_delta):
	if !_is_game_running and Input.is_action_just_pressed("ui_select"):
		start_game()

func _physics_process(_delta):
	# TODO move somewhere better than physics process!
	if count_units() <= 0:
		_is_game_running = false
		print("Game over! Score: ", _score)
		# TODO emit signal etc
