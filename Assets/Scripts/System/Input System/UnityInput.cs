using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityInput : MonoBehaviour,i_NativeInputIFC
{
	CJI_INPUT lastRead; 

	public CJI_INPUT  GetLastRead() { return lastRead;}
	public void  Preset(CJI_INPUT inVals) {lastRead = inVals;}

	void Awake()
	{
		lastRead = ScriptableObject.CreateInstance<CJI_INPUT>();
	}
	public bool HasIK()
	{
		return false;
	}
	public void  Read()
	{
		if (lastRead == null)
			return;

		// Maybe one PC will set this true
		lastRead.hasHandData = HasIK();

		// Player 1 KBD
		lastRead.btns[0].isOn = Input.GetKeyDown(KeyCode.F1);
		lastRead.btns[1].isOn = Input.GetKeyDown(KeyCode.Space);
		lastRead.btns[2].isOn = Input.GetKeyDown(KeyCode.RightControl);
		lastRead.btns[3].isOn = Input.GetKeyDown(KeyCode.RightShift);
		lastRead.btns[4].isOn = Input.GetMouseButtonDown(0);
		lastRead.btns[5].isOn = Input.GetMouseButtonDown(1);
		lastRead.btns[6].isOn = Input.GetMouseButtonDown(2);
		lastRead.btns[7].isOn = Input.GetMouseButton(0);
		lastRead.btns[8].isOn = Input.GetMouseButton(1);
		lastRead.btns[9].isOn = Input.GetMouseButton(2);

		// Player 2 KBD
		lastRead.btns[10].isOn = Input.GetKeyDown(KeyCode.RightControl);

		// Arrow keys stick 1
		lastRead.stickDeltas[0].x 	= lastRead.sticks[0].x;
		lastRead.stickDeltas[0].y 	= lastRead.sticks[0].y;

		lastRead.sticks[0].x  		= (Input.GetKey(KeyCode.A)?-1.0f:0.0f) +(float)(Input.GetKey(KeyCode.D)?1.0f:0.0f);
		lastRead.sticks[0].y  		= (Input.GetKey(KeyCode.W)?1.0f:0.0f)   +(Input.GetKey(KeyCode.S)?-1.0f:0.0f);  

		lastRead.stickDeltas[0].x  	= lastRead.sticks[0].x - lastRead.stickDeltas[0].x; 
		lastRead.stickDeltas[0].y  	= lastRead.sticks[0].y - lastRead.stickDeltas[0].y; 


		// Mouse move stick 2
		lastRead.stickDeltas[1].x 	= lastRead.sticks[1].x;
		lastRead.stickDeltas[1].y  	= lastRead.sticks[1].y; 

		lastRead.sticks[1].x  		= (Input.GetKey(KeyCode.LeftArrow) ? -1.0f : 0.0f) + (float)(Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f) ; 
		lastRead.sticks[1].y  		= (Input.GetKey(KeyCode.DownArrow) ? -1.0f : 0.0f) + (float)(Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f);

		lastRead.stickDeltas[1].x  	= lastRead.sticks[1].x - lastRead.stickDeltas[1].x; 
		lastRead.stickDeltas[1].y  	= lastRead.sticks[1].y - lastRead.stickDeltas[1].y; 

		// A throttle
		lastRead.sticks[2].x  += (Input.GetKey(KeyCode.A)?-Time.deltaTime:0.0f) + (Input.GetKey(KeyCode.D)?Time.deltaTime:0.0f);  
		lastRead.sticks[2].y  += (Input.GetKey(KeyCode.W)?-Time.deltaTime:0.0f) + (Input.GetKey(KeyCode.S)?Time.deltaTime:0.0f);  

		Mathf.Clamp(lastRead.sticks[1].x,-1.0f,1.0f); 
		Mathf.Clamp(lastRead.sticks[1].y,-1.0f,1.0f); 

	}
		// Update is called once per frame
		void Update()
    {
		Read();
    }
}
