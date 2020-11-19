using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputWrapperP2 : GameInputWrapper
{
	public override void SetCurrent(CJI_INPUT newInputs) 
	{
		if (newInputs==null)
			return;

		if (currentInputs == null)
			return;

		currentInputs = newInputs;

		currentGameInputs.trigger1 			= currentInputs.btns[10].isOn;
		currentGameInputs.trigger2 			= currentInputs.btns[11].isOn;

		currentGameInputs.returnHolorangs   = currentInputs.btns[4].isOn;
		currentGameInputs.allowRun			= currentInputs.btns[4].isOn;
		currentGameInputs.mouseLeft			= currentInputs.btns[4].isOn;
		currentGameInputs.mouseRight		= currentInputs.btns[5].isOn;
		currentGameInputs.mouseMiddle		= currentInputs.btns[6].isOn;

		currentGameInputs.mouseLeftHold		= currentInputs.btns[7].isOn;
		currentGameInputs.mouseRightHold	= currentInputs.btns[8].isOn;
		currentGameInputs.mouseMiddleHold	= currentInputs.btns[9].isOn;

		currentGameInputs.camRotX 			= currentInputs.sticks[1].x;
		currentGameInputs.camRotY 			= currentInputs.sticks[1].y;

		currentGameInputs.camRotDeltaX 		= currentInputs.stickDeltas[1].x;
		currentGameInputs.camRotDeltaY 		= currentInputs.stickDeltas[1].y;

		currentGameInputs.moveDirs			= currentInputs.sticks[1].vVal;
		currentGameInputs.turnDirs			= currentInputs.sticks[1].vVal;

		currentGameInputs.rHandPos			= currentInputs.pointers[0].vPos;
		currentGameInputs.rHandDir			= currentInputs.pointers[0].vDir;

		currentGameInputs.lHandPos			= currentInputs.pointers[1].vPos;
		currentGameInputs.lHandDir			= currentInputs.pointers[1].vDir;
		currentGameInputs.hasHandData		= currentInputs.hasHandData;
	} 
}
