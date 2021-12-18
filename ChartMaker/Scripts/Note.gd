tool
extends Node2D
class_name Note

enum Direction{Left,Right}

export(float) var beat:float setget set_beat,get_beat
func set_beat(value:float):
	if get_parent() != null&&get_parent().name == "NoteMaster":
		position.y = - value / get_parent().bpm * 60 * get_parent().pixelPerSec 
	pass
func get_beat():
	if get_parent() != null&&get_parent().name == "NoteMaster":
		return -position.y / get_parent().pixelPerSec / 60 * get_parent().bpm
	pass
	
export(float) var scale_:float setget set_scale_,get_scale_
func set_scale_(value:float):
	scale.x = value
func get_scale_():
	return scale.x

export(float) var position_:float setget set_position_,get_position_
func set_position_(value:float):
	if get_parent() != null&&get_parent().name == "NoteMaster":
		position.x = value*get_parent().width
func get_position_():
	if get_parent() != null&&get_parent().name == "NoteMaster":
		return position.x / get_parent().width
