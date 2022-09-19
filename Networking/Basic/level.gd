extends Node

const PlayerScene = preload("res://character.tscn")

func _ready():
	# Start the server if Godot is passed the "--server" argument,
	# and start a client otherwise.
	if "--server" in OS.get_cmdline_args():
		start_network(true)
	else:
		start_network(false)

func start_network(server: bool):
	var peer = ENetMultiplayerPeer.new()
	if server:
		multiplayer.peer_connected.connect(self.create_player)
		multiplayer.peer_disconnected.connect(self.destroy_player)
		
		peer.create_server(4242)
		print('server listening on localhost 4242')
	else:
		var targetIP = "localhost"
		peer.create_client(targetIP, 4242)

	multiplayer.multiplayer_peer = peer

func create_player(id):
	# Instantiate a new player for this client.
	var p = PlayerScene.instantiate()

	# Set the name, so players can figure out their local authority
	p.name = str(id)
	
	$NetworkedNodes.add_child(p)

func destroy_player(id):
	# Delete this peer's node.
	$NetworkedNodes.get_node(str(id)).queue_free()
