extends Control

@onready var boss_list_btn = $Panel/OptionButton
@onready var headcount_list_btn = $Panel/OptionButton2

# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_texture_button_pressed():
	visible = false
	#queue_free()
