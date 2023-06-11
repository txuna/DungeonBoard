extends Panel

@onready var enter_room_btn = $Button
@onready var room_id_label = $Label
@onready var room_title_label = $ScrollContainer/Label2
@onready var room_headcount_label = $Label3
@onready var room_state_label = $Label4
@onready var room_boss_label = $Label5

signal request_enter_room(_room_id)

var _room_id = 0

func _set_label(room_id:int, boss_id:int, room_title:String, room_headcount:int, room_current_person:int, room_state:int):
	room_id_label.text = "[{room_id}]".format({
		"room_id" : str(room_id)
	})
	room_title_label.text = room_title
	room_boss_label.text = Global.boss_string[boss_id]
	room_headcount_label.text = "[{c}/{m}]".format({
		"c" : str(room_current_person),
		"m" : str(room_headcount)
	})
	room_state_label.text = Global.room_state_string[room_state]
	_room_id = room_id
	custom_minimum_size = Vector2(650, 80)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_button_pressed():
	emit_signal("request_enter_room", _room_id)
