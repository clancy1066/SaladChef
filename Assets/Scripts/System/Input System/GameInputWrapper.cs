using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MY_GAME_INPUTS
{
	// General actions
	public bool	trigger1;
	public bool trigger2;
	public bool trigger3;
	public bool allowRun;
	public bool jump;
	public bool run;

	public bool	mouseLeft;
	public bool mouseRight;
	public bool mouseMiddle;

	public bool	mouseLeftHold;
	public bool mouseRightHold;
	public bool mouseMiddleHold;

	// sticks
	public float move_X;
	public float move_Y;

	// Stick 2 - Camera
	public float camRotX;
	public float camRotY;
	public float camRotDeltaX;
	public float camRotDeltaY;

	public Vector2 moveDirs;
	public Vector2 turnDirs;

	// throttle
	public float throttle;

	// Pointers
	public bool	   hasHandData;
	public Vector3 rHandPos;
	public Vector3 rHandDir;
	public Vector3 lHandPos;
	public Vector3 lHandDir;

	// Control
	public bool pause;
	public bool space;

	// Hacks
	public bool	returnHolorangs;
};

public class GameInputWrapper : MonoBehaviour
{
	public InputHandler		inputHandler;

	// From the machine
	static CJI_INPUT		 currentInputs;

	// Massaged into game date
	static MY_GAME_INPUTS	currentGameInputs;

	public void				SetInputHandler(InputHandler newInputHandler) {inputHandler = newInputHandler; }

	[SerializeField]
	static public MY_GAME_INPUTS	GetLastRead() { return currentGameInputs; }

	void Start()
	{
		currentInputs = ScriptableObject.CreateInstance<CJI_INPUT>();

	//	currentGameInputs = new MY_GAME_INPUTS();
	}

	void Update()
	{
		if (inputHandler)
		{
			currentInputs = inputHandler.GetLastRead(); 
			SetCurrent(currentInputs);
		}
	}

	public void SetCurrent(CJI_INPUT newInputs) 
	{
		if (newInputs==null)
			return;

		if (currentInputs == null)
			return;

		currentInputs = newInputs;

		currentGameInputs.trigger1 			= currentInputs.btns[0].isOn;
		currentGameInputs.trigger2 			= currentInputs.btns[1].isOn;
		currentGameInputs.trigger3			= currentInputs.btns[2].isOn;

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

		currentGameInputs.moveDirs			= currentInputs.sticks[0].vVal;
		currentGameInputs.turnDirs			= currentInputs.sticks[1].vVal;

		currentGameInputs.rHandPos			= currentInputs.pointers[0].vPos;
		currentGameInputs.rHandDir			= currentInputs.pointers[0].vDir;

		currentGameInputs.lHandPos			= currentInputs.pointers[1].vPos;
		currentGameInputs.lHandDir			= currentInputs.pointers[1].vDir;
		currentGameInputs.hasHandData		= currentInputs.hasHandData;
	} 
}
