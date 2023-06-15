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
@onready var levelup_marker = $WhiteBackgroundControl/LevelupMarker
@onready var skill_select_control = $SkillSelectControl

var is_create_player_icon = false
var is_game_setup = false
var is_simulate = false 

var tween 

# 플레이어들의 행동 저장 - 행동을 레디스에서 받아오는 과정에서 한번만 처리하기 위해
# ex) 플레이어가 주사위를 던졌을 때 행동인덱스 : 1, 행위(주사위, 1) 하면 최근 스택에서 1번 행동이 있었는지 확인하고 없다면 
# 행위 보여줌 ( 주사위 1만큼 나온것을 보여줌 ) 
# 상대의 시뮬레이션 중에는 주사위 굴리기 금지
var act_stack = []

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
				return card_node


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
			
	# 프로필 설정 - WhoIsTurn의 UserId와 같다면 세팅
	# Room이랑 겹칠 수 도 있음
	for node in player_container.get_children():
		node.queue_free()
			
	for player in json.gameInfo.players: 
		var node = load("res://src/InGame/player_control.tscn").instantiate() 
		player_container.add_child(node)
		node._set_name(player.classId, player.userId)
		node._set_hp(player.hp, player.maxHp)
		node._set_mp(player.mp, player.maxMp)
		node._set_stat(player.attack, player.defence, player.magic, player.level)
		Global.player_stat = player
	
	# boss 세팅 - Image는 처음만 설정	
	boss_control._update_boss(json.gameInfo.bossInfo)
	# WhoIsTurn을 보고 주사위 오픈
	if json.gameInfo.whoIsTurn.userId == Global.user_id:
		if is_simulate:
			return 
		dice_control._set_disabled_button(false)
	else:
		dice_control._set_disabled_button(true)
	

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
		
	if json.player == null:
		return
		
	if json.room.state == Global.RoomStateType.READY:
		for node in player_container.get_children():
			node.queue_free()
		
		exit_btn.visible = true 
		
		if json.room.hostUserId == Global.user_id:
			game_start_btn.visible = true 
			host_label.visible = true 
		else:
			game_start_btn.visible = false 
			host_label.visible = false
		
		# Ready일 때 세팅하고 Game 진행되면 다시 생성
		for player in json.player:
			var node = load("res://src/InGame/player_control.tscn").instantiate() 
			player_container.add_child(node)
			node._set_name(player.classId, player.userId)
	# 게임 시작 상태로 바뀐다면
	else:
		if not is_game_setup:
			exit_btn.visible = false
			game_start_btn.visible = false
			boss_control.visible = true
			dice_control.visible = true
			load_game_info_timer.start()
			is_game_setup = true


func create_player_icon(player_info):
	is_create_player_icon = true 
	for player in player_info:
		var node = load("res://src/player_icon.tscn").instantiate() 
		player_icon_container.add_child(node)
		node._init_player_icon(player.userId, player.positionCard, player.classId)
		node.global_position = _get_card(player.positionCard)._load_player_position()


# 움직임의 변화 확인 
func move_player_icon(player_info):
	for player in player_info:
		var icon = get_player_icon(player.userId)
		if icon.get_card_number() == player.positionCard:
			continue 
		else:
			# 상대가 얼마큼 움직였는지 표시하기 - log_container 
			if tween:
				tween.kill() 
			
			var card_node = _get_card(player.positionCard)
			tween = create_tween().set_parallel(true)
			tween.tween_property(icon, "global_position", card_node._load_player_position(), 0.5)
			tween.finished.connect(_finised_player_icon_move.bind(player.userId, card_node._load_card_type()))
			icon.update_card_number(player.positionCard)
		
			
func get_player_icon(user_id):
	for player in player_icon_container.get_children():
		if player.user_id == user_id:
			return player


func _animation_finished():
	pass

# 움직인 후 도착 칸에 따른 카드 request
# 여기서 서버 응답 전에 시뮬레이션할거 진행 -> 스킬샷 or 레벨업
func _finised_player_icon_move(tween_user_id, tween_card_type):	
	# request - response 응답이 와야 is_simulate = false  
	if tween_card_type == Global.CardType.LevelupCard:
		# 해당 유저 위치에서 레벨업 에니메이션 플레이
		# CODE
		var node = load("res://src/ui/levelup_label.tscn").instantiate() 
		add_child(node)
		node.global_position = levelup_marker.global_position
		node.animation_fin.connect(_animation_finished)
		node._play()
		
		if tween_user_id != Global.user_id:
			is_simulate = false 
			return 
		
		_request_levelup_card()
	
	elif tween_card_type == Global.CardType.SkillCard:
		# 해당 유저 위치에서 스킬 샷 에니메이션 플레이
		# CODE
		if tween_user_id != Global.user_id:
			is_simulate = false 
			return 
			
		open_skill_card()
	
'''	
	elif tween_card_type == Global.CardType.SpecialCard:
		if tween_user_id != Global.user_id:
			is_simulate = false 
			return 
			
		_request_skill_card()
'''


func _request_levelup_card():
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_response_levelup_card)
	http._request("Game/Levelup", true, {
		"GameId" : Global.room_id
	})

# 레벨업 시뮬레이션 진행하고 is_simulate = false 
func _response_levelup_card(json):
	is_simulate = false
	
	if json.result != Global.NONE_ERROR:
		return 
	
	
func _request_skill_card(skill_id):
	print(skill_id)
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_response_levelup_card)
	http._request("Game/Skill", true, {
		"GameId" : Global.room_id,
		"SkillId" : skill_id
	})
	

func _response_skill_card(json):
	is_simulate = false
	
	if json.result != Global.NONE_ERROR:
		return 
	
	
	
func open_skill_card():
	skill_select_control.visible = true 
	skill_select_control._load_skill(Global.class_id)
	
	
func _on_skill_select_control_select_skill(skill_id):
	skill_select_control.visible = false
	_request_skill_card(skill_id)


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


func _on_dice_control_pressed_dice():
	is_simulate = true
	dice_control._set_disabled_button(true)


# 주사위 돌림 - 마이턴일 떄
func _on_dice_control_get_dice(dice_number):
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_dice_response)
	http._request("Game/Dice", true, {
		"GameId" : Global.room_id,
		"DiceNumber" : dice_number
	})
	
	
func _on_dice_response(json):
	if json.result != Global.NONE_ERROR:
		return 
		

