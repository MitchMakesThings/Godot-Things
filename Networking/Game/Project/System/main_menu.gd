extends Control

@export
var map_scene : PackedScene

@export
var ip_input : LineEdit

@export
var port_input : LineEdit

func _ready():
	# Start the server if Godot is passed the "--server" argument.
	# This lets us host the server headless if we want :)
	if "--server" in OS.get_cmdline_args():
		_on_btn_host_pressed.call_deferred()

func _on_btn_host_pressed():
	NetworkState.start_network(true)
	load_scene()

func _on_btn_join_pressed():
	NetworkState.start_network(false, ip_input.text, port_input.text.to_int())
	load_scene()

func load_scene():
	get_tree().change_scene_to_packed(map_scene)
