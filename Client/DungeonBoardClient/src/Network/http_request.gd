extends HTTPRequest

signal _http_response(response_json)

const BASE_URL = "http://localhost:5111/"
const HEADERS = ["Content-Type: application/json"]
const POST = HTTPClient.METHOD_POST

# flag값에 따라 auth_token, userId 포함할것인지
func _request(path:String, flag:bool, request_json:Dictionary):	
	if flag:
		request_json["UserId"] = Global.user_id
		request_json["AuthToken"] = Global.auth_token
	
	var json = JSON.stringify(request_json)
	request(BASE_URL + path, HEADERS, POST, json)
	

func _on_request_completed(result, response_code, headers, body):
	if response_code != 200:
		print(body.get_string_from_utf8())
		return 
		
	var json = JSON.parse_string(body.get_string_from_utf8())
	emit_signal("_http_response", json)
	queue_free()
