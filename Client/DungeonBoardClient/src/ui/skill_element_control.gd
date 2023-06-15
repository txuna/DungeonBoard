extends Panel

@onready var skill_name = $Label
@onready var skill_effect = $ScrollContainer/Label2
@onready var skill_comment = $ScrollContainer2/Label3

signal skill_select(skill_id)

var _skill_id 

# Called when the node enters the scene tree for the first time.
func _ready():
	custom_minimum_size = Vector2(650, 100)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _set_skill(skill_id):
	_skill_id = skill_id
	var skill = Global.skill_string[int(skill_id)]
	
	skill_name.text = skill.name
	skill_effect.text = "({b}) + 공격력({a}) + 마법력({m}) + 방어력({d}) | Usage : MP({p})".format(
		{
			"b" : skill.base_value, 
			"a" : skill.attack, 
			"m" : skill.magic, 
			"d" : skill.defence,
			"p" : skill.mp
		}
	)
	skill_comment.text = skill.comment


func _on_button_control_btn_pressed():
	emit_signal("skill_select", _skill_id)
