using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class WiimoteTestController : MonoBehaviour {
	
/*	You only need to declare the functions you're going to use. */
/*	~ All functions are enabled here for testing purposes */

	[DllImport ("UniWii")]
	private static extern void wiimote_start();

	[DllImport ("UniWii")]
	private static extern void wiimote_stop();

	[DllImport ("UniWii")]
	private static extern int wiimote_count();
	
	[DllImport ("UniWii")]
	private static extern void wiimote_rumble( int which, float duration);
	[DllImport ("UniWii")]
	private static extern double wiimote_getBatteryLevel( int which );

	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccY(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccZ(int which);

	[DllImport ("UniWii")]
	private static extern float wiimote_getIrX(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getIrY(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getRoll(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getPitch(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getYaw(int which);

	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonA(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonB(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonUp(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonLeft(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonRight(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonDown(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButton1(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButton2(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonPlus(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonMinus(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonHome(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckStickX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckStickY(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckAccX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckAccZ(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonNunchuckC(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonNunchuckZ(int which);

/*	private static extern float wiimote_get( string url);*/
	public Transform objToControl;
	public Transform objToDisplay;
	public float sensitivity = 8.0f;
	public float pitchFudge = 30.0f;
	public float rollFudge = 50.0f;
	public bool showDebugGUI = false;
	public bool enableDebugTests = false;
	public Transform buttonA;
	public Transform buttonB;
	public Transform button1;
	public Transform button2;
	public Transform buttonPlus;
	public Transform buttonMinus;
	public Transform buttonHome;
/*	public Transform buttonC;*/
/*	public Transform buttonZ;*/
	public Transform dpad;
	public Transform expansionPlug;
	public Transform[] lightIndicators;
	public Texture2D wiimote_cursor_tex;
	public Texture2D expansion_cursor_tex;
	private int lightIndicatorCount;
	private bool[] isExpansion;
	private string display;
	private int[] cursor_x, cursor_y;
	private Vector3 vec;
	private Vector3 oldVec;
	public static int wiimoteCount;

	public static int x;
	public static int y;
	public static int z;
	public static float roll;
	public static float pitch;
	public static float yaw;
	public static float ir_x;
	public static float ir_y;
	public static float nx;
	public static float ny;
	public static float nsx;
	public static float nsy;
	
	public static bool btnA = false;
	public static bool btnB = false;
	public static bool btn1 = false;
	public static bool btn2 = false;
	public static bool btnPlus = false;
	public static bool btnMinus = false;
	public static bool btnHome = false;
	public static bool upDpad = false;
	public static bool downDpad = false;
	public static bool leftDpad = false;
	public static bool rightDpad = false;
	public static bool btnC = false;
	public static bool btnZ = false;
	
	private double batteryLevel;
	private float rumbleTime;
	public static bool rumble = false;
	
	public Texture2D backIcon;

/*
	TODO instantiate wiimote prefab on multiple connected wiimotes
	TODO adjust camera view to match # of wiimotes
*/
	
	/* unity methods */
	void Start () {
		/* check for object to control*/
		if (!objToControl)
			objToControl = transform;
		if (!objToDisplay && enableDebugTests) {
			Debug.Log (Time.time.ToString("f2") + ", " + gameObject.name + ": "
				+ ", To enable debug object tests with a display object, you need to set the objToDisplay property on this component!"
				);
		}
		/* if testing buttons, and buttons not set up, then warn*/
		if (enableDebugTests && (!buttonA || !buttonB || !button1 || !button2 || !buttonPlus 
			|| !buttonMinus || !buttonHome || !dpad || !expansionPlug || lightIndicators.Length < 3))
			Debug.Log (Time.time.ToString("f2") + ", " + gameObject.name + ": "
				+ ", To enable debug tests, you need to set the button properties on this component!"
				);

		/* do debug init, if needed */
		else if (enableDebugTests) {
		}

		/* begin init */
		wiimote_start();
		cursor_x = new int[16];
		cursor_y = new int[16];
//		wiimote_cursor_tex = (Texture2D) Resources.Load("crosshair");
		
	}

	void FixedUpdate () 
	{

		wiimoteCount = wiimote_count();
//		if (isExpansion.Length != wiimoteCount)
			isExpansion = new bool[wiimoteCount];

		/* check for wiimote count */
		if (wiimoteCount>0) {
			
			/* execute for every wiimote connected */
			for (int i=0; i<=wiimoteCount-1; i++) {
				/* set member vars to plugin data */
				x = wiimote_getAccX(i);
				y = wiimote_getAccY(i);
				z = wiimote_getAccZ(i);
				roll = wiimote_getRoll(i) + rollFudge;
				pitch = wiimote_getPitch(i) + pitchFudge;
				yaw = wiimote_getYaw(i);
				ir_x = wiimote_getIrX(i);
				ir_y = wiimote_getIrY(i);
				nx = wiimote_getNunchuckAccX(i);
				ny = wiimote_getNunchuckAccZ(i);
				nsx = wiimote_getNunchuckStickX(i);
				nsy = wiimote_getNunchuckStickY(i);
				
				//get battery levels
				batteryLevel = wiimote_getBatteryLevel(i);
		
				/* check for expansion */
				/*
					TODO find a smarter way of detecting expansion state
				*/
/*				if ((nx != 0.05f || nx <= -0.05f) && (ny >= 0.05f || ny <= -0.05f) && isNunchuck)*/
				if (nx == 0.0f && ny == 0.0f && isExpansion[i])
					isExpansion[i] = false;
				else if (nx != 0.0f && ny != 0.0f && !isExpansion[i])
					isExpansion[i] = true;

				/* set target orientation */
				/*
					TODO get yaw working; plugin issue?
				*/
				if (!float.IsNaN(roll) && !float.IsNaN(pitch) && (i==wiimoteCount-1)) {
					vec = new Vector3(pitch, 0 , -1 * roll);
/*					vec = new Vector3(pitch, yaw , -1 * roll);*/
					vec = Vector3.Lerp(oldVec, vec, Time.deltaTime * sensitivity);
					oldVec = vec;
					objToControl.eulerAngles = vec;
				}

				/* ir cursor values */
				if ( (i==wiimoteCount-1) && (ir_x != -100) && (ir_y != -100) ) {
			    	float temp_x = ((ir_x + 1.0f)/ 2.0f) * Screen.width;
			    	float temp_y = Screen.height - (((ir_y + 1.0f)/ 2.0f) * Screen.height);
/*			    	float temp_x = ( Screen.width * 0.5f) + ir_x * Screen.width * 0.5f;
			    	float temp_y = Screen.height - (ir_y * Screen.height * 0.5f);*/
			    	cursor_x[wiimoteCount] = Mathf.RoundToInt(temp_x);
			    	cursor_y[wiimoteCount] = Mathf.RoundToInt(temp_y);
				}

			}
		}
		else {
			/* do something*/
		}

		/* do info for debug gui*/
		if (showDebugGUI) {
			DoDebugStr(0);
		}

		/* do button test visuals */
		if (enableDebugTests) {
			DoDebugTests(0);
		}

	}

	void OnApplicationQuit() {
		wiimote_stop();
		if (objToDisplay)
			objToDisplay.renderer.sharedMaterial.shader = Shader.Find ("Diffuse");
	}
	
	void OnGUI() {
		if (MainGuiControls.WiiMenu) {
		
			//back icon
		if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
		{
			MainGuiControls.WiiMenu = false;
			MainGuiControls.hideMenus = false;
		}	
		
			
		if (wiimoteCount > 0) {	
			//rumble button
			if (GUI.Button (new Rect (25 , Screen.height/2-15 , 100, 30), "<<Rumble>>"))//game image
			{
				wiimote_rumble(0, 0.5f);
				rumble = true;
			}
			else{rumble = false;}
			//Debug.Log("rumble: "+rumble);
			
			//sensitivity horizontal bar
			GUI.Label(new Rect (30, Screen.height/2-105, 100, 20), "Sensitivity");
			sensitivity = GUI.HorizontalSlider(new Rect(25, Screen.height/2-80, 100, 30), sensitivity, 1.0F, 16.0F);
		}
			
		//if (showDebugGUI) {
			//GUI.Label( new Rect(10,10, 500, 100), display);
			//if ((cursor_x != 0) || (cursor_y != 0)) GUI.Box ( new Rect (cursor_x, cursor_y, 50, 50), wiimote_cursor_tex); //"Pointing\nHere");
/*			int wiimoteCount = wiimote_count();*/

			/* create style for cursor*/
			GUIStyle label_wiimote_cursor;
			if (wiimote_cursor_tex) {
				label_wiimote_cursor = new GUIStyle();
				label_wiimote_cursor.normal.background = wiimote_cursor_tex;
			}
			else
				label_wiimote_cursor = "box";
			label_wiimote_cursor.clipping = TextClipping.Overflow;
			label_wiimote_cursor.normal.textColor = Color.red;

			/* show debug info */
			GUI.Label( new Rect(10,50, Screen.width-10, Screen.height-10), display);

			/* for each remote, show debug cursors */
			for (int i=0; i<=wiimoteCount-1; i++) {
				/* wiimote cursor */
/*				if ((cursor_x[wiimoteCount] != 0) || (cursor_y[wiimoteCount] != 0)) */
/*					GUI.Box ( new Rect (cursor_x[wiimoteCount], cursor_y[wiimoteCount], 64.0f, 64.0f), "Wiimote #" + wiimoteCount);*/

				/* use ir */
				float ir_x = wiimote_getIrX(i);
				float ir_y = wiimote_getIrY(i);
/*			    if ( (ir_x != -100.0f) && (ir_y != -100.0f) ) {*/
/*				    float temp_x = ((ir_x + 1.0f)/ 2.0f) * Screen.width;
				    float temp_y = Screen.height - (((ir_y + 1.0f)/ 2.0f) * Screen.height);*/
			    	float temp_x = (Screen.width * 0.5f) + ir_x * Screen.width * 0.5f;
			    	float temp_y = Screen.height - (ir_y * Screen.height * 0.5f);
					GUI.Box ( new Rect (temp_x, temp_y, 64.0f, 64.0f), "IR Pointer #" + i, label_wiimote_cursor);
/*					GUI.Box ( new Rect (ir_x, ir_y, 64.0f, 64.0f), (wiimote_cursor_tex) ? "" : "Pointer #" + i , label_wiimote_cursor);*/
/*			    }*/

				/* expansion joystick cursor */
				if (isExpansion[i]) {
					if (expansion_cursor_tex)
						label_wiimote_cursor.normal.background = expansion_cursor_tex;
					else {
						label_wiimote_cursor = "box";
						label_wiimote_cursor.normal.background = null;
					}

					label_wiimote_cursor.normal.textColor = Color.yellow;

				    Vector3 extJoyViewport = new Vector3(wiimote_getNunchuckStickX(i) * (Screen.width / 256), Screen.height - (wiimote_getNunchuckStickY(i) * (Screen.height / 256)), 0.0f);
					GUI.Box ( new Rect (extJoyViewport.x, extJoyViewport.y, 50.0f, 50.0f), (expansion_cursor_tex) ? "" : "Expansion #" + i , label_wiimote_cursor);
				}
			}
		}
	}

	
	/* custom methods */
	
	void DoDebugStr (int i) {
		if (wiimoteCount > 0) {
			display = "";

			display += "Wiimote #" + i + ":"
			//	+ "\n\t\tBattery: " + batteryLevel 
				+ "\n\tAccelerometer: " + x + ", " + y + ", " + z 
				+ "\n\t\tRoll: " + roll 
				+ "\n\t\tPitch: " + pitch 
				+ "\n\t\tYaw: " + yaw 
				+ "\n\tIR Pitch & Yaw: " + ir_x + ", " + ir_y
				+ "\n\tOrientation Vector: " + vec
				+ "\n\tSensitivity: " + sensitivity.ToString("F2");
			if (isExpansion[i]) {
				display += "\n\tExpansion Accelerometer: " + nx + ", " + ny 
					+ "\n\tExpansion Joystick: " + nsx + ", " + nsy;
			}
			else {
				display += "\n\tExpansion Device: _not detected_";
			}
			display += "\n\tButtons Active: ";
			if (wiimote_getButtonA(i))
			{
				display += " A ";
				btnA = true; 
			}
			else{btnA = false;}
			
			if (wiimote_getButtonB(i))
			{
				display += " B ";
				btnB = true; 
			}
			else{btnB = false;}
			
			if (wiimote_getButtonHome(i))
			{
				display += " Home ";
				btnHome = true;
			}
			if (wiimote_getButtonPlus(i))
			{
				display += " + ";
				btnPlus = true;
			}
			if (wiimote_getButtonMinus(i))
			{
				display += " - ";
				btnMinus = true;
			}
			if (wiimote_getButton1(i))
			{
				display += " 1 ";
				btn1 = true;
			}
			if (wiimote_getButton2(i))
			{
				display += " 2 ";
				btn2 = true;
			}
			/* dpad */
			if (wiimote_getButtonUp(i))
			{
				display += " Up ";
				upDpad = true;
			}
			if (wiimote_getButtonDown(i))
			{
				display += " Down ";
				downDpad = true;
			}
			if (wiimote_getButtonLeft(i))
			{
				display += " Left ";
				leftDpad = true;
			}
			if (wiimote_getButtonRight(i))
			{
				display += " Right ";
				rightDpad = true;
			}
			/* expansion */
			if (wiimote_getButtonNunchuckC(i))
			{
				display += " C ";
				btnC = true;
			}
			if (wiimote_getButtonNunchuckZ(i))
			{
				display += " Z ";
				btnZ = true;
			}
		}
		else {
			display = "No Wii Remote detected... \nPress the '1' & '2' buttons on your Wii Remote to search!";
		}
	}

	void DoDebugTests (int i) {
		if (wiimoteCount > 0) {
			if (objToDisplay && objToDisplay.renderer.sharedMaterial.shader != Shader.Find ("Diffuse")) {
				objToDisplay.renderer.sharedMaterial.shader = Shader.Find ("Diffuse");
			}

			if (lightIndicatorCount != wiimoteCount) {
				DoLightInit(wiimoteCount);
			}

			/*
				TODO work around no color = operand in C# to set color only on change
			*/
/*			if (isExpansion[i] && !expansionPlug.gameObject.active || expansionPlug.renderer.material.color != activeColor) {*/
			if (isExpansion[i]) {
				Color activeColor = new Color (1.0f,0.0f,0.0f,0.5f);
				if (wiimote_getButtonNunchuckC(i)) activeColor.a = 1.0f;
				else if (wiimote_getButtonNunchuckZ(i)) activeColor.a = 1.0f;
/*				else if (extJoyAfter != extJoyBefore) activeColor.a = 1.0f; */
				else {
					activeColor.a = 0.5f;
				}
				expansionPlug.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
				SetColorInstance(expansionPlug, activeColor);
				expansionPlug.gameObject.active = true;
				/* do detect unplugged - wonky! */
				if (nx == 255.0f && ny == 255.0f)
					isExpansion[i] = false;
			}
			if (!isExpansion[i] && expansionPlug.gameObject.active) {
				expansionPlug.gameObject.active = false;
				SetColorInstance(expansionPlug, Color.white);
			}

			/* do button debug behavior */
			if (wiimote_getButtonA(i)) SetColorInstance (buttonA, Color.red);
			else SetColorInstance(buttonA, Color.white);
			if (wiimote_getButtonB(i)) SetColorInstance (buttonB, Color.red);
			else SetColorInstance(buttonB, Color.white);
			if (wiimote_getButtonHome(i)) SetColorInstance (buttonHome, Color.red);
			else SetColorInstance(buttonHome, Color.white);
			if (wiimote_getButtonPlus(i)) {
				sensitivity += 0.05f;
				SetColorInstance (buttonPlus, Color.red);
			}
			else SetColorInstance(buttonPlus, Color.white);
			if (wiimote_getButtonMinus(i)) { 
				sensitivity -= 0.05f;
				SetColorInstance (buttonMinus, Color.red);
			}
			else SetColorInstance(buttonMinus, Color.white);
			if (wiimote_getButton1(i)) SetColorInstance (button1, Color.red);
			else SetColorInstance(button1, Color.white);
			if (wiimote_getButton2(i)) SetColorInstance (button2, Color.red);
			else SetColorInstance(button2, Color.white);
			/* dpad */
			if (wiimote_getButtonUp(i)) {
				if (objToDisplay)
					objToDisplay.localPosition += new Vector3(0,0,0.10f);
				SetColorInstance (dpad, Color.red);
			}
			else if (wiimote_getButtonDown(i)) {
				if (objToDisplay)
					objToDisplay.localPosition -= new Vector3(0,0,0.10f);
				SetColorInstance (dpad, Color.red);
			}
			else if (wiimote_getButtonLeft(i)) {
				SetColorInstance (dpad, Color.red);
/*				sensitivity -= 0.05f;*/
			}
			else if (wiimote_getButtonRight(i)) {
				SetColorInstance (dpad, Color.red);
/*				sensitivity += 0.05f;*/
			}
			else SetColorInstance(dpad, Color.white);
			sensitivity = Mathf.Clamp (sensitivity, 1.0f, 16.0f);
			if (objToDisplay)
				objToDisplay.localPosition = new Vector3(objToDisplay.localPosition.x, objToDisplay.localPosition.y, Mathf.Clamp (objToDisplay.localPosition.z, 0.0f, 16.0f));
			/* expansion port */
			/*
				TODO Nunchuck 3d model
			*/
/*			if (wiimote_getButtonNunchuckC(i)) SetColorInstance (buttonC, Color.red);
			else SetColorInstance(buttonC, Color.white);
			if (wiimote_getButtonNunchuckZ(i)) SetColorInstance (buttonZ, Color.red);
			else SetColorInstance(buttonZ, Color.white);*/
		}
		else {
			if (objToDisplay && enableDebugTests && objToDisplay.renderer.sharedMaterial.shader != Shader.Find ("Transparent/Diffuse")) {
				SetColorShared (objToDisplay, new Color(1.0f,1.0f,1.0f,0.25f));
				objToDisplay.renderer.sharedMaterial.shader = Shader.Find ("Transparent/Diffuse");
			}
		}
	}

	/* scene light indicator visual helper */
	void DoLightInit (int newCount) {
		int i=0;
		for (i=0; i<=3; i++) {
			lightIndicators[i].gameObject.active = false;
		}
		lightIndicatorCount = 0;
		for (i=0; i<newCount; i++) {
			lightIndicators[i].gameObject.active = true;
		}
		lightIndicatorCount = newCount;
	}

	
	/* utilities */
	
	/* scene indicator visual helper */
	void SetColorInstance (Transform someTR, Color newColor) {
		someTR.renderer.material.color = newColor;
	}
	
	void SetColorShared (Transform someTR, Color newColor) {
		someTR.renderer.sharedMaterial.color = newColor;
	}
}
