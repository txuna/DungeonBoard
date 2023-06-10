extends Control

@onready var line_edit = $LineEdit

func _use_secret(flag):
	line_edit.secret = flag 
	

func _load_line_text():
	return line_edit.text
	
	
func _set_placeholder(content):
	line_edit.set_placeholder(content)


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
