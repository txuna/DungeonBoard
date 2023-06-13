extends Node

var auth_token = ""
var user_id = 0
var room_id = 0

const NONE_ERROR = 0

const error_code_msg = {
	
}

const MAX_HEADCOUNT = 4

enum CardType {
	SkillCard = 0, 
	LevelupCard = 1, 
	ShopCard = 2, 
	SpecialCard = 3
}

const card_string = {
	CardType.SkillCard : {
		"name" : "스킬", 
		"color" : Color.DARK_CYAN
	},
	CardType.LevelupCard : {
		"name" : "레벨업",
		"color" : Color.DARK_GREEN
	},
	CardType.ShopCard : {
		"name" : "상점",
		"color" : Color.BLUE_VIOLET
	},
	CardType.SpecialCard : {
		"name" : "특수카드", 
		"color" : Color.DARK_ORANGE
	}
}

enum ClassType{
	NONE_CLASS = 0, 
	WARRIOR = 1, 
	WIZARD = 2, 
	ARCHER = 3, 
}

enum RoomStateType{
	READY = 0, 
	PLAYING = 1
}

# 직업 
const class_string = {
	ClassType.NONE_CLASS : "초보자",
	ClassType.WARRIOR : "전사",
	ClassType.WIZARD : "마법사",
	ClassType.ARCHER : "궁수"
}

# 아이템 
const item_string = {
	
}

# 방 상태 
const room_state_string = {
	RoomStateType.READY : "준비",
	RoomStateType.PLAYING : "게임중"
}

# 보스 
const boss_string = {
	1 : {
		"name" : "보스1"
	},
	2 : {
		"name" : "보스2"
	}
}

const skill_string = {
	
}

