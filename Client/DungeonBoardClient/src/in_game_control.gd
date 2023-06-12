extends Control

@onready var game_start_btn = $WhiteBackgroundControl/ButtonControl
@onready var player_container = $WhiteBackgroundControl/Players


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_request_load_room_info():
	pass
	

func _on_response_load_room_info(json):
	if json.result != Global.NONE_ERROR:
		return



# 게임시작 request 
func _on_button_control_btn_pressed():
	pass # Replace with function body.


func _on_request_game_start():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_response_game_start)
	http._request("Game/Start", true, {
		"RoomId" : Global.room_id
	})
	
	
func _on_response_game_start(json):
	if json.result != Global.NONE_ERROR:
		return 
		
	game_start_btn.visible = false
	

func _on_load_room_info_timer_timeout():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_load_room_info_timer_response)
	http._request("Room/Load", true, {
		"RoomId" : Global.room_id
	})
	

func _on_load_room_info_timer_response(json):
	print(json.result)
	if json.result != Global.NONE_ERROR:
		return 
	
	for node in player_container.get_children():
		node.queue_free()
		
	for player in json.player:
		var node = load("res://src/InGame/player_control.tscn").instantiate() 
		player_container.add_child(node)
		node._set_class_name(player.classId)
		node._set_user_id(player.userId)
		





