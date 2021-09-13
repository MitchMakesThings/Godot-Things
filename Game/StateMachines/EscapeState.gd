extends State
class_name EscapeState

func enter(extra_params := []) -> void:
	.enter(extra_params)
	assert(_slime)
	
	# A single extra parameter can be passed, telling us where the slime should vanish from
	var exit_position = _slime.position + Vector2(0, -50)
	if extra_params.size() == 1:
		exit_position = extra_params[0]

	$Tween.interpolate_property(_slime, "position", _slime.position, exit_position, 2, Tween.TRANS_BACK, Tween.EASE_OUT)
	$Tween.interpolate_property(_slime, "scale", _slime.scale, Vector2.ZERO, 1, Tween.TRANS_CUBIC, Tween.EASE_IN)
	$Tween.start()

func process(_delta : float) -> void:
	# Don't do the base state behaviour! You can't fall or die while escaping :)
	pass

func _on_Tween_tween_all_completed():
	GameManager.save_unit(_slime)
	_slime.queue_free()
