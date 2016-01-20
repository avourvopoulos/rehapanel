using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class KinectSettings : MonoBehaviour {
	
	public GameObject ZigFuz;
	public GameObject ZigInputContainerz;
	
	
	
	string longWord = "20"; //-27 to 27
//	 float longWord = 20.0f; 
    public static int angle;
	
	bool readingAngle = false;
    bool SeatedMode = true;
    bool TrackSkeletonInNearMode = false;
    bool NearMode = false;
    private Thread t;
	
	float Smoothing = 0.0f; 
	float Correction = 0.0f; 
	float Prediction = 0.0f; 
	float JitterRadius = 0.0f; 
	float MaxDeviationRadius = 0.0f; 
	
	
	private bool auto = false;
	private bool microsoftsdk = true;
	private bool openni = false;
	
	private bool capture = true;
	
	// Use this for initialization
	void Start () 
	{
        Debug.Log("Kinect specific comopnent started");
		ZigFuz = GameObject.Find("ZigFu");
		ZigInputContainerz = GameObject.Find("ZigInputContainer");
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	 
	
    static void setAngle()
    {       
        long a = (long)angle;              
        NuiWrapper.NuiCameraElevationSetAngle(a);
        Thread.Sleep(0);   
    }

    
    static int getAngle()
	{
        long angleOut;    
        NuiWrapper.NuiCameraElevationGetAngle(out angleOut);
        return (int)angleOut;
    }
    
	
    void OnGUI()
    {
		
GUI.enabled = microsoftsdk;
		
		GUI.BeginGroup (new Rect (Screen.width / 2 - 500, Screen.height / 2 - 320, 200, 230));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,230), "Motor Control");
		GUI.color = Color.white;
		
        longWord = GUI.TextField(new Rect(10, 25, 100, 30), readingAngle ? getAngle().ToString() : longWord, 20);
	//	longWord = GUI.VerticalSlider (new Rect (25, 205, 100, 130), longWord, 27.0f, -27.0f); //slide bar
        
		if (GUI.Button(new Rect(140, 30, 20, 20), ">")&& int.Parse(longWord)<= 27)
        {
			 int newangle = int.Parse(longWord)+1;
			longWord = newangle.ToString();
		}
		if (GUI.Button(new Rect(120, 30, 20, 20), "<")&& int.Parse(longWord)>= -27)
        {
			int newangle = int.Parse(longWord)-1;
			longWord = newangle.ToString();
		}
		
        if (GUI.Button(new Rect(10, 60, 100, 30), "Set Angle") && int.Parse(longWord)<= Math.Abs(27))
        {
            
            angle = int.Parse(longWord);
	//		angle = (int)longWord;
            NuiWrapper.NuiCameraElevationSetAngle(angle);
            t = new Thread(setAngle);    //attempted a Paramaterized Thread to no avail       
            t.Start();
            Thread.Sleep(0);           
         
        }

        readingAngle = GUI.Toggle(new Rect(10, 100, 100, 30), readingAngle, "Read Angle");      
		
		GUI.color = Color.yellow;
		 GUI.Label(new Rect(70, 130, 100, 25), "Settings"); // section title
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
		
       GUI.EndGroup (); // end settings group
		
		
			// Smoothing Param Group
		GUI.BeginGroup (new Rect (Screen.width / 2 - 500, Screen.height / 2 - 80, 200, 340));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,340), "Smoothing Parameters");
		GUI.color = Color.white;
		//	KinectSDKSmoothingParameters
	
		NuiWrapper.NuiTransformSmoothParameters smoothingParam = new NuiWrapper.NuiTransformSmoothParameters();
		
		 GUI.Label(new Rect(10, 35, 100, 20), "Smoothing");
		Smoothing = GUI.HorizontalSlider (new Rect (10, 60, 100, 20), Smoothing, 0.0f, 1.0f);
		GUI.Label(new Rect(120, 55, 100, 20), Smoothing.ToString());
		smoothingParam.Smoothing = Smoothing;
		
		 GUI.Label(new Rect(10, 85, 100, 20), "Correction");
		Correction = GUI.HorizontalSlider (new Rect (10, 110, 100, 20), Correction, 0.0f, 1.0f);
		GUI.Label(new Rect(120, 105, 100, 20), Correction.ToString());
		smoothingParam.Correction = Correction;
		
		 GUI.Label(new Rect(10, 135, 100, 20), "Prediction");
		Prediction = GUI.HorizontalSlider (new Rect (10, 160, 100, 20), Prediction, 0.0f, 1.0f);
		GUI.Label(new Rect(120, 155, 100, 20), Prediction.ToString());
		smoothingParam.Prediction = Prediction;
		
		 GUI.Label(new Rect(10, 185, 100, 20), "Jitter Radius");
		JitterRadius = GUI.HorizontalSlider (new Rect (10, 210, 100, 20), JitterRadius, 0.0f, 1.0f);
		GUI.Label(new Rect(120, 205, 100, 20), JitterRadius.ToString());
		smoothingParam.JitterRadius = JitterRadius;
		
		 GUI.Label(new Rect(10, 235, 160, 20), "Max Deviation Radius");
		MaxDeviationRadius = GUI.HorizontalSlider (new Rect (10, 260, 100, 20), MaxDeviationRadius, 0.0f, 1.0f);
		GUI.Label(new Rect(120, 255, 100, 20), MaxDeviationRadius.ToString());
		smoothingParam.MaxDeviationRadius = JitterRadius;
		
		if (GUI.Button (new Rect (10,295,40,25), "Min"))
		{
			 Smoothing = 0.0f; 
			 Correction = 0.0f; 
			 Prediction = 0.0f; 
			 JitterRadius = 0.0f; 
			 MaxDeviationRadius = 0.0f; 
		}
		if (GUI.Button (new Rect (60,295,40,25), "Med"))
		{
			 Smoothing = 0.5f; 
			 Correction = 0.5f; 
			 Prediction = 0.5f; 
			 JitterRadius = 0.5f; 
			 MaxDeviationRadius = 0.5f; 	
		}
		if (GUI.Button (new Rect (110,295,40,25), "Max"))
		{
		     Smoothing = 1.0f; 
			 Correction = 1.0f; 
			 Prediction = 1.0f; 
			 JitterRadius = 1.0f; 
			 MaxDeviationRadius = 1.0f; 
			
		}
		
		GUI.EndGroup (); //end smoothing group
		
GUI.enabled = true;	
		
	
		//Test GUI
		GUI.BeginGroup (new Rect (Screen.width / 2 +300, Screen.height / 2 + 120, 200, 100));
		GUI.color = Color.red;
		GUI.Box (new Rect (0,0,200,100), "Capture Data");
		GUI.color = Color.white;
		
GUI.enabled = capture;	
		if (GUI.Button (new Rect (30,40,60,30), "Start"))
		{
			capture=false;
		}
  GUI.enabled = true;	
		
GUI.enabled = !capture;	
		if (GUI.Button (new Rect (100,40,60,30), "Stop"))
		{
			capture=true;
		}
  GUI.enabled = true;	
		
		GUI.EndGroup ();
		
		//Test GUI ToP
		GUI.BeginGroup (new Rect (10, 10, Screen.width-20, 50));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,Screen.width-20,50), "Kinect");
		GUI.color = Color.white;		
		GUI.EndGroup ();
		
		//Enable/Disable Kinect
		GUI.BeginGroup (new Rect (Screen.width / 2- 190 , Screen.height / 2 + 100, 400, 270));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,400,270), "Kinect SDK");
		GUI.color = Color.white;
		if (GUI.Button (new Rect (200,240,60,30), "Disable"))
		{
			Debug.Log("disable");
			ZigFuz.SetActive(false);
			ZigInputContainerz.SetActive(false);
		}
		if (GUI.Button (new Rect (120,240,60,30), "Enable"))
		{
			Debug.Log("enable");
			ZigFuz.SetActive(true);
			ZigInputContainerz.SetActive(true);
		}
		
		GUI.EndGroup ();
		
		
		// Driver Group
		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 160, 235, 100));
		
		GUI.Box (new Rect (0,0,235,100), "Driver");
			
		if (auto = GUI.Toggle (new Rect (10, 25, 45, 30), auto, "Auto"))
			{
			  microsoftsdk = false;
			  openni = false;
		//	ZigInput.InputType = ZigInputType.Auto;
		//	Debug.Log("Auto: "+ZigInput.InputType);
			//	if (ZigInputType.Auto == )
			}
			
		if (microsoftsdk = GUI.Toggle (new Rect (60, 25, 100, 30), microsoftsdk, "Microsoft SDK"))
			{
			  auto = false;
			  openni = false;
	//		ZigInput.InputType = ZigInputType.KinectSDK;
	//		Debug.Log("KinectSDK: "+ZigInput.InputType);
			}
			
		if (openni = GUI.Toggle (new Rect (165, 25, 80, 30), openni, "Open NI"))
			{
			  microsoftsdk = false;
			  auto = false;
	//		ZigInput.InputType = ZigInputType.OpenNI;
	//		Debug.Log("OpenNI: "+ZigInput.InputType);
			}			
	
		GUI.EndGroup (); //end Driver group
	
		
		
    }// Gui
	
	
	
	  
	
}
