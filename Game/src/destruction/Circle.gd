extends Node2D

# Called every frame
func _draw():
	var center = Vector2(200, 200)
	var radius = 80
	var color = Color.black
	
	draw_circle( center, radius, color )
