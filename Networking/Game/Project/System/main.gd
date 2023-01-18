extends Node

@export
var PlayerScene = preload("res://Characters/Aliens/Character.tscn")

@export
var MobScene = preload("res://Characters/Mobs/mob.tscn")

func start_network(server: bool, ip: String = '', port: int = 4242) -> void:
	var peer = ENetMultiplayerPeer.new()
	if server:
		# Listen to peer connections, and create new character for them
		multiplayer.peer_connected.connect(self.create_player)
		# Listen to peer disconnections, and destroy their players
		multiplayer.peer_disconnected.connect(self.destroy_player)
		
		peer.create_server(4242)
		print('server listening on localhost 4242')
	else:
		peer.create_client(ip, port)

	multiplayer.set_multiplayer_peer(peer)

func create_player(id : int) -> void:
	# Instantiate a new player for this client.
	var p = PlayerScene.instantiate()

	# Set the name, so players can figure out their local authority
	p.name = str(id)
	
	$Players.add_child(p)

func destroy_player(id : int) -> void:
	# Delete this peer's node.
	$Players.get_node(str(id)).queue_free()


func _on_mob_spawn_timeout():
	if not multiplayer.is_server():
		return
	var mob = MobScene.instantiate()
	get_node("Mobs").add_child(mob, true)
