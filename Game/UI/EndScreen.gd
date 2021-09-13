extends Control

export var score_label_nodepath : NodePath
export var next_level_button_nodepath : NodePath
export var end_game_label_nodepath : NodePath

func _ready():
	var connection = GameManager.connect("game_ended", self, "_on_game_finished")
	assert(connection == OK)
	assert(score_label_nodepath)
	assert(next_level_button_nodepath)
	assert(end_game_label_nodepath)
	
func _on_game_finished(_player_won : bool):
	# TODOD change colour depending on whether the player won
	# TODO generate some interesting text
	get_node(score_label_nodepath).text = String(GameManager.get_score()) + " Slimes did their job"
	visible = true
	# TODO - add some sweet easing etc Bounce maybe?
	$AnimationPlayer.play("enter")
	
	if !GameManager.has_next_level():
		get_node(next_level_button_nodepath).visible = false
		get_node(end_game_label_nodepath).visible = true


func _on_BtnNextLevel_pressed():
	GameManager.go_to_next_level()
