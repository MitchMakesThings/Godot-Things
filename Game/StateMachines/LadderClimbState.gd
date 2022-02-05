extends State
class_name LadderClimbState

func enter(extra_params := []) -> void:
	.enter(extra_params)
	assert(_slime)
	
	# A single extra parameter can be passed, telling us where the slime should vanish from
	var top_position = _slime.position + Vector2(0, -50)
	if extra_params.size() == 1:
		top_position = extra_params[0]
	
	# Shift the target position across a little bit, so it'll actually make it off the ladder!
	# TODO we might have to delay this until the end, so the slimes go straight up the ladder instead of diagonally.
	# I'm hoping this won't be a problem with a skinnier ladder
	top_position += _slime.get_facing() * 32

	if !$Tween.is_connected("tween_all_completed", self, "_on_Tween_tween_all_completed"):
		var conn = $Tween.connect("tween_all_completed", self, "_on_Tween_tween_all_completed")
		assert(conn == OK)
	$Tween.interpolate_property(_slime, "position", _slime.position, top_position, 2, Tween.TRANS_QUART, Tween.EASE_OUT)
	$Tween.start()

func process(_delta : float) -> void:
	# Don't do the base state behaviour! You can't fall while climbing!
	pass

func _on_Tween_tween_all_completed():
	# TODO after finishing climbing all the way, step off in the direction of Slime.facing?
	_slime.get_state_machine().pop_state()
