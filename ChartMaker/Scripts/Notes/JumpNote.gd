tool
extends Note
# Declare member variables here. Examples:
# var a = 2
# var b = "text"


func note_info_to_string(width:float,pixelPerSec:float) -> String:
	return "J,"+String(-position.y/pixelPerSec)
