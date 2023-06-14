extends Control

@onready var name_label = $Label
@onready var texture = $TextureRect
@onready var hp = $ProgressBar

@onready var attack_label = $Label2 
@onready var magic_label = $Label3
@onready var defence_label = $Label4

var is_complete = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func _update_boss(boss_info:Dictionary):
	# 처음 세팅
	if is_complete == false:
		name_label.text = Global.boss_string[boss_info.bossId].name 
		hp.max_value = Global.boss_string[boss_info.bossId].hp
		hp.value = hp.max_value
		texture.texture = Global.boss_string[boss_info.bossId].image
		
	hp.value = boss_info.hp 
	attack_label.text = "공격력 : {v}".format({
		"v" : boss_info.attack
	})
	magic_label.text = "마법력 : {v}".format({
		"v" : boss_info.magic
	})
	defence_label.text = "방어력 : {v}".format({
		"v" : boss_info.defence
	})
	
	is_complete = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

