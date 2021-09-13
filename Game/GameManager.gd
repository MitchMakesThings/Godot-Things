extends Node

var _units := []
var _score := 0
var _total_units := 0 # Total number of units that were added to the scene
var _acceptable_losses := 0

var _is_game_running := false
var _next_level : PackedScene

signal game_initialised # Called at the end of initialise() to let UI do it's initial state etc
signal game_started
signal game_ended # Signal to indicate the game ended. One parameter, true if you won.
signal unit_died
signal unit_escaped

# Clears and reconfigures the game state
func initialise(next_level, acceptable_losses) -> void:
	_next_level = next_level
	_acceptable_losses = acceptable_losses
	
	_score = 0
	_total_units = _units.size()
	_is_game_running = false
	emit_signal("game_initialised")

func add_unit(unit) -> void:
	_units.append(unit)
	_total_units += 1

func save_unit(unit) -> void:
	if _remove_unit(unit):
		_add_score()
		emit_signal("unit_escaped")
	_check_game_ended()

func kill_unit(unit) -> void:
	if _remove_unit(unit):
		_acceptable_losses -= 1
		emit_signal("unit_died")
		_check_game_ended()

func _check_game_ended() -> void:
	if _units.size() == 0:
		emit_signal("game_ended", true)
	if _acceptable_losses <= 0:
		emit_signal("game_ended", false)

func _remove_unit(unit) -> bool:
	var index = _units.find(unit)
	if index >= 0:
		_units.remove(index)
		return true
	return false
	

func count_units():
	return _units.size()

func _add_score() -> void:
	_score += 1

func get_score() -> int:
	return _score

func get_acceptable_losses() -> int:
	return _acceptable_losses

func go_to_next_level() -> void:
	# TODO scene change animation
	var changed = get_tree().change_scene_to(_next_level)
	assert(changed == OK)

func has_next_level() -> bool:
	return _next_level != null

# This needs to be called to start the game!
# Any initialisation stuff can start here
# Then we'll ask each unit to proceed to their next state
func start_game() -> void:
	emit_signal("game_started")
	_is_game_running = true

# TODO move this somewhere sensible!
func _process(_delta):
	if !_is_game_running and Input.is_action_just_pressed("ui_select"):
		start_game()
