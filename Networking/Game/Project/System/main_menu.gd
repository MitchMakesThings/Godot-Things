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
	load_scene()
	# Start the networking in server mode
	get_node("/root/MainGame").start_network(true)
	queue_free()

func _on_btn_join_pressed():
	load_scene()
	get_node("/root/MainGame").start_network(false, ip_input.text, port_input.text.to_int())
	queue_free()

func load_scene():
	var map = map_scene.instantiate()
	add_sibling(map)
	get_tree().current_scene = get_node("/root/MainGame")
