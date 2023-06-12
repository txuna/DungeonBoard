extends Control

@onready var boss_list_btn = $Panel/OptionButton
@onready var headcount_list_btn = $Panel/OptionButton2

@onready var room_title_field = $Panel/LineEdit

# Called when the node enters the scene tree for the first time.
func _ready():
	room_title_field._set_placeholder("방 제목...")
	
	for i in range(Global.MAX_HEADCOUNT):
		headcount_list_btn.add_item("{i}명".format({"i" : i+1}), i+1)

	for i in Global.boss_string:
		boss_list_btn.add_item(Global.boss_string[i].name ,i)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_texture_button_pressed():
	visible = false
	
	
func _request_create_room():	
	var room_title_text = room_title_field._load_line_text()
	if room_title_text == "":
		var alert = load("res://src/ui/alert_control.tscn").instantiate()
		add_child(alert)
		alert._set_text("방 제목을 입력해주세요")
		return 
	
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_response_create_room)
	http._request("Room/Create", true, {
		"Title" : room_title_text,
		"HeadCount" : headcount_list_btn.get_selected_id(),
		"BossId" : boss_list_btn.get_selected_id()
	})
	
	
func _response_create_room(json):
	if json.result != Global.NONE_ERROR:
		return 
		
	# change to file -> Game SCENE
	visible = false
	Global.room_id = json.roomId
	get_tree().change_scene_to_file("res://src/in_game_control.tscn")
	

func _on_button_control_btn_pressed():
	_request_create_room()
