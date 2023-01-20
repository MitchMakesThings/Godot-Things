extends Node

var is_server := false

func start_network(server: bool, ip: String = '', port: int = 4242) -> void:
	is_server = server
	var peer = ENetMultiplayerPeer.new()
	if server:
		peer.create_server(4242)
		print('server listening on localhost 4242')
	else:
		show_message("Connecting...")
		peer.create_client(ip, port)
		multiplayer.connection_failed.connect(_on_connection_failed)
		multiplayer.connected_to_server.connect(_on_connection_success)

	multiplayer.set_multiplayer_peer(peer)

func show_message(message : String) -> void:
	$CanvasLayer/CenterContainer/Panel/Label.text = message
	$CanvasLayer/CenterContainer.visible = true

func hide_message() -> void:
	$CanvasLayer/CenterContainer.visible = false

func _on_connection_failed():
	show_message("Failed to connect :(")
	await get_tree().create_timer(2).timeout
	hide_message()
	get_tree().change_scene_to_file("res://System/main_menu.tscn")

func _on_connection_success():
	show_message("Connected!")
	await get_tree().create_timer(1).timeout
	hide_message()
