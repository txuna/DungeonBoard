extends Button

signal btn_pressed

func _set_text(_text):
	text = _text 

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_pressed():
	emit_signal("btn_pressed")
