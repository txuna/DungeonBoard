extends Panel

@onready var class_name_label = $ClassPanel/Label
@onready var attack_label = $ClassPanel/Label2
@onready var defence_label = $ClassPanel/Label3
@onready var magic_label = $ClassPanel/Label4
@onready var level_label = $ClassPanel/Label5

@onready var hp_bar = $HpBar
@onready var mp_bar = $MpBar
@onready var hp_label = $HpLabel
@onready var mp_label = $MpLabel

@onready var item_container = $Control

var user_id = 0
var class_id = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	custom_minimum_size = Vector2(370, 170)
	_set_hp(0, 100)
	_set_mp(0, 100)
	_set_name(0, 0)
	_set_stat(0, 0, 0, 1)
	#_set_items()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

# 공격력 얻어옴 
func _request_load_player_info(user_id:int):
	pass
	

func _response_load_player_info(json):
	if json.result != Global.NONE_ERROR:
		return 


func _set_name(_class_id:int, _user_id:int):
	user_id = _user_id
	class_id = _class_id
	class_name_label.text = "({n}){c}".format({
		"n" : str(user_id), 
		"c" : Global.class_string[class_id].name
	})


func _set_hp(value, max):
	hp_bar.max_value = max 
	hp_bar.value = value
	hp_label.text = "[{v} / {m}]".format({
		"v" : str(value), 
		"m" : str(max)
	})
	

func _set_mp(value, max):
	mp_bar.max_value = max 
	mp_bar.value = value
	mp_label.text = "[{v} / {m}]".format({
		"v" : str(value), 
		"m" : str(max)
	})


func _set_stat(atk, def, mag, level):
	attack_label.text = "공격력 : {v}".format({
		"v" : atk
	})
	defence_label.text = "방어력 : {v}".format({
		"v" : def
	})
	magic_label.text = "마법력 : {v}".format({
		"v" : mag
	})
	level_label.text = "레벨 : {v}".format({
		"v" : level
	})


func _set_items():
	for panel_node in item_container.get_children():
		var texture_node = panel_node.get_node_or_null("TextureRect")
		if texture_node == null:
			continue 

