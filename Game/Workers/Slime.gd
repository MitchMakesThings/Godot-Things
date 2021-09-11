extends KinematicBody2D
class_name Slime

var _facing := Vector2.LEFT
var _velocity := Vector2.ZERO
var _state := "default" # TODO make state machine
						# Valid values - default, blocker

export var Speed : float = 150
export var MaxSpeed : float = 100
export var SplatSpeed : float = 300

func _ready():
	$AnimatedSprite.play("walking")
	GameManager.add_unit(self)
	
func _exit_tree():
	GameManager.remove_unit(self)

func _physics_process(delta : float):
	if is_dying() or _state == "blocker":
		return
	if !is_on_floor():
		_velocity += Vector2.DOWN * delta * 300
	else:
		_velocity += _facing * delta * Speed
		_velocity.x = min(abs(_velocity.x), MaxSpeed) * _facing.x

	if test_move(self.transform, _facing):
		change_direction()
		return

	var fallingSpeed = _velocity.y
	_velocity = move_and_slide(_velocity, Vector2.UP)
	# If we were falling before move_and_slide, but now are on the floor, 
	# that means we just splatted to the ground.
	if fallingSpeed > 0 and is_on_floor():
		# If we splatted hard enough we die!
		if fallingSpeed > SplatSpeed:
			kill()
	# Check whether we collided with something on our insta-kill collision layer
	for i in get_slide_count():
		var collision = get_slide_collision(i)
		if collision.collider.collision_layer & 16 == 16:
			kill()

func change_direction():
	_facing.x *= -1
	$AnimatedSprite.flip_h = !$AnimatedSprite.flip_h

func _on_AnimatedSprite_animation_finished():
	if is_dying():
		queue_free()

func kill():
	$AnimatedSprite.play("death")

func is_dying():
	return $AnimatedSprite.animation == "death"


func _on_Slime_input_event(viewport, event, shape_idx):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			# TODO - refactor!
			# Make blocker. Note collision layer indexes are 0-based!
			set_collision_layer_bit(1, true)
			_state = "blocker"
