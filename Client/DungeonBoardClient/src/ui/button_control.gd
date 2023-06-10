extends Control

@onready var button = $Button

signal btn_pressed

func _set_text(text):
	button.text = text 

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_button_pressed():
	emit_signal("btn_pressed")
