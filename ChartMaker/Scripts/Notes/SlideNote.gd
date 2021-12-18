tool
extends Note

export(Direction) var whichLeg
export(Direction) var slideDirection


func note_info_to_string(width:float,pixelPerSec:float) -> String:
	if whichLeg==Direction.Left:
		if slideDirection==Direction.Left:
			return "S,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",L,L"
		else:
			return "S,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",L,R"
	else:
		if slideDirection==Direction.Left:
			return "S,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",R,L"
		else:
			return "S,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",R,R"
