extends Control

func _on_Blocker_button_down():
	GameManager.set_player_mode("Blocker")


func _on_Digger_button_down():
	GameManager.set_player_mode("Digger")
