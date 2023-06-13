extends Panel

@onready var name_label = $Label
@onready var card_panel = $Panel

@export var card_number = 0
@export var card_type = Global.CardType.SkillCard

# 스킬, 레벨업, 상점칸마다 색갈 다르게
func _set_card():
	name_label.text = Global.card_string[card_type].name
	var cstylebox = StyleBoxFlat.new() 
	cstylebox.bg_color = Global.card_string[card_type].color
	card_panel.add_theme_stylebox_override("panel", cstylebox)
	
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
