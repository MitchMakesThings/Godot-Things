extends TileMap

const PlayerScene = preload("res://Characters/Aliens/Player.tscn")

func _ready():
	# Get the UID of the scene we want replicated.
	#var id = ResourceLoader.get_resource_uid(PlayerScene.resource_path)
	# Configure the scene to be controlled by the server,
	# and which properties will be replicated during spawn.
	#multiplayer.replicator.spawn_config(id, MultiplayerReplicator.REPLICATION_MODE_SERVER,
	#[&"player_name", &"position"])
	# Configure the variables to be synchronized periodically
	# (every 16 milliseconds = 62.5 Hz).
	#multiplayer.replicator.sync_config(id, 1600, [&"position"])

	# Start the server if Godot is passed the "--server" argument,
	# and start a client otherwise.
	
	$MultiplayerSpawner.spawned.connect(self.spawned)
	
	if "--server" in OS.get_cmdline_args():
		start_network(true)
	else:
		start_network(false)

func start_network(server: bool):
	var peer = ENetMultiplayerPeer.new()
	if server:
		# Listen to peer connections, and create new player for them
		#multiplayer.peer_connected.connect(self.create_player)
		# Listen to peer disconnections, and destroy their players
		#multiplayer.peer_disconnected.connect(self.destroy_player)
		
		multiplayer.peer_connected.connect(self.create_player)
		peer.create_server(4242)
		print('server listening on localhost 4242')
	else:
		peer.create_client("localhost", 4242)

	multiplayer.set_multiplayer_peer(peer)

func create_player(id):
	# Instantiate a new player for this client.
	var p = PlayerScene.instantiate()
	# Sets the player name (only sent during spawn).
	p.player_name = "Player %d" % id
	# Set a random position (sent on every replicator update).
	p.position = Vector2(randi() % 500, randi() % 500)
	# Add it to the "Players" node.
	# We give the new Node a name for easy retrieval, but that's not necessary.
	p.name = str(id)
	p.set_multiplayer_authority(id)
	
	print('added player: ', str(id), ' owned by: ', p.get_multiplayer_authority())
	$Players.add_child(p)
	

func destroy_player(id):
	# Delete this peer's node.
	$Players.get_node(str(id)).queue_free()
	
func spawned(scene_id: int, node: Node):
	print('spawned ', scene_id, ' of type ', node)
