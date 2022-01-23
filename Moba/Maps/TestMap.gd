extends TileMap

const PlayerScene = preload("res://Characters/Aliens/Player.tscn")

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
		# Listen to peer connections, and create new player for them
		multiplayer.peer_connected.connect(self.create_player)
		# Listen to peer disconnections, and destroy their players
		multiplayer.peer_disconnected.connect(self.destroy_player)
		
		peer.create_server(4242)
		print('server listening on localhost 4242')
	else:
		peer.create_client("localhost", 4242)

	multiplayer.set_multiplayer_peer(peer)
	
	# Spawn test button
	var btn = preload("res://Items/blue_button.tscn").instantiate()
	btn.position = Vector2(386, 351)
	btn.name = str(randi())
	$NetworkedNodes.add_child(btn)

func create_player(id):
	# Instantiate a new player for this client.
	var p = PlayerScene.instantiate()

	# Set the name, so players can figure out their local authority
	p.name = str(id)
	
	print('added player: ', str(id), ' owned by: ', p.get_multiplayer_authority())
	$NetworkedNodes.add_child(p)

func destroy_player(id):
	# Delete this peer's node.
	$NetworkedNodes.get_node(str(id)).queue_free()
