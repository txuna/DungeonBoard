extends Control

@onready var name_label = $Label
@onready var texture = $TextureRect
@onready var hp = $ProgressBar

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func _set_boss(boss_id:int):
	name_label.text = Global.boss_string[boss_id].name 
	texture.texture = Global.boss_string[boss_id].image
	hp.max_value = Global.boss_string[boss_id].hp 
	hp.value = hp.max_value


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

