extends Control

@onready var anime = $AnimationPlayer

signal animation_fin

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_animation_player_animation_finished(anim_name):
	emit_signal("animation_fin")
	queue_free()

func _play():
	anime.play("levelup")
	
