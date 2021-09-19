extends State
class_name ActionState

export var texture_nodepath : NodePath
export var texture_progress : Texture

var _progress : TextureProgress
var _internal_counter : float = 0

func enter(extra_params := []) -> void:
	.enter(extra_params)
	_slime.get_animated_sprite().play("idle")
	$Offset/Panel.visible = true
	_progress = get_node(texture_nodepath) as TextureProgress
	_progress.texture_progress = texture_progress
	assert(_progress)
	_progress.visible = true
	
func exit() -> void:
	.exit()
	$Offset/Panel.visible = false
	_progress.visible = false

func process(delta : float) -> void:
	# Blockers don't do anything!
	# TODO - add timer and count-down until blocker state expires
	.process(delta)
	_internal_counter += delta * 50
	if _internal_counter >= 1:
		_internal_counter = 0
		_progress.set_value(_progress.value + _progress.step)
		if _progress.value >= 100:
			process_finished()

func process_finished():
	_state_machine.pop_state()
