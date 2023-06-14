extends Control

@onready var game_start_btn = $WhiteBackgroundControl/ButtonControl
@onready var player_container = $WhiteBackgroundControl/Players
@onready var host_label = $HostLabel
@onready var exit_btn = $WhiteBackgroundControl/ButtonControl2

@onready var card_control_containter = $WhiteBackgroundControl/Panel

@onready var load_game_info_timer = $LoadGameInfoTimer
@onready var load_room_info_timer = $LoadRoomInfoTimer

@onready var boss_control = $WhiteBackgroundControl/BossControl

@onready var dice_control = $WhiteBackgroundControl/DiceControl

@onready var player_icon_container = $WhiteBackgroundControl/PlayerIcon

var is_create_player_icon = false
var is_game_setup = false

# Called when the node enters the scene tree for the first time.
func _ready():
	_set_card()
	boss_control.visible = false
	dice_control.visible = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _set_card():
	for control in card_control_containter.get_children():
		for card_node in control.get_children():
			card_node._set_card()


func _get_card(n):
	for control in card_control_containter.get_children():
		for card_node in control.get_children():
			if card_node.card_number == n:
				return card_node._load_player_position()


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
		print(json.result)
		return 


#/Game/Info
# Player List, Player Icon등 생성
func _on_load_game_info_timer_timeout():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_load_game_info_timer_response)
	http._request("Game/Info", true, {
		"GameId" : Global.room_id
	})


# WhoIsTurn에 대해서 표시하기 ( DiceOpen 도 ) 
func _on_load_game_info_timer_response(json):
	if json.result != Global.NONE_ERROR:
		return 
	
	# player 프로및 및 각 카드 포지션에 플레이에 아이콘 생성 
	# player 생성도 처음만
	if not is_create_player_icon:
		create_player_icon(json.gameInfo.players)
	else:
		# 이동의 변화가 있었는지 확인
		move_player_icon(json.gameInfo.players)
			
	# 프로필 설정
	for player in json.gameInfo.players: 
		pass
	
	# boss 세팅 - Image는 처음만 설정	
	boss_control._update_boss(json.gameInfo.bossInfo)
	# WhoIsTurn을 보고 주사위 오픈
	if json.gameInfo.WhoisTurn == Global.user_id:
		dice_control.visible = true 
	else:
		dice_control.visible = false
	

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
	# 게임 시작 상태로 바뀐다면
	else:
		if not is_game_setup:
			exit_btn.visible = false
			game_start_btn.visible = false
			boss_control.visible = true
			dice_control.visible = true
			load_game_info_timer.start()
			is_game_setup = true
	
	for player in json.player:
		var node = load("res://src/InGame/player_control.tscn").instantiate() 
		player_container.add_child(node)
		node._set_name(player.classId, player.userId)


func create_player_icon(player_info):
	is_create_player_icon = true 
	for player in player_info:
		var node = load("res://src/player_icon.tscn").instantiate() 
		player_icon_container.add_child(node)
		node._init_player_icon(player.userId, player.positionCard, player.classId)
		node.global_position = _get_card(player.positionCard)


# 움직임의 변화 확인 
func move_player_icon(player_info):
	for player in player_info:
		var icon = get_player_icon(player.userId)
		if icon.get_card_number() == player.positionCard:
			print("[{u}] 변화 없음".format({"u" : player.userId}))
			continue 
		else:
			icon.global_position = _get_card(player.positionCard)
			icon.update_card_number(player.positionCard)
		
		
func get_player_icon(user_id):
	for player in player_icon_container.get_children():
		if player.user_id == user_id:
			return player


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


# 주사위 돌림 - 마이턴일 떄
func _on_dice_control_get_dice(dice_number):
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_response_exit_room)
	http._request("Game/Dice", true, {
		"DiceNumber" : dice_number
	})
	
	
func _on_dice_response(json):
	if json.result != Global.NONE_ERROR:
		return 
		
		
