extends VBoxContainer
class_name JobWidget

export var key := "Q"
export var background_nodepath : NodePath
export var button_nodepath : NodePath
export var counter_nodepath : NodePath
export var state_name : String
export var texture_normal : Texture

var _counter : Label

func _ready():
	var button = get_node(button_nodepath) as TextureButton
	var conn = button.connect("pressed", self, "activate_job")
	assert(conn == OK)
	conn = GameManager.connect("player_mode_changed", self, "deactivate_job")
	assert(conn == OK)
	button.texture_normal = texture_normal
	$KeyLabel.text = key
	_counter = get_node(counter_nodepath) as Label
	conn = GameManager.connect("job_count_changed", self, "_on_job_count_changed")

func activate_job():
	get_node(background_nodepath).color = Color.white
	GameManager.set_player_mode(state_name)

func deactivate_job(new_state : String):
	if new_state != state_name:
		get_node(background_nodepath).color = Color("776663")
		
func _on_job_count_changed(job_changed):
	if state_name == job_changed:
		var count := 0
		match state_name:
			"Blocker":
				count = GameManager.blocker_count
			"Digger":
				count = GameManager.digger_count
			"LadderBuilder":
				count = GameManager.ladder_count
		_counter.text = String(count)
