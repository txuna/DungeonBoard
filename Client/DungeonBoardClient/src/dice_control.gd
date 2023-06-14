extends Control

@onready var dice_label = $Panel/Label

signal get_dice(dice_number)

var dice_number = 1

var tween 

# Called when the node enters the scene tree for the first time.
func _ready():
	dice_label.text = "0"


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

# 주사위 굴리기
func _on_button_control_btn_pressed():
	if tween:
		tween.kill()
			
	var tween = create_tween().set_parallel(true)
	tween.finished.connect(_dice_finished)
	tween.tween_method(_dice_callback, 0, 10, 2)


func _dice_callback(value):
	var rng = RandomNumberGenerator.new()
	dice_number =  rng.randi_range(1, 6)
	dice_label.text = str(dice_number)
	
func _dice_finished():
	emit_signal("get_dice", dice_number)
