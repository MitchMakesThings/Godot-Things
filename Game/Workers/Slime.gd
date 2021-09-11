extends KinematicBody2D
class_name Slime

var _facing := Vector2.LEFT
var _state := "default" # TODO make state machine
						# Valid values - default, blocker

export var Speed : float = 150
export var MaxSpeed : float = 100
export var SplatSpeed : float = 300

func _ready():
	GameManager.add_unit(self)
	
func _exit_tree():
	GameManager.remove_unit(self)

func _physics_process(delta : float):
	$StateMachine.process(delta)

func change_direction():
	_facing.x *= -1
	$AnimatedSprite.flip_h = !$AnimatedSprite.flip_h

func get_facing() -> Vector2:
	return _facing

func _on_Slime_input_event(viewport, event, shape_idx):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			# TODO - refactor!
			# Make blocker. Note collision layer indexes are 0-based!
			$StateMachine.push_state("Blocker")
