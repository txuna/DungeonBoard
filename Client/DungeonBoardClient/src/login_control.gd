extends Control

@onready var email_field = $WhiteBackgroundControl/EmailControl
@onready var password_field = $WhiteBackgroundControl/PasswordControl
@onready var login_btn = $WhiteBackgroundControl/ButtonControl
@onready var go_register_btn = $WhiteBackgroundControl/GoRegister

# Called when the node enters the scene tree for the first time.
func _ready():
	email_field._set_placeholder("Email")
	password_field._set_placeholder("Password")
	password_field._use_secret(true)
	login_btn._set_text("로그인")
	login_btn.btn_pressed.connect(_login)
	go_register_btn.pressed.connect(change_to_register)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _login():
	var email_text = email_field._load_line_text()
	var password_text = password_field._load_line_text() 
	
	if email_text == "" or password_text == "":
		var alert = load("res://src/ui/alert_control.tscn").instantiate()
		add_child(alert)
		alert._set_text("이메일 또는 패스워드의 값이 비었습니다.")
		return 
	
	var http = load("res://src/Network/http_request.tscn").instantiate()
	add_child(http)
	http._http_response.connect(_on_response)
	http._request("Login", false, {
		"Email" : email_text, 
		"Password" : password_text
	})
	
	
func _on_response(json):
	if json.result != Global.NONE_ERROR:
		return 
		
	print(json.userId)
	print(json.authToken)
	print(json.classId)
	Global.user_id = json.userId 
	Global.auth_token = json.authToken
	
	# 직업 선택 화면으로 
	if json.classId == Global.ClassType.NONE_CLASS:
		pass
	
	# 로비화면으로
	else:
		pass

	
func change_to_register():
	get_tree().change_scene_to_file("res://src/register_control.tscn")












