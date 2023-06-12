extends Control

@onready var create_room_control = $WhiteBackgroundControl/CreateRoomControl
@onready var create_room_btn = $WhiteBackgroundControl/ButtonControl

@onready var room_container = $WhiteBackgroundControl/Panel/ScrollContainer/VBoxContainer

# Called when the node enters the scene tree for the first time.
func _ready():
	create_room_control.visible = false
	create_room_btn.btn_pressed.connect(_open_create_room_control)
	_request_load_room()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _open_create_room_control():
	create_room_control.visible = true


func _request_load_room():	
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_response_load_room)
	http._request("Room/LoadAll", true, {})
	
	
func _response_load_room(json):
	if json.result != Global.NONE_ERROR:
		return 
	
	for node in room_container.get_children():
		node.queue_free()
		
	for room in json.redisRoom:
		var room_element = load("res://src/ui/RoomElement.tscn").instantiate() 
		room_container.add_child(room_element)
		room_element._set_label(
			room.roomId, 
			room.bossId, 
			room.title, 
			room.headCount,
			room.users.size(), 
			room.state
		)
		room_element.request_enter_room.connect(_on_request_enter_room)


func _on_button_control_2_btn_pressed():
	_request_load_room()


func _on_request_enter_room(room_id):
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_response_enter_room)
	http._request("Room/Enter", true, {
		"RoomId" : room_id
	})
	

# Scene 변경
func _on_response_enter_room(json):
	if json.result != Global.NONE_ERROR:
		return 	
	
	Global.room_id = json.roomId
	get_tree().change_scene_to_file("res://src/in_game_control.tscn")
	
	
