extends Control

@onready var game_start_btn = $WhiteBackgroundControl/ButtonControl

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_request_load_room_info():
	pass
	

func _on_response_load_room_info(json):
	if json.result != Global.NONE_ERROR:
		return



# 게임시작 request 
func _on_button_control_btn_pressed():
	pass # Replace with function body.


func _on_request_game_start():
	pass
	
	
func _on_response_game_start(json):
	if json.result != Global.NONE_ERROR:
		return 
		
	game_start_btn.visible = false
	

