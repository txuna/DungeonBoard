extends TextureRect

@onready var name_label = $Label

var current_card_number = 0
var user_id


# Called when the node enters the scene tree for the first time.
func _ready():
	pass
	


func _init_player_icon(_user_id, _current_card_number, class_id):
	user_id = _user_id 
	current_card_number = _current_card_number
	
	name_label.text = "({i}){c}".format({
		"i" : str(user_id),
		"c" : Global.class_string[int(class_id)].name
	})
	

func update_card_number(c):
	current_card_number = c


func get_card_number():
	return current_card_number


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
