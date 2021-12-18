tool
extends Node2D

export(float) var width:float = 10
export(float) var pixelPerSec:float = 10
export(float) var bpm:float = 100
export(bool) var printChart:bool = false setget set_printChart,get_printChart
func set_printChart(value:bool):
	var nodeInfos:String = ""
	for node in get_children():
		if node is Note:
			nodeInfos += node.note_info_to_string(width,pixelPerSec) + "\n"
	print(nodeInfos)
func get_printChart():
	return printChart
