extends Control

func _request_load_room(class_id):	
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_response_load_room)
	http._request("Class/Select", true, {
		"ClassId" : class_id
	})
	
	
func _response_load_room(json):
	if json.result != Global.NONE_ERROR:
		return 
	
	get_tree().change_scene_to_file("res://src/lobby_control.tscn")


func _on_select_warrior_pressed():
	_request_load_room(1)


func _on_select_wizard_btn_pressed():
	_request_load_room(2)


func _on_select_archer_btn_pressed():
	_request_load_room(3)
