extends Node

@export
var PlayerScene = preload("res://Characters/Aliens/Character.tscn")

@export
var MobScene = preload("res://Characters/Mobs/mob.tscn")

# This should really be configured on the TestMap (and other maps!)
# That way individual maps could specify where mobs move to/from
# This script could then get those by querying a Map scripts mob_targets property.
# For now, I'm being lazy :)
@export
var mob_targets : Array[Vector2] = []

func _ready():
	if NetworkState.is_server:
		# Listen to peer connections, and create new character for them
		multiplayer.peer_connected.connect(self.create_player)
		# Listen to peer disconnections, and destroy their players
		multiplayer.peer_disconnected.connect(self.destroy_player)

func create_player(id : int) -> void:
	# Instantiate a new player for this client.
	var p = PlayerScene.instantiate()

	# Set the name, so players can figure out their local authority
	p.name = str(id)
	
	$Players.add_child(p)

func destroy_player(id : int) -> void:
	# Delete this peer's node.
	$Players.get_node(str(id)).queue_free()


# Called from the Timer node
func _on_mob_spawn_timeout():
	if not multiplayer.is_server():
		return
	# Limit the scene to 100 whales. 
	# This is to keep the screen from becoming too cluttered (and because we're not using pathfinding!)
	# Not a performance concern.
	if len(get_tree().get_nodes_in_group("mobs")) < 100:
		var mob = MobScene.instantiate()
		mob.targets = mob_targets
		get_node("Mobs").add_child(mob, true)
