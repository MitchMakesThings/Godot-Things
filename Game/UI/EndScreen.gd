extends Control

export var score_label_nodepath : NodePath

func _ready():
	var connection = GameManager.connect("game_ended", self, "_on_game_finished")
	assert(connection == OK)
	
func _on_game_finished():
	# TODO generate some interesting text
	get_node(score_label_nodepath).text = String(GameManager.get_score()) + " Slime did their job"
	visible = true
	# TODO - add some sweet easing etc Bounce maybe?
	$AnimationPlayer.play("enter")
