using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class EmulateData : MonoBehaviour {
	
	public Texture2D mouseIcon;
	public Texture2D leftButton;
	public Texture2D rightButton;
	
	public Texture2D backIcon;
	
	float xmax, xmin, ymax, ymin, zmax, zmin;
	
	public static float dirH;
	public static float dirV;
	
	public static Vector3 udppos;
	public static Quaternion udprot;
	
	List<float> xvalues = new List<float>();
	List<float> yvalues = new List<float>();
	List<float> zvalues = new List<float>();
	float scalex, scaley, scalez;
	bool calibrate = true;
	public GUIStyle txtstyle;
	
	public static float ALPHA = 0.55f;
	public float[] n_input = new float[2];//xy in/out
	public float[] n_output = new float[2];//xy in/out
	
	Vector3 FilteredPosition;
	public static float delta;
	public static bool deltaon = true;
	
	bool yon = true;
	bool zon = false;
	
	public static bool emulate = false;
	public static bool btnemulate = false;
	public Vector2 scrollPositiontrc = Vector2.zero;
	public Vector2 scrollPositionbtnL = Vector2.zero;
	public Vector2 scrollPositionbtnR = Vector2.zero;
	//GUIStyle guistyle;
	
	public static bool leftClick = false;
	public static bool rightClick = false;
	
	public static string selection = "n/a";
	public static string LButton = "n/a";
	public static string RButton = "n/a";
	//window
	public static bool activeWin = false;
	public Rect windowRect0 = new Rect(300, 150, 290, 400);//posx, posy, width, height
	bool activewin=false;
	
	float timer = 0f;
	
	void Update()
	{
		if(emulate || btnemulate)
	    {	
			if(leftClick)
			{
				Emulator.LeftMouseClick(0, 0);
			}
			if(rightClick)
			{
				Emulator.RightMouseClick(0, 0);
			}
		}
		
		//stop emulation if data is not available after a few seconds
		if(UDPReceive.emutracklst.Count == 0)
		{
			timer += Time.deltaTime;
		}
		if(timer >= 3)
		{
			emulate=false;
			resetCalibration();
			timer = 0;
		}
	}

	void FixedUpdate () 
	{
		
		if(Input.GetKey(KeyCode.Q))
		{
			btnemulate=false;//stop button emulation
		}
		
			//apply Time.deltaTime to the lowpass filter
			if(deltaon)
			{
				delta = Time.deltaTime;
			}
			else
			{
				delta = 1.0f;
			}
			//use y or z coordinates for the y axis
			if(yon)
			{
				zon=false;
			}
			else
			{
				yon=false;
				zon=true;
			}
		
		if(emulate)
	    {	
			
			if(calibrate)	
			{
				//XYZ MIN/MAX
				if(udppos.x !=0)//x
				{
					xvalues.Add(-udppos.x);
				
					xvalues.Sort();
					xmin = xvalues[0];
					xmax = xvalues[xvalues.Count-1];
				}
				
				if(udppos.y !=0)//y
				{
					yvalues.Add(-udppos.y);
				
					yvalues.Sort();
					ymin = yvalues[0];
					ymax = yvalues[yvalues.Count-1];
				}	
				
				if(udppos.z !=0)//z
				{
					zvalues.Add(-udppos.z);
				
					zvalues.Sort();
					zmin = zvalues[0];
					zmax = zvalues[zvalues.Count-1];
				}
				
			//	calstatus = calibrate;
			}//end calibrate

			
			int rangemaxx = Screen.currentResolution.width; 
			int rangemaxy = Screen.currentResolution.height;
			int rangeminx = 1; 
			int rangeminy = 1;
			//scale raw data between -1 to 1
			//new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
			scalex = ((-udppos.x - xmin)/(xmax-xmin)) * (rangemaxx - (rangeminx)) + (rangeminx);
			scaley = ((-udppos.y - ymin)/(ymax-ymin)) * (rangemaxy - (rangeminy)) + (rangeminy);
			scalez = ((-udppos.z - zmin)/(zmax-zmin)) * (rangemaxy - (rangeminy)) + (rangeminy);
			
			if(scalex > rangemaxx){scalex=rangemaxx;}
			else if(scalex < rangeminx){scalex=rangeminx;}
			else{scalex=scalex;}
			
			if(scaley > rangemaxy){scaley=rangemaxy;}
			else if(scaley < rangeminy){scaley=rangeminy;}
			else{scaley=scaley;}
			
			if(scalez > rangemaxy){scalez=rangemaxy;}
			else if(scalez < rangeminy){scalez=rangeminy;}
			else{scalez=scalez;}
	
			//put last x,y coordinates to an array in order to feed the tracking to the low pass filter
				n_input[0]=scalex;
				//option for y or z axis to act like y.
				if(yon)
				{n_input[1]=scaley;}//y
				else
				{n_input[1]=scalez;}//z
			
			n_output[0] = lowPass(n_input, n_output)[0];
			n_output[1] = lowPass(n_input, n_output)[1];
			
			//lowPass(n_input, n_output); //[0]for x, [1]for y
			FilteredPosition = new Vector3(lowPass(n_input, n_output)[0], lowPass(n_input, n_output)[1]);	
			
			
				//add mouse cursor code
				Emulator.MoveMouse((int)FilteredPosition.x, (int)FilteredPosition.y);
				//print("emulation");						
				
				if(Input.GetKey(KeyCode.Q))
				{
					emulate=false;//stop emulation
					btnemulate=false;
					resetCalibration();
				}
				if(Input.GetKey(KeyCode.C))
				{
					calibrate=false;//stop calivration
					print("calibration stoped");
				}
				if(Input.GetKey(KeyCode.D))
				{
					if(!deltaon)
					{
					deltaon=true;//enable delta in lowpass
					print("Delta ON");
					Thread.Sleep(100);
					}
					else
					{
					deltaon=false;//disable delta in lowpass
					print("Delta OFF");
					Thread.Sleep(100);
					}
				}
		}//if emulate
		
	}//update
	
	
	IEnumerator Calibrate()
    {
			print("Calibrating...");
            yield return new WaitForSeconds(10);
			calibrate=false;//stop calibration
			print("Calibration finished!");
    }
	
	
	
	//low pass filter x,y
	float[] lowPass( float[] input, float[] output ) 
	{
     for(int i=0; i<input.Length; i++ ) 
		{
			 if( output == null || output.Length == 0 || float.IsNaN(output[0]) || float.IsNaN(output[1]) || float.IsInfinity(output[0]) || float.IsInfinity(output[1]))
			 {
			//	return input;
				output[i] = input[i];
			 }
			 else
			{
				output[i] = output[i] + ALPHA * (input[i] - output[i]) * delta;
			//	output[i] = (1-ALPHA)* input[i] + (ALPHA*output[i]); //ToDo: include a deltatime relative to ALPHA
				
			}
    	}
    		return output;
	}
	
	
	void OnGUI () 
	{
		if(MainGuiControls.EmuMenu)
		{		
	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.EmuMenu = false;
				MainGuiControls.hideMenus = false;
			}
			
			//mouse visualisation
			GUI.Label(new Rect(Screen.width/2-60, 80, 180, 240), mouseIcon);
			
			if(leftClick)
			{
			GUI.Label(new Rect(Screen.width/2-60, 80, 180, 240), leftButton);
			}
			if(rightClick)
			{		
			GUI.Label(new Rect(Screen.width/2-60, 80, 180, 240), rightButton);
			}
				
			
			//list buttons of available joints dynamically
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width/2-60, 330, 130, 200), "Cursor Emulation:");
			GUI.color = Color.white;
			float yOffset = 0.0f;			
			scrollPositiontrc = GUI.BeginScrollView(new Rect(Screen.width/2-100, 350, 200, 130), scrollPositiontrc, new Rect(0, 0, 300, 180+(UDPReceive.emutracklst.Count*20)));
				foreach(string emu in UDPReceive.emutracklst)
		        {
		           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
					{
						resetCalibration();
						selection = emu;//fix to point into a joint
						emulate=true;
						print("Selected: " + emu);
						StartCoroutine(Calibrate());//start calibration
		           	}			
		          yOffset += 25;
		         }	
	        GUI.EndScrollView();
			
			
			//callibratoin pop-up window
				if (activeWin)
				{
		       	 windowRect0 = GUI.Window(0, windowRect0, CalibWindow, "Calibration");
				}
			GUI.enabled = emulate;
			//callibratoin pop-up window
			  if (GUI.Button(new Rect((Screen.width/2)-50, Screen.height/2+160, 100, 20), "Filtering"))
				{activeWin = true;}
				GUI.Label(new Rect((Screen.width/2)-80, Screen.height/2+190, 200, 100), "Press 'q' to stop emulation");
			
			//display calibration dialog
			if(calibrate && emulate)
			{
			GUI.color = Color.yellow;	
			GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+250, 200, 100), "Calibrating...");
			GUI.color = Color.white;	
			}
			else if (!calibrate && emulate)
			{
			GUI.color = Color.green;	
			GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+250, 200, 100), "Calibration finished!");
			GUI.color = Color.white;
			}
			else{GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+250, 200, 100), " ");}
			
			GUI.enabled = true;	
			
				
			
			//left button
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width/2 - 310, Screen.height/2 - 230, 130, 200), "Left Button:");
			GUI.color = Color.white;
			float yOffset2 = 0.0f;			
			scrollPositionbtnL = GUI.BeginScrollView(new Rect(Screen.width/2- 360, Screen.height/2 - 200, 200, 130), scrollPositionbtnL, new Rect(0, 0, 300, 180+(UDPReceive.emubuttonlstL.Count*20)));
				foreach(string emu in UDPReceive.emubuttonlstL)
		        {
		           if(GUI.Button (new Rect (5, 20+ yOffset2, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
					{
						LButton = emu;
						btnemulate=true;
						print("Selected: " + emu);
		           	}			
		          yOffset2 += 25;
		         }	
	        GUI.EndScrollView();
				
				
			//right button
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width/2 + 210, Screen.height/2 - 230, 130, 200), "Right Button:");
			GUI.color = Color.white;
			float yOffset3 = 0.0f;			
			scrollPositionbtnR = GUI.BeginScrollView(new Rect(Screen.width/2 + 160, Screen.height/2 - 200, 200, 130), scrollPositionbtnR, new Rect(0, 0, 300, 180+(UDPReceive.emubuttonlstR.Count*20)));
				foreach(string emu in UDPReceive.emubuttonlstR)
		        {
		           if(GUI.Button (new Rect (5, 20+ yOffset3, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
					{
						RButton = emu;
						btnemulate=true;
						print("Selected: " + emu);
		           	}			
		          yOffset3 += 25;
		         }	
	        GUI.EndScrollView();		
			
		}//if emumenu
						
	}
	
	void resetCalibration()
	{
		xmax = 0;
		xmin = 0;
		ymax = 0;
		ymin = 0;
		zmax = 0;
		zmin = 0;
		scalex = 0;
		scaley = 0;
		scalez = 0;	
		xvalues.Clear();
		yvalues.Clear(); 
		zvalues.Clear();
		calibrate = true;
	}
	
	
		void CalibWindow(int windowID)
		{
			//Filtering & calibration
			GUI.BeginGroup (new Rect (30, 30, 230, 320));
			GUI.Box (new Rect (0,0,230,320), "Tracking Calibration");

			GUI.Label(new Rect(20, 40, 220, 20), "max: "+xmax.ToString("0.00")+","+ymax.ToString("0.00")+","+zmax.ToString("0.00"));
			GUI.Label(new Rect(20, 60, 220, 20), "min: "+xmin.ToString("0.00")+","+ymin.ToString("0.00")+","+zmin.ToString("0.00"));
			GUI.Label(new Rect(20, 90, 220, 20), "Scale: "+scalex.ToString("0.00")+","+scaley.ToString("0.00")+","+scalez.ToString("0.00"));
			
			yon = GUI.Toggle(new Rect(150, 120, 80, 20), yon, "use y");
			zon = GUI.Toggle(new Rect(150, 140, 80, 20), zon, "use z");
			
			//calibrate = GUI.Toggle(new Rect(30, 160, 100, 30), calibrate, "Calibrate");
			
			//filter
			GUI.Label(new Rect(20, 190, 200, 20), "Filtered: "+FilteredPosition.x.ToString("0.00")+","+FilteredPosition.y.ToString("0.00"));
			ALPHA = GUI.HorizontalSlider(new Rect(30, 250, 100, 30), ALPHA, 0.0F, 1.0F);
			GUI.Label(new Rect(50, 270, 200, 100), "Alpha: "+ALPHA.ToString("0.00"));
			deltaon = GUI.Toggle(new Rect(160, 250, 100, 30), deltaon, "Delta");
			GUI.EndGroup ();
		
			//close button
	        if (GUI.Button(new Rect((windowRect0.width/2)-50, windowRect0.height-30, 100, 20), "Close"))
			{
				activeWin = false;
	            print("closing calibration window");
			}
			
			//drag window
	         GUI.DragWindow(new Rect(0, 0, 10000, 20));
    	}
	
}
