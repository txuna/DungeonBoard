extends Control

@onready var email_field = $WhiteBackgroundControl/Control
@onready var password_field = $WhiteBackgroundControl/Control2
@onready var confirm_password_field = $WhiteBackgroundControl/Control3
@onready var register_btn = $WhiteBackgroundControl/ButtonControl

# Called when the node enters the scene tree for the first time.
func _ready():
	email_field._set_placeholder("Email")
	password_field._set_placeholder("Password")
	password_field._use_secret(true)
	confirm_password_field._set_placeholder("Confirm Password")
	confirm_password_field._use_secret(true)
	register_btn._set_text("회원가입")
	register_btn.btn_pressed.connect(_register)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _register():
	var email_text = email_field._load_line_text() 
	var password_text = password_field._load_line_text() 
	var confirm_password_text = confirm_password_field._load_line_text() 
	
	if email_text == "" or password_text == "" or confirm_password_text == "":
		var alert = load("res://src/ui/alert_control.tscn").instantiate()
		add_child(alert)
		alert._set_text("이메일 또는 패스워드의 값이 비었습니다.")
		return 
	
	if password_text != confirm_password_text:
		var alert = load("res://src/ui/alert_control.tscn").instantiate()
		add_child(alert)
		alert._set_text("패스워드가 일치하기 않습니다.")
		return 
		return 
		
	print("RE")


func _on_button_pressed():
	get_tree().change_scene_to_file("res://src/login_control.tscn")
