using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using Windows.Kinect;

public class KinectGUI : MonoBehaviour {

	private KinectSensor _Sensor1;

	public Camera CamKinectModel;

	public Texture2D backIcon;
	public Texture2D rightHandGesture;
	public Texture2D leftHandGesture;

	//kinect parameters
	bool readingAngle = false;
	public static bool SeatedMode = false;
	public static bool TrackSkeletonInNearMode = false;
	public static bool NearMode = false;
	public static bool toggleMirror = false;
	public static string longWord = "-20"; //-27 to 27
	//	 float slideangle = 20.0f; 
	public static int angle;
	
	private Thread t;
	//smoothing parameters
	public static float Smoothing = 0.7f; 
	public static float Correction = 0.3f; 
	public static float Prediction = 1.0f; 
	public static float JitterRadius = 1.0f; 
	public static float MaxDeviationRadius = 1.0f; 
	
	public static int resize = 0;
	
	//	private bool auto = false;
	//	private bool microsoftsdk = true;
	//	private bool openni = false;
	private bool motorgui = false;
	private bool smoothgui = true;

	public static bool startzigfu = false;	

	//for docking/undocking
	public static int gone = 0;
	public static int nwidth= 0;
	

	// Use this for initialization
	void Awake()
	{		 
//		if (this._Sensor1 != null)
//		{
//			this._Sensor1.Open();
//		}
		
	}

	void Start () {

		if (HandlePrefs.bciMode != "on") {
//		startKinect();
						motorgui = true;
						smoothgui = false;	
						MainGuiControls.hideviewers = 0;			
						ZigSkeleton.mirror = true;				
						startzigfu = true;
						sendULData ();
				}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	static void setAngle()//camera motor
	{       
		long a = (long)angle;              
		NuiWrapper.NuiCameraElevationSetAngle(a);
		Thread.Sleep(0);   
	}
	
	
	static int getAngle()//camera motor
	{
		long angleOut;    
		NuiWrapper.NuiCameraElevationGetAngle(out angleOut);
		return (int)angleOut;
	}


	void OnGUI()
	{
		if(MainGuiControls.KinectMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.KinectMenu = false;
				MainGuiControls.hideMenus = false;
				CameraSwitch.avatarView = false;
			}	
			
			//wave detetor	
			//		GUI.Label(new Rect(Screen.width/2-80, Screen.height/2+10, 100, 25), ZigHandSessionDetector.hand);
			
			if(ZigHandSessionDetector.hand=="left")
			{
				GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-35, 80, 80), leftHandGesture);	
			}
			else if (ZigHandSessionDetector.hand=="right")
			{
				GUI.Label(new Rect(Screen.width/2+20, Screen.height/2-35, 80, 80), rightHandGesture);	
			}
			else
			{
				GUI.Label(new Rect(Screen.width/2-80, Screen.height/2, 100, 25), ZigHandSessionDetector.hand);
			}
			
			//avatar labels
			if(CamKinectModel.enabled==true)
			{
				//Model Viewer Boxes
				GUI.BeginGroup (new Rect (Screen.width / 2 - 290-gone, (Screen.height / 2 - 275)*MainGuiControls.hideMenu, 280, 25));
				GUI.color = Color.yellow;
				GUI.Box (new Rect (0,0,280,25), "Kinect Avatar");		
				GUI.EndGroup ();
				
				
				GUI.BeginGroup (new Rect (Screen.width / 2 + 10-gone, (Screen.height / 2 - 275)*MainGuiControls.hideMenu, 280, 25));
				GUI.color = Color.yellow;
				GUI.Box (new Rect (0,0,280,25), "Network Avatar");		
				GUI.EndGroup ();
				
			}
			
			GUI.enabled = motorgui;
			
			//		GUI.BeginGroup (new Rect (Screen.width / 2 - 500, (Screen.height / 2 - 320)*hideMenu, 200, 240));
			GUI.BeginGroup (new Rect (10+gone, (Screen.height / 2 - 275)*MainGuiControls.hideMenu, 200, 235));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,235), "Motor Control");
			GUI.color = Color.white;
			
			//        longWord = GUI.TextField(new Rect(10, 30, 30, 30), readingAngle ? getAngle().ToString() : longWord, 20);
			longWord = GUI.TextField(new Rect(10, 35, 40, 30), longWord, 20);
			//		slideangle = GUI.VerticalSlider (new Rect (160, 30, 30, 30), slideangle, 27.0f, -27.0f); //slide bar
			
			
			
			if (GUI.Button(new Rect(30, 70, 20, 20), ">")&& int.Parse(longWord)<= 27)
			{
				int newangle = int.Parse(longWord)+1;
				longWord = newangle.ToString();
			}
			if (GUI.Button(new Rect(10, 70, 20, 20), "<")&& int.Parse(longWord)>= -27)
			{
				int newangle = int.Parse(longWord)-1;
				longWord = newangle.ToString();
			}
			
			if (GUI.Button(new Rect(50, 35, 80, 30), "Set Angle") && int.Parse(longWord)<= Math.Abs(27))
			{
				
				angle = int.Parse(longWord);
				//		angle = (int)longWord;
				NuiWrapper.NuiCameraElevationSetAngle(angle);
				t = new Thread(setAngle);    //attempted a Paramaterized Thread to no avail       
				t.Start();
				Thread.Sleep(0);           
				
			}
			
			GUI.Label(new Rect(20, 100, 120, 25), "Current Angle: " + getAngle().ToString());
			
			//        readingAngle = GUI.Toggle(new Rect(10, 100, 100, 30), readingAngle, "Read Angle");      
			
			GUI.enabled = false;
			
			GUI.enabled = true;
			
			GUI.color = Color.yellow;
			GUI.Label(new Rect(70+gone, 130, 100, 25), "Settings"); // section title
			GUI.color = Color.white;
			
			bool nNearMode = GUI.Toggle(new Rect(10, 150, 100, 20), NearMode, "Near Mode");
			if (nNearMode != NearMode)
			{
				NearMode = nNearMode;
				ZigInput.Instance.SetNearMode(NearMode);
			}
			bool nSeatedMode = GUI.Toggle(new Rect(10, 170, 100, 20), SeatedMode, "Seated Mode");
			bool nTrackSkeletonInNearMode = GUI.Toggle(new Rect(10, 190, 190, 20), TrackSkeletonInNearMode, "Track Skeleton In NearMode");
			if ((nSeatedMode != SeatedMode) || (TrackSkeletonInNearMode != nTrackSkeletonInNearMode))
			{
				SeatedMode = nSeatedMode;
				TrackSkeletonInNearMode = nTrackSkeletonInNearMode;
				ZigInput.Instance.SetSkeletonTrackingSettings(SeatedMode, TrackSkeletonInNearMode);
			}
			
			//mirror joints button
			toggleMirror = GUI.Toggle(new Rect(10, 210, 100, 20), toggleMirror, "Mirror Joints");
			if(toggleMirror)
			{
				ZigSkeleton.mirror=true;
			}
			else
			{
				ZigSkeleton.mirror=false;
			}
			
			GUI.enabled = true;
			
			
			//		//SDK toggle buttons
			//		KinectSdk = GUI.Toggle(new Rect(5, 235, 100, 20), KinectSdk, "KinectSDK");
			//		OpenNI = GUI.Toggle(new Rect(70, 235, 100, 20), OpenNI, "OpenNI");
			//		OpenNI2 = GUI.Toggle(new Rect(120, 235, 100, 20), OpenNI2, "OpenNI2");
			
			GUI.EndGroup (); // end settings group
			
			
			GUI.enabled = smoothgui;		
			// Smoothing Param Group
			//		GUI.BeginGroup (new Rect (Screen.width / 2 - 500, (Screen.height / 2 - 70)*hideMenu, 200, 340));
			//		GUI.BeginGroup (new Rect (Screen.width / 2 - 500+gone, (Screen.height / 2 - 40)*hideMenu, 200, 280));
			GUI.BeginGroup (new Rect (10+gone, (Screen.height / 2 - 30)*MainGuiControls.hideMenu, 200, 2750));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,275), "Smoothing Parameters");
			GUI.color = Color.white;
			
			GUI.Label(new Rect(10, 30, 100, 20), "Smoothing");//h 35
			Smoothing = GUI.HorizontalSlider (new Rect (10, 55, 100, 20), Smoothing, 0.0f, 1.0f);
			GUI.Label(new Rect(120, 50, 100, 20), Smoothing.ToString("0.00"));
			//		ZigInput.Settings.KinectSDKSpecific.SmoothingParameters.Smoothing = Smoothing;
			
			GUI.Label(new Rect(10, 70, 100, 20), "Correction");
			Correction = GUI.HorizontalSlider (new Rect (10, 95, 100, 20), Correction, 0.0f, 1.0f);
			GUI.Label(new Rect(120, 90, 100, 20), Correction.ToString("0.00"));
			//		ZigInput.Settings.KinectSDKSpecific.SmoothingParameters.Correction = Correction;
			
			GUI.Label(new Rect(10, 110, 100, 20), "Prediction");
			Prediction = GUI.HorizontalSlider (new Rect (10, 135, 100, 20), Prediction, 0.0f, 1.0f);
			GUI.Label(new Rect(120, 130, 100, 20), Prediction.ToString("0.00"));
			//		ZigInput.Settings.KinectSDKSpecific.SmoothingParameters.Prediction = Prediction;
			
			GUI.Label(new Rect(10, 150, 100, 20), "Jitter Radius");
			JitterRadius = GUI.HorizontalSlider (new Rect (10, 175, 100, 20), JitterRadius, 0.0f, 1.0f);
			GUI.Label(new Rect(120, 170, 100, 20), JitterRadius.ToString("0.00"));
			//		ZigInput.Settings.KinectSDKSpecific.SmoothingParameters.JitterRadius = JitterRadius;
			
			GUI.Label(new Rect(10, 190, 160, 20), "Max Deviation Radius");
			MaxDeviationRadius = GUI.HorizontalSlider (new Rect (10, 215, 100, 20), MaxDeviationRadius, 0.0f, 1.0f);
			GUI.Label(new Rect(120, 210, 100, 20), MaxDeviationRadius.ToString("0.00"));
			//		ZigInput.Settings.KinectSDKSpecific.SmoothingParameters.MaxDeviationRadius = MaxDeviationRadius;
			
			if (GUI.Button (new Rect (10,240,40,25), "Min"))
			{
				Smoothing = 0.5f; 
				Correction = 0.1f; 
				Prediction = 0.5f; 
				JitterRadius = 0.1f; 
				MaxDeviationRadius = 0.1f; 
			}
			if (GUI.Button (new Rect (60,240,40,25), "Med"))
			{
				Smoothing = 0.5f; 
				Correction = 0.5f; 
				Prediction = 0.5f; 
				JitterRadius = 0.05f; 
				MaxDeviationRadius = 0.04f; 	
			}
			if (GUI.Button (new Rect (110,240,40,25), "Max"))
			{
				Smoothing = 0.7f; 
				Correction = 0.3f; 
				Prediction = 1.0f; 
				JitterRadius = 1.0f; 
				MaxDeviationRadius = 1.0f; 
				
			}
			
			GUI.EndGroup (); //end smoothing group
			
			GUI.enabled = true;	
			
			
			//Enable/Disable Kinect
			GUI.BeginGroup (new Rect (Screen.width / 2- 290+gone , (Screen.height / 2 + 50+resize)*MainGuiControls.hideMenu, 580, 320));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,580,320), "Sensor Viewers");
			GUI.color = Color.white;
			
			if (GUI.Button (new Rect (550,10,20,20), "_"))
			{
				if (resize ==0)
				{
					resize = 280;
					Debug.Log("Min");
				}
				else
				{
					resize = 0;
					Debug.Log("Max");
				}
			}	
			
			
			GUI.EndGroup ();//end of sensor viewer group
			
			
			//Enable/Disable Kinect
			GUI.BeginGroup (new Rect (Screen.width - 212+gone , Screen.height / 2 - 275, 200, 60));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,60), "ON/OFF");
			GUI.color = Color.white;			
			
			//	GUI.enabled = startzigfu;		
			
			if(!startzigfu)
			{
				GUI.color = Color.green;
				if(GUI.Button (new Rect (60, 25, 80, 30), "Start"))
				{
					startKinect();
				}
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = Color.red;
				if(GUI.Button (new Rect (60, 25, 80, 30), "Stop"))
				{
					stopKinect();
				}	
				GUI.color = Color.white;
			}
			
			//	GUI.enabled = false;	//enable 			
			
			GUI.EndGroup ();	

			
		}//if kinect menu			

	}


	public static void sendULData()
	{
		//enable network
		if(!UDPData.flag)
		{
			UDPData.IP = "127.0.0.1";
			UDPData.port = 1202;
			UDPData.init();
			GeneralOptions.policyServer();//start policy server
			UDPData.flag=true;
			Debug.Log("Start UDP");
		}
	}

	public void startKinect()
	{
//		Debug.Log("enable");
		NuiWrapper.NuiInitialize(1);
		motorgui = true;
		smoothgui = false;	
		MainGuiControls.hideviewers = 0;			
		ZigSkeleton.mirror=true;				
		startzigfu = true;

//		this._Sensor1.Open();
		sendULData();
	}
	
	public void stopKinect()
	{
//		Debug.Log("disable");
		motorgui = false;
		smoothgui = true;
		MainGuiControls.hideviewers = 5000;	
		startzigfu = false;
		Zig.startflag=true;	
		NuiWrapper.NuiShutdown();
//		this._Sensor1.Close ();

		//stop logging kinect
		if(CheckUpdates.clinicVersion)
			stopLogKinectData();		
	}
	
	public void logKinectData()
	{
		if(!XmlDataWriter.selectedLogDev.Contains("KINECT"))
		{
			XmlDataWriter.selectedLogDev.Add("KINECT");
			
			XmlDataWriter.startLog=true;
		}
	}
	public void stopLogKinectData()
	{
		if(XmlDataWriter.selectedLogDev.Contains("KINECT"))
		{
			XmlDataWriter.selectedLogDev.Remove("KINECT");
			XmlDataWriter.capture = false;
			XmlDataWriter.startLog=false;
		}
	}

}
