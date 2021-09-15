extends KinematicBody2D
class_name Slime

var _facing := Vector2.LEFT
var _interactable := false

export var Speed : float = 150
export var MaxSpeed : float = 100
export var SplatSpeed : float = 300

func _ready():
	GameManager.add_unit(self)
	GameManager.connect("game_started", self, "_on_game_started")
	
func _exit_tree():
	# Ensure we always get cleaned up!
	# States are expected to have already cleaned us up. 
	# But in case they don't, we'll die.
	GameManager.kill_unit(self)

func _on_game_started():
	_interactable = true

func _physics_process(delta : float):
	$StateMachine.process(delta)

func change_direction():
	_facing.x *= -1
	$AnimatedSprite.flip_h = !$AnimatedSprite.flip_h

func get_facing() -> Vector2:
	return _facing
	
func get_animated_sprite() -> AnimatedSprite:
	return $AnimatedSprite as AnimatedSprite

# Get the state machine for this slime
# Unfortunately this argument can't be typed, because then we ahve a cyclic 
# dependency between Slimes and State Machines. Godot typing is weird like that.
func get_state_machine():
	return $StateMachine

func _on_Slime_input_event(_viewport, event, _shape_idx):
	if !_interactable:
		return
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			$StateMachine.push_state(GameManager.get_player_mode())
