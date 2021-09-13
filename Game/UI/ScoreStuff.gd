extends MarginContainer

export var acceptable_losses_nodepath : NodePath
var _acceptable_losses_label : Label

func _ready():
	_acceptable_losses_label = get_node(acceptable_losses_nodepath)
	assert(_acceptable_losses_label)
	
	var conn = GameManager.connect("game_initialised", self, "_on_game_initialised")
	assert(conn == OK)
	
	conn = GameManager.connect("unit_died", self, "_on_unit_died")
	assert(conn == OK)

func _on_game_initialised():
	_acceptable_losses_label.text = String(GameManager.get_acceptable_losses())

func _on_unit_died():
	_acceptable_losses_label.text = String(GameManager.get_acceptable_losses())


