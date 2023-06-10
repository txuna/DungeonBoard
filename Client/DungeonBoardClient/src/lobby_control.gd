extends Control

@onready var create_room_control = $WhiteBackgroundControl/CreateRoomControl
@onready var create_room_btn = $WhiteBackgroundControl/ButtonControl

# Called when the node enters the scene tree for the first time.
func _ready():
	create_room_control.visible = false
	create_room_btn.btn_pressed.connect(_open_create_room_control)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _open_create_room_control():
	create_room_control.visible = true
