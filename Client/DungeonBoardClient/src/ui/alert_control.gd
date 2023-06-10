extends ColorRect

@onready var content_field = $Label2

func _set_text(_text):
	content_field.text = _text

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_button_control_btn_pressed():
	queue_free()
