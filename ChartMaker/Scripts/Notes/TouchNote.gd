tool
extends Note

export(Direction) var whichLeg

func note_info_to_string(width:float,pixelPerSec:float) -> String:
	if whichLeg==Direction.Left:
		return "T,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",L"
	else:
		return "T,"+String(position.x/width)+","+String(scale.x)+","+String(-position.y/pixelPerSec)+",R"
