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

enum SkillType{
	Fixed = 0, 
	Physic = 1,
	Heal = 2
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
	ClassType.NONE_CLASS : {
		"name" : "초보자",
		"skillId" : []
	},
	ClassType.WARRIOR : {
		"name" : "전사",
		"skillId" : [3]
	},
	ClassType.WIZARD : {
		"name" : "마법사",
		"skillId" : [4]
	},
	ClassType.ARCHER : {
		"name" : "궁수",
		"skillId" : [5]
	},
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
var boss_string = {
	1 : {
		"name" : "보스1",
		"hp" : 5000, 
		"attack" : 35, 
		"magic" : 35, 
		"defence" : 100, 
		"skillSet1" : 1, 
		"skillSet2" : 2,
		"image" : load("res://assets/abor.png")
	},
	2 : {
		"name" : "보스2",
		"hp" : 10000, 
		"attack" : 50, 
		"magic" : 50, 
		"defence" : 200, 
		"skill_set1" : 1, 
		"skill_set2" : 2,
		"image" : load("res://assets/mushroom_fill.png")
	}
}

var skill_string = {
	1 : {
		"name" : "화염1", 
		"type" : SkillType.Physic, 
		"base_value" : 30, 
		"attack" : 10,
		"magic" : 10, 
		"defence" : 0,
		"comment" : "(보스)(물리피해)모든 적을 불태운다."
	},
	2 : {
		"name" : "화염2", 
		"type" : SkillType.Fixed, 
		"base_value" : 50, 
		"attack" : 10,
		"magic" : 10, 
		"defence" : 0,
		"comment" : "(보스)(고정피해)모든 적을 강하게 불태운다."
	},
	3 : {
		"name" : "돌진1", 
		"type" : SkillType.Physic, 
		"base_value" : 25, 
		"attack" : 30,
		"magic" : 0, 
		"defence" : 50,
		"comment" : "(물리피해)적으로 돌진한다."
	},
	4 : {
		"name" : "회복1", 
		"type" : SkillType.Heal, 
		"base_value" : 15, 
		"attack" : 0,
		"magic" : 20, 
		"defence" : 0,
		"comment" : "(힐)팀원 모두를 일정수치만큼 회복시킨다."
	},
	5 : {
		"name" : "더블샷", 
		"type" : SkillType.Physic, 
		"base_value" : 35, 
		"attack" : 50,
		"magic" : 0, 
		"defence" : 0,
		"comment" : "(물리피해)적에게 2개의 화살을 발사한다."
	}
}


