using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
//using Microsoft.Kinect;
using Windows.Kinect;

public class MainGuiControls : MonoBehaviour {
	
	// Window Always-on-top when docked
//	[DllImport("user32.dll")]
//	static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
//	
//	static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
//	static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
//	const UInt32 SWP_NOSIZE = 0x0001;
//	const UInt32 SWP_NOMOVE = 0x0002;
//	const UInt32 SWP_SHOWWINDOW = 0x0040;
//	const UInt32 FLAGS = SWP_NOMOVE | SWP_NOSIZE;

	public GUIStyle guistyle;
	public GUIStyle guistyleMainbar;

	private KinectSensor _Sensor;
	
	public GameObject Model;
	
    public AudioClip buttonsound;
	
	public Camera WiiCam;
	public Camera MyoCam;

	public GUIStyle tooltipStyle;
	public GUIStyle mainMenuStyle;
	public GUIStyle plainStyle;
	
	//textures
	public Texture2D exiticon;
	public Texture2D lablogo;
	Texture dockstatus;
	public GUIStyle style;
	
	public Texture2D deviceIcon;
	public Texture2D dataIcon;
	public Texture2D optionsIcon;
	
	public Texture2D homeIcon;
	public Texture2D backIcon;
	public Texture2D dockIcon;
	public Texture2D undockIcon;
	
	//devices icons
	public Texture2D kinect2Icon;
	public Texture2D kinectIcon;
	public Texture2D kinectIconMini;
	public Texture2D emotivIcon;
	public Texture2D myomoIcon;
	public Texture2D neuroskyIcon;
	public Texture2D tobiiIcon;
	public Texture2D vuzixIcon;
	public Texture2D wiiIcon;
	public Texture2D oculusIcon;
	public Texture2D tetIcon;
	public Texture2D BCI2000Icon;
	public Texture2D LeapMotionIcon;
	public Texture2D tobiieyexIcon;
	public Texture2D myoIcon;
	public Texture2D faceapiIcon;
	public Texture2D openbciIcon;
	public Texture2D bitalinoIcon;
	
	public Texture2D inDataIcon;
	public Texture2D outDataIcon;
	public Texture2D logIcon;
	public Texture2D emuIcon;
	

	
	public Texture2D greenIcon;
	public Texture2D redIcon;
	
	//cameras
	public GameObject CamKinect2Model;
	public Camera CamKinectModel;
	public Camera CamNetModel;
	public Camera CameraUp;
	public Camera CameraHead;
	public Camera BitalinoCam;
	public Camera MainCamera;
	
	//menu hide
	public static int hideMenu = 1;
	public static int myomomenu = 1000;
	public static int emotivmenu = 1000;
	public static int optionsmenu = 1000;
	public static int vrpnmenu = 1000;
	public static int miscmenu = 1000;
		
	public static int hideviewers = 5000;
	
	private bool capture = true;
//	public static bool writeXml = false;
	public static bool writeXml {get; set;}
	public static bool endXml = false;
	private bool mutesound = false;
	public static bool dock = false;//docked window
	
	public static int menuindex = 1;//menu index nnumber for docked mode
	

//	public static bool logALL=false;//log all data toggle button
	
	bool showlogo=false;
	
	bool fullscreen=false;
	
	public static bool DeviceMenu = false;
	public static bool DataMenu = false;
	public static bool OptionsMenu = false;
	public static bool GamesMenu = false;
	
	public static bool KinectMenu = false;
	public static bool Kinect2Menu = false;
	public static bool EmotivMenu = false;
	public static bool MyomoMenu = false;
	public static bool NeuroskyMenu = false;
	public static bool TobiiMenu = false;
	public static bool VuzixMenu = false;
	public static bool WiiMenu = false;
	public static bool OculusMenu = false;
	public static bool thirdPartyMenu = false;
	public static bool TETMenu = false;
	public static bool TobiiEyeXMenu = false;
	public static bool LeapMotionMenu = false;
	public static bool MyoMenu = false;
	public static bool faceapiMenu = false;
	public static bool openbciMenu = false;
	public static bool bitalinoMenu = false;

	public static bool BCIAPE = false;
	public static bool TrainSession = false;
	public static bool LiveSession = false;

	public static bool NetMenu = false;
	public static bool LogMenu = false;
	public static bool EmuMenu = false;
	public static bool VisMenu = false;

	public static bool hideMenus = false;

	public Vector2 scrollPosition = Vector2.zero;

//	private KinectSensor sensor;
	
	
	//place window always-on-top
//	public void SetOnTop()
//	{
//	    SetWindowPos(HWND_TOPMOST, HWND_TOPMOST, 0, 0, 0, 0, FLAGS);
//	}
//	public void UnSetOnTop()
//	{
//	    SetWindowPos(HWND_NOTOPMOST, HWND_NOTOPMOST, 0, 0, 0, 0, FLAGS);
//	} 	
	
	//Awake is called when the script instance is being loaded
	

	void Start()
	{
		if (HandlePrefs.minWindow == true) {
			dockWindow();
		}

		BitalinoCam.enabled = false;

	}

	void Update () 
	{
		if (SystemDetails.win8) {
			//Debug.Log("windows 8");	
			try
			{
				_Sensor = KinectSensor.GetDefault();
			}
			catch
			{
				Debug.LogError("Cannot init KinectSensor");
			}

		} else {
			_Sensor = null;
		//	Debug.Log("NOT 8");	
				}


		if(KinectMenu && !CameraSwitch.avatarView)
		{
			CamKinectModel.enabled = true;
			CamNetModel.enabled = true;		
			CameraUp.enabled = false;
			CameraHead.enabled = false;
		}
		else
		{
			CamKinectModel.enabled = false;
			CamNetModel.enabled = false;
			
		}
		
		if(WiiMenu)//enable/disable wii camera
		{
			WiiCam.enabled = true;
		}
		else
		{
			WiiCam.enabled = false;
		}

		if(MyoMenu)//enable/disable Myo Camera
		{
			MyoCam.enabled = true;
		}
		else
		{
			MyoCam.enabled = false;
		}
		
	}
	 
	void dockWindow(){
		dock = true;//docked window 
		Screen.SetResolution (1024, 180, false);
		CamKinect2Model.SetActive (false);
		Debug.Log("DOCKED");
		}

	void undockWindow(){
		dock = false;
		Screen.SetResolution (1024, 720, false);
		homeScreen();
		Debug.Log("UN-DOCKED");
	}
	
    void OnGUI()
    {
		
		/******************************************************************
		 * New Menu
		 * ****************************************************************
		 * */
		
//		GUI.Box (new Rect (10,10,Screen.width-20,Screen.height-20), "");//background box
//		GUI.Box (new Rect (10,10,Screen.width-20,60), "", guistyleMainbar);//menubar box
		GUI.Box (new Rect (10,10,Screen.width-20,60), "");//menubar box
		
		if(dock && UDPData.flag)
		{
			GUI.Label(new Rect (Screen.width/2-110 , 100 , 280,60), "Sending Data: "+UDPData.IP+" - "+"Port: "+ UDPData.port);
		}
		
		if(dock)
		{	//undocked
			if (GUI.Button (new Rect (Screen.width/2-30 , 10 ,60,60), undockIcon))//undock button
			{
				undockWindow();
//				UnSetOnTop();//unset always-on-top
			}
		}
		else
		{	//home button
			if (GUI.Button (new Rect (Screen.width/2-30 , 10 ,60,60), homeIcon))//home button
			{
				homeScreen();
				CamKinect2Model.SetActive (false);
				CameraSwitch.avatarView = false;
				hideMenus = false;

			}			
		}
		
		//dock button
		if(hideMenus == false && !dock && DeviceMenu == DataMenu == OptionsMenu == !GamesMenu == !fullscreen == false)
		{
			if (GUI.Button (new Rect (Screen.width/2-30 , Screen.height-70 ,60,60), dockIcon))//dock button
			{
				if(!dock)
				{
					dockWindow();
//					SetOnTop();//set always-on-top
				}	
			}	
		}
		
		//network arrows
		if(UDPData.flag)//outgoing data
		{
			if (GUI.Button (new Rect (Screen.width-110 , 20 ,25,25), outDataIcon, plainStyle))//outgoing data button
			{
				homeScreen();
				CamKinect2Model.SetActive (false);
				NetMenu = true;
				hideMenus = true;
				DataMenu = true;

				if(dock)
				{
				Screen.SetResolution (1024, 720, false);
				dock = false;
				}
			}
		}
		//incoming data
		if(UDPReceive.isConnected || ReceiveVRPN.started)
		{
			if (GUI.Button (new Rect (Screen.width-130 , 20 ,25,25), inDataIcon, plainStyle))//incoming data button
			{
				homeScreen();
				CamKinect2Model.SetActive (false);
				NetMenu = true;
				hideMenus = true;
				DataMenu = true;

				if(dock)
				{
				Screen.SetResolution (1024, 720, false);
				dock = false;
				}
			}
		}
		//VRPN
		if(ReceiveVRPN.started)
		{
			GUI.Label(new Rect(Screen.width-180, 20, 100, 20), "VRPN");
		}

		//logging data
		if(XmlDataWriter.capture)//logging data
		{
			if (GUI.Button (new Rect (Screen.width-170 , 15 ,30,30), logIcon, plainStyle))//log data button
			{
				homeScreen();
				CamKinect2Model.SetActive (false);
				LogMenu = true;
				hideMenus = true;
				DataMenu = true;

				if(dock)
				{
				Screen.SetResolution (1024, 720, false);
				dock = false;
				}
			}
		}
		//emulation
		if(EmulateData.emulate || EmulateData.btnemulate)//emulation
		{
			if (GUI.Button (new Rect (Screen.width-190 , 20 ,30,20), emuIcon, plainStyle))//log data button
			{
				homeScreen();
				CamKinect2Model.SetActive (false);
				EmuMenu = true;
				hideMenus = true;
				DataMenu = true;

				if(dock)
				{
				Screen.SetResolution (1024, 720, false);
				dock = false;
				}
			}
		}		
		
			/* ******************************
			 * active devices/services
			 * ******************************
		 	*/

		//if (SystemDetails.win8){//if windows 8
			if (_Sensor != null) //kinect 2
			{
				if (_Sensor.IsAvailable && _Sensor.IsOpen)
				{
				GUI.color = Color.green;//if user tracked then green	
				//else
				//GUI.color = Color.red;//else red

				if (GUI.Button (new Rect (20 , 20 ,80,30), kinect2Icon, plainStyle))
				{
					
					homeScreen();
					Kinect2Menu = true;
					DeviceMenu = true;
					hideMenus = true;
					CamKinect2Model.SetActive(true);

					if(dock)
					{
						dock = false;
						Screen.SetResolution (1024, 720, false);
					}
				}	
				}
				GUI.color = Color.white;
			}
			if(KinectGUI.startzigfu)//kinect
			{
				if(Zig.trackeduser==true)
				{GUI.color = Color.green;}//if user tracked then green
				else{GUI.color = Color.red;}//else red
				if (GUI.Button (new Rect (20 , 40 ,80,30), kinectIconMini, plainStyle))
				{
					homeScreen();
					DeviceMenu = true;
					KinectMenu = true;
					hideMenus = true;
					CamKinect2Model.SetActive(false);
					if(dock)
					{
						dock = false;
						Screen.SetResolution (1024, 720, false);
					}
				}
				GUI.color = Color.white;
			}
		//}//end if windows 8
		else{
			if(KinectGUI.startzigfu)//kinect
			{
				if(Zig.trackeduser==true)
				{GUI.color = Color.green;}//if user tracked then green
				else{GUI.color = Color.red;}//else red
					if (GUI.Button (new Rect (20 , 20 ,80,30), kinectIconMini, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						KinectMenu = true;
						hideMenus = true;
						
						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}
		}
			if (EmoEngineInst.IsStarted)//emotiv
			{
				if(EmoUserManagement.numUser > 0)
				{GUI.color = Color.green;}//if user tracked then green
				else{GUI.color = Color.red;}//else red
				if (GUI.Button (new Rect (100 , 20 ,80,30), emotivIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						EmotivMenu = true;
						hideMenus = true;

						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}



		if (OpenBCIConnection.isConnected)//openBCI
		{
			if(!OpenBCIConnection.paused)
			{GUI.color = Color.green;}//if user tracked then green
			else{GUI.color = Color.red;}//else red
			if (GUI.Button (new Rect (100 , 20 ,80,30), openbciIcon, plainStyle))
			{
				homeScreen();
				DeviceMenu = true;
				openbciMenu = true;
				hideMenus = true;
				
				if(dock)
				{
					dock = false;
					Screen.SetResolution (1024, 720, false);
				}
			}
			GUI.color = Color.white;
		}




			if (WiimoteTestController.wiimoteCount>0)//wii
			{
				GUI.color = Color.green;
				if (GUI.Button (new Rect (180 , 20 ,80,30), wiiIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						WiiMenu = true;
						hideMenus = true;

						if(dock)
						{				
						dock = false;
						Screen.SetResolution (1024, 720, false);		
						}
					}
				GUI.color = Color.white;
			}		
			if (MyomoConnection.sp.IsOpen)//myomo
			{
				GUI.color = Color.green;
				if (GUI.Button (new Rect (260 , 10 ,70,50), myomoIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						MyomoMenu = true;
						hideMenus = true;

						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}
			if (Vuzix.trackerStatus)//vuzix
			{
				GUI.color = Color.green;
				if (GUI.Button (new Rect (340 , 20 ,80,30), vuzixIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						VuzixMenu = true;
						hideMenus = true;

						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}
			if (OVRDevice.IsHMDPresent())//oculus
			{
				GUI.color = Color.green;
				if (GUI.Button (new Rect (340 , 20 ,80,30), oculusIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						OculusMenu = true;
						hideMenus = true;
						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}		
			if (TobiiData.trackerStatus)//Tobii
			{
				if(TobiiData.eyeStatus)
				{GUI.color = Color.green;}//if eye found
				else{GUI.color = Color.red;}//else red
				if (GUI.Button (new Rect (340 , 20 ,50,50), tobiiIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						TobiiMenu = true;
						hideMenus = true;

						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);
						}
					}
				GUI.color = Color.white;
			}	
			if (TETSettings.eyeServer)//TET
			{
				if(TETSettings.isConnected)
				{GUI.color = Color.green;}//if eye found
				else{GUI.color = Color.red;}//else red
				if (GUI.Button (new Rect (280 , 40 ,120,140), tetIcon, plainStyle))
					{
						homeScreen();
						DeviceMenu = true;
						TETMenu = true;
						hideMenus = true;

						if(dock)
						{
						dock = false;
						Screen.SetResolution (1024, 720, false);		
						}
					}
				GUI.color = Color.white;
			}	
			if (BCI2000.data != string.Empty)//bci2000
			{
				if (GUI.Button (new Rect (100 , 20 ,45,45), BCI2000Icon, plainStyle))
				{
					homeScreen();
					DeviceMenu = true;
					LaunchApps.bci2000 = true;
					hideMenus = true;
					
					if(dock)
					{
					dock = false;
					Screen.SetResolution (1024, 720, false);
					}
				}
				
			}
		if (ThinkGearGUI.neuroskyConnected)//neurosky
		{
			GUI.color = Color.green;
			if (GUI.Button (new Rect (100 , 15 ,70,50), neuroskyIcon, plainStyle))
			{
				homeScreen();
				DeviceMenu = true;
				NeuroskyMenu = true;
				hideMenus = true;
				
				if(dock)
				{
					dock = false;
					Screen.SetResolution (1024, 720, false);
				}
			}
			GUI.color = Color.white;
		}
		if (DisconnectionNotice.isConnected)//leap motion
		{
			GUI.color = Color.green;
			if (GUI.Button (new Rect (100 , 20 ,45,45), LeapMotionIcon, plainStyle))
			{
				homeScreen();
				DeviceMenu = true;
				LeapMotionMenu = true;
				hideMenus = true;
				
				if(dock)
				{
					dock = false;
					Screen.SetResolution (1024, 720, false);
				}
			}
			GUI.color = Color.white;	
		}
		if (TobiiEyeXGUI.eyeServer)//Tobii EYEX
		{
			GUI.color = Color.green;//if eye found
			if (GUI.Button (new Rect (280 , 40 ,120,140), tobiieyexIcon, plainStyle))
			{
				homeScreen();
				DeviceMenu = true;
				TobiiEyeXMenu = true;
				hideMenus = true;
				
				if(dock)
				{
					dock = false;
					Screen.SetResolution (1024, 720, false);		
				}
			}
			GUI.color = Color.white;
		}	

		if (ThalmicMyo.myoSynced)//Myo
		{
			GUI.color = Color.green;//if eye found
			if (GUI.Button (new Rect (260 , 10 ,70,50), myoIcon, plainStyle))
			{
				homeScreen();
				DeviceMenu = true;
				MyoMenu = true;
				hideMenus = true;
				
				if(dock)
				{
					dock = false;
					Screen.SetResolution (1024, 720, false);		
				}
			}
			GUI.color = Color.white;
		}	
		
		//main menu
		if (DeviceMenu == DataMenu == OptionsMenu == !GamesMenu == !dock == false)
		{
			
			if(CheckUpdates.releaseVersion)
			{
			GUI.Label(new Rect(Screen.width/2+160 , Screen.height/2-120, 20, 20), redIcon);//Options
			}
			
			GUI.Label(new Rect(Screen.width/2-280 , Screen.height/2-160, 120, 25), "Devices", mainMenuStyle);
			if (GUI.Button (new Rect (Screen.width/2-340 , Screen.height/2-120 ,200,200), deviceIcon))
			{
				DeviceMenu = true;
			}
			GUI.Label(new Rect(Screen.width/2-10 , Screen.height/2-160, 120, 25), "Data", mainMenuStyle);
			if (GUI.Button (new Rect (Screen.width/2-90 , Screen.height/2-120 ,200,200), dataIcon))
			{
				DataMenu = true;
			}
			
	GUI.enabled = !CheckUpdates.releaseVersion;		
			
			GUI.Label(new Rect(Screen.width/2+220 , Screen.height/2-160, 120, 25), "Options", mainMenuStyle);
			if (GUI.Button (new Rect (Screen.width/2+160 , Screen.height/2-120 ,200,200), optionsIcon))
			{
				OptionsMenu = true;
			}
	GUI.enabled = true;		
			
			if(CheckUpdates.internet)
			{
				if (GUI.Button(new Rect((Screen.width/2)-60, Screen.height/2+160, 120, 30), "Online Apps"))
				{
					GamesMenu = true;
				}
			}
		
		}
		
		
		if(DeviceMenu && !hideMenus)
		{
//			scrollPosition = GUI.BeginScrollView(new Rect(20, 10, 1000, 850), scrollPosition, new Rect(0, 0, 1000, 950));

			//kinematics
//			GUI.Label(new Rect (110,Screen.height/2-270, 180, 20), "Kinematics");
			GUI.Box (new Rect (60 ,Screen.height/2-250,190,310), "Kinematics");
//			if (SystemDetails.win8){//if windows 8

			if (GUI.Button (new Rect (80 , Screen.height/2-220 ,150,50), new GUIContent(kinectIcon, "Microsoft                  Kinect v1.0")))
				{
					KinectMenu = true;
					hideMenus = true;
				}
			GUI.enabled = SystemDetails.win8;//enable only on win8
				if (GUI.Button (new Rect (80 , Screen.height/2-150 ,150,50), new GUIContent(kinect2Icon, "Microsoft                Kinect v2.0")))
				{
					Kinect2Menu = true;
					hideMenus = true;
					CamKinect2Model.SetActive(true);
				}
			GUI.enabled = true;
//			}//end if windows 8
//			else{
//				if (GUI.Button (new Rect (80 , Screen.height/2-220 ,150,150), new GUIContent(kinectIcon, "Microsoft                  Kinect")))
//				{
//					KinectMenu = true;
//					hideMenus = true;
//				}
//			}
			if (GUI.Button (new Rect (80 , Screen.height/2-80 ,150,50), new GUIContent(wiiIcon, "Nintendo                       Wii Remote")))
			{
				WiiMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (80 , Screen.height/2-10 ,150,50), new GUIContent(LeapMotionIcon, "Leap                       Motion")))
			{
				LeapMotionMenu = true;
				hideMenus = true;
			}

			
//			//Third Party Apps
//			GUI.Box (new Rect (60 ,Screen.height/2+320,190,70), "");
//			if (GUI.Button (new Rect (75 , Screen.height/2+330 ,160,50), "Third Party Applications"))
//			{
//				thirdPartyMenu = true;
//				hideMenus = true;
//			}


			//enable menus for Dev. version
			GUI.enabled = !CheckUpdates.releaseVersion;
			
			//Electrophysiological
//			GUI.Label(new Rect (Screen.width/2-175,Screen.height/2-270, 180, 20), "Electrophysiological");
			GUI.Box (new Rect (Screen.width/2-215 ,Screen.height/2-250,190,450), "Electrophysiological");
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2-220 ,150,50), new GUIContent(emotivIcon, "Emotiv                           EPOC")))
			{
				EmotivMenu = true;
				hideMenus = true;
			}			
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2-150 ,150,50), new GUIContent(neuroskyIcon, "NeuroSky                       Mindwave")))
			{
				NeuroskyMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2-80 ,150,50), new GUIContent(myoIcon, "Thalmic                          Myo")))
			{
				MyoMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2-10 ,150,50), new GUIContent(myomoIcon, "Myomo                          mpower 1000")))
			{
				MyomoMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2+60 ,150,50), new GUIContent(openbciIcon, "OpenBCI                      v1")))
			{
				openbciMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2-195 , Screen.height/2+130 ,150,50), new GUIContent(bitalinoIcon, "PLUX                         bitalino")))
			{
				bitalinoMenu = true;
				BitalinoCam.enabled = true;
				hideMenus = true;
			}
			
			//Eye Tracking
//			GUI.Label(new Rect (Screen.width/2+75,Screen.height/2-270, 180, 20), "Eye Tracking");
			GUI.Box (new Rect (Screen.width/2+25,Screen.height/2-250,190,240), "Eye Tracking");
			if (GUI.Button (new Rect (Screen.width/2+45, Screen.height/2-220 ,150,50), new GUIContent(tobiiIcon, "Tobii		T120")))
			{
				TobiiMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2+45, Screen.height/2-150 ,150,50), new GUIContent(tobiieyexIcon, "Tobii	  	EyeX")))
			{
				TobiiEyeXMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2+45, Screen.height/2-80 ,150,50), new GUIContent(tetIcon, "Eye Tribe	  	ET1000")))
			{
				TETMenu = true;
				hideMenus = true;
			}	
			
			//Head Tracking
//			GUI.Label(new Rect (Screen.width/2+305,Screen.height/2-270, 180, 20), "Head Tracking");
			GUI.Box (new Rect (Screen.width/2+255,Screen.height/2-250,190,240), "Head Tracking");
			if (GUI.Button (new Rect (Screen.width/2+275, Screen.height/2-220 ,150,50), new GUIContent(oculusIcon, "Oculus		 Rift")))
			{
				OculusMenu = true;
				hideMenus = true;
			}	
			if (GUI.Button (new Rect (Screen.width/2+275, Screen.height/2-150 ,150,50), new GUIContent(vuzixIcon, "Vuzix	                VR920")))
			{
				VuzixMenu = true;
				hideMenus = true;
			}
			if (GUI.Button (new Rect (Screen.width/2+275, Screen.height/2-80 ,150,50), new GUIContent(faceapiIcon, "face	                API")))
			{
				faceapiMenu = true;
				hideMenus = true;
			}	
			
	GUI.enabled = true;		
			
			//tooltip
			GUI.Label(new Rect(Screen.width/2-150, 20, 100, 40), GUI.tooltip, tooltipStyle); //gui tooltip//ijd;pii/vdeGUI.Label(new Rect(10, 40, 100, 40), GUI.tooltip);UI.Label(new Rect(10, 40, 100, 40), GUI.tooltip); ff

//			GUI.EndScrollView();


			//Third Party Apps
//			GUI.Box (new Rect (Screen.width/2+255 ,Screen.height/2+90,190,70), "");
////			GUI.color = Color.yellow;
//			if (GUI.Button (new Rect (Screen.width/2+275 , Screen.height/2+100 ,150,50), "Third Party Applications"))
//			{
//				thirdPartyMenu = true;
//				hideMenus = true;
//			}
//			GUI.color = Color.white;
		}
		
		
		//green-red icons
		if(CheckUpdates.releaseVersion && DeviceMenu && !hideMenus)
		{
		//	GUI.Label(new Rect(180 , Screen.height/2-220 , 20, 20), greenIcon);//kinect
		//	GUI.Label(new Rect(180 , Screen.height/2-50 , 20, 20), greenIcon);//wii
			
		//	GUI.Label(new Rect(60 ,Screen.height/2+140, 20, 20), redIcon);//third party
			GUI.Label(new Rect(Screen.width/2-195 , Screen.height/2-220, 20, 20), redIcon);//emotiv
			GUI.Label(new Rect(Screen.width/2-195 , Screen.height/2-50, 20, 20), redIcon);//neurosky
			GUI.Label(new Rect(Screen.width/2-195 , Screen.height/2+120, 20, 20), redIcon);//myomo	
			GUI.Label(new Rect(Screen.width/2+45, Screen.height/2-220, 20, 20), redIcon);//tobii
			GUI.Label(new Rect(Screen.width/2+275, Screen.height/2-220, 20, 20), redIcon);//oculus
			GUI.Label(new Rect(Screen.width/2+275, Screen.height/2-50, 20, 20), redIcon);//vuzix
			GUI.Label(new Rect(Screen.width/2+45, Screen.height/2-50, 20, 20), redIcon);//TET
		}
		
		

		//about page
		if (GUI.Button (new Rect (Screen.width-190 , Screen.height-100 ,150,150), lablogo, style))
		{
			About.activeWin=true;
		}	

		
 }// GUI
	

	/*
	 * Return to home screen
	 * */
	void homeScreen()
	{
		DeviceMenu = false;
		DataMenu = false;
		OptionsMenu = false;
		GamesMenu = false;
		
		KinectMenu = false;
		Kinect2Menu = false;
//		CamKinect2Model.SetActive (false);
		EmotivMenu = false;
		MyomoMenu = false;
		NeuroskyMenu = false;
		TobiiMenu = false;
		OculusMenu = false;
		VuzixMenu = false;
		WiiMenu=false;
		WiiCam.enabled = false;
		TETMenu=false;
		thirdPartyMenu=false;
		LaunchApps.bci2000 = false;
		MyoMenu = false;
		openbciMenu = false;
		faceapiMenu = false;
		bitalinoMenu = false;
		BitalinoCam.enabled = false;

		NetMenu = false;
		LogMenu = false;
		EmuMenu = false;
		VisMenu = false;

		TobiiEyeXMenu = false;
		LeapMotionMenu = false;

		BCIAPE = false;
		TrainSession = false;
		LiveSession = false;

		CameraSwitch.avatarView = false;

		MainCamera.enabled = true;
	}
	
	


	
}
