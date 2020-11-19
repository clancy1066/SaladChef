using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


[StructLayout(LayoutKind.Explicit,Pack=4)]
public struct ST_BUTTON
{
	#region
	[FieldOffset(0)]
	public bool isOn;
	[FieldOffset(0)]
	public bool 	triggered;
	[FieldOffset(0)]
	public float 	fVal; // For analog buttons
	#endregion
};

// All sticks 2D
[StructLayout(LayoutKind.Explicit,Pack=16)]
public struct ST_STICK
{
	#region
	[FieldOffset(0)]
	public Vector2 vVal;
	[FieldOffset(0)]
	public float 	x;
	[FieldOffset(4)]
	public float 	y;
	[FieldOffset(8)]
	public float z;

	#endregion
};

// Pointers mouse/hand controller
[StructLayout(LayoutKind.Explicit, Pack = 32)]
public struct ST_POINTER
{
	#region
	[FieldOffset(0)]
	public Vector3 vPos;
	
	[FieldOffset(0)]
	public float x;
	[FieldOffset(4)]
	public float y;
	[FieldOffset(8)]
	public float z;

	[FieldOffset(12)]
	public Vector3 vDir;

	[FieldOffset(24)]
	public Vector3 vHit;
	#endregion
};

// Placeholder for live data feeds (Streaming)
[StructLayout(LayoutKind.Explicit,Pack=4)]
public struct ST_FEED
{
	#region
	[FieldOffset(0)]
	public byte[] lastData;
	#endregion
};

public class CJI_INPUT : ScriptableObject
{
	static int numButtons	= 64;
	static int numSticks	= 8; 
	static int numPointers	= 2;
	static int numFeeds		= 4;

	public ST_BUTTON[] btns;        // 	= new ST_BUTTON[numButtons];
//	public ST_BUTTON[] btnDeltas;   // 	= new ST_BUTTON[numButtons];
	public ST_STICK[] sticks;       // = new ST_STICK[numSticks];
	public ST_STICK[] stickDeltas;  // = new ST_STICK[numSticks];
	public ST_POINTER[] pointers;   // = new ST_STICK[numPointers];
	public ST_FEED[] feeds;         //	= new ST_FEED[numFeeds];
	public bool      hasHandData;         //	= new ST_FEED[numFeeds];

	void Awake()
	{
		Debug.Log("CJI_INPUT AWAKE");

		btns		= new ST_BUTTON[numButtons];
	//	btnDeltas	= new ST_BUTTON[numButtons];
		sticks		= new ST_STICK[numSticks];
		stickDeltas = new ST_STICK[numSticks]; 
		pointers	= new ST_POINTER[numPointers];
		feeds		= new ST_FEED[numFeeds];
	}

	void Start()
	{
		Debug.Log("CJI_INPUT START");
	}
}

public class InputHandler : MonoBehaviour 
{
	// Native input handler. Set this in Inspector
	public i_NativeInputIFC		currentIFC;

	// Other platform live here
	public Transform			nativeIFCHolder;
	i_NativeInputIFC[]			nativeIFCs;

	CJI_INPUT lastRead;

	public CJI_INPUT GetLastRead() { return lastRead; }

	void Start()
	{
		lastRead = ScriptableObject.CreateInstance<CJI_INPUT>();

		if (nativeIFCHolder)
		{
			nativeIFCs = nativeIFCHolder.gameObject.GetComponentsInChildren<i_NativeInputIFC>();

			if (nativeIFCs!=null)
				Debug.Log("nativeIFCs: " + nativeIFCs.Length);

			if (nativeIFCs.Length > 0)
				currentIFC = nativeIFCs[0];
			else
				Debug.Log("No IFCs in holder: " + nativeIFCHolder.name);

		}
		else
			Debug.Log("No IFC holder");

		Debug.Log("IFC: " + currentIFC);

		if (currentIFC!=null) lastRead = currentIFC.GetLastRead();

	}
		
	void Update()
	{
		if (currentIFC != null)
		{
			lastRead = currentIFC.GetLastRead();
		}
	}
}
