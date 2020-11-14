using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface i_NativeInputIFC 
{
	CJI_INPUT  GetLastRead();

	void  Preset(CJI_INPUT inVals);
	void  Read();

	bool HasIK();

}
