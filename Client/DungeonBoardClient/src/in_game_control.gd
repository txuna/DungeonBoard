extends Control

@onready var game_start_btn = $WhiteBackgroundControl/ButtonControl
@onready var player_container = $WhiteBackgroundControl/Players
@onready var host_label = $HostLabel
@onready var exit_btn = $WhiteBackgroundControl/ButtonControl2

@onready var card_control_containter = $WhiteBackgroundControl/Panel

@onready var load_game_info_timer = $LoadGameInfoTimer
@onready var load_room_info_timer = $LoadRoomInfoTimer

@onready var boss_control = $WhiteBackgroundControl/BossControl


# Called when the node enters the scene tree for the first time.
func _ready():
	_set_card()
	boss_control.visible = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _set_card():
	for control in card_control_containter.get_children():
		for card_node in control.get_children():
			card_node._set_card()

# 게임시작 request 
func _on_button_control_btn_pressed():
	_on_request_game_start()


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
	exit_btn.visible = false
	load_game_info_timer.start()
	boss_control.visible = true 
	#boss_control._set_boss()

#/Game/Info
# Player List, Player Icon등 생성
func _on_load_game_info_timer_timeout():
	pass # Replace with function body.
	
	
func _on_load_game_info_timer_response(json):
	if json.result != Global.NONE_ERROR:
		return 

	

func _on_load_room_info_timer_timeout():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_load_room_info_timer_response)
	http._request("Room/Load", true, {
		"RoomId" : Global.room_id
	})


func _on_load_room_info_timer_response(json):
	if json.result != Global.NONE_ERROR:
		return 
	
	for node in player_container.get_children():
		node.queue_free()
		
	if json.player == null:
		return
		
	if json.room.state == Global.RoomStateType.READY:
		exit_btn.visible = true 
		if json.room.hostUserId == Global.user_id:
			game_start_btn.visible = true 
			host_label.visible = true 
		else:
			game_start_btn.visible = false 
			host_label.visible = false
	else:
		exit_btn.visible = false
		game_start_btn.visible = false
		
	for player in json.player:
		var node = load("res://src/InGame/player_control.tscn").instantiate() 
		player_container.add_child(node)
		node._set_name(player.classId, player.userId)


func _on_button_control_2_btn_pressed():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_response_exit_room)
	http._request("Room/Exit", true, {
		"RoomId" : Global.room_id
	})

func _on_response_exit_room(json):
	if json.result != Global.NONE_ERROR:
		return 
	
	Global.room_id = 0
	get_tree().change_scene_to_file("res://src/lobby_control.tscn")


