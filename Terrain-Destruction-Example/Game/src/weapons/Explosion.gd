extends Node2D


func _ready():
	$AnimatedSprite2D.play()


func _on_AnimatedSprite_animation_finished():
	queue_free()
