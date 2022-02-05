extends Control

export var blocker_widget_nodepath : NodePath
export var digger_widget_nodepath : NodePath
export var ladder_widget_nodepath : NodePath

var blocker_widget : JobWidget
var digger_widget : JobWidget
var ladder_widget : JobWidget

func _ready():
	blocker_widget = get_node(blocker_widget_nodepath) as JobWidget
	digger_widget = get_node(digger_widget_nodepath) as JobWidget
	ladder_widget = get_node(ladder_widget_nodepath) as JobWidget

func _process(_delta):
	if Input.is_action_just_pressed("job_1"):
		blocker_widget.activate_job()
	if Input.is_action_just_pressed("job_2"):
		digger_widget.activate_job()
	if Input.is_action_just_pressed("job_3"):
		ladder_widget.activate_job()
