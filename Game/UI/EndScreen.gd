extends Control

export var score_label_nodepath : NodePath
export var next_level_button_nodepath : NodePath
export var retry_level_button_nodepath : NodePath
export var end_game_label_nodepath : NodePath
export var main_message_nodepath : NodePath

func _ready():
	var connection = GameManager.connect("game_ended", self, "_on_game_finished")
	assert(connection == OK)
	assert(score_label_nodepath)
	assert(next_level_button_nodepath)
	assert(retry_level_button_nodepath)
	assert(end_game_label_nodepath)
	assert(main_message_nodepath)
	
func _on_game_finished(player_won : bool):
	# TODOD change colour depending on whether the player won
	if player_won:
		$ColorRect.color = Color("49b47e")
		get_node(main_message_nodepath).text = "You win!"
	else:
		$ColorRect.color = Color("dd4e54")
		get_node(main_message_nodepath).text = "Good try!"
		get_node(next_level_button_nodepath).visible = false
	# TODO generate some interesting text
	get_node(score_label_nodepath).text = String(GameManager.get_score()) + " Slimes did their job"
	
	visible = true
	$Tween.interpolate_property(self, "rect_position", rect_position, Vector2(0,0), 0.6, Tween.TRANS_BOUNCE, Tween.EASE_OUT)
	$Tween.start()
	
	if !GameManager.has_next_level():
		get_node(next_level_button_nodepath).visible = false
		get_node(end_game_label_nodepath).visible = true


func _on_BtnNextLevel_pressed():
	GameManager.go_to_next_level()


func _on_BtnRetry_pressed():
	GameManager.restart_level()
