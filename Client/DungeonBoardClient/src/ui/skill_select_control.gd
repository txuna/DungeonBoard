extends Panel

@onready var skill_container = $ScrollContainer/VBoxContainer
@onready var title = $Label

signal select_skill

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _load_skill(class_id):
	for node in skill_container.get_children():
		node.queue_free()
	
	var class_s = Global.class_string[int(class_id)]
	title.text = "스킬 선택({v})".format({"v" : class_s.name})
	
	for id in class_s.skillId:
		var node = load("res://src/ui/skill_element_control.tscn").instantiate()
		skill_container.add_child(node)
		node._set_skill(id)
		node.skill_select.connect(_select_skill)	


func _select_skill(skill_id):
	emit_signal("select_skill", skill_id)
