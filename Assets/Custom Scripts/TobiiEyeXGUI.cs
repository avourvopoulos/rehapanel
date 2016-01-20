using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class TobiiEyeXGUI : MonoBehaviour {

	public Camera tobiieyeXCam;

	public Texture2D backIcon;

	//launching process
	string eyeServerURL = "";
	string programFiles = "";
	bool eyeServerExists = false;
	public static bool eyeServer = false;
	string processOutput; 
	List<string> inputData = new List<string>();
	List<string> errorMsg = new List<string>();
	Process process1 = null;//EyeTribe.exe
	StreamWriter messageStream;	
	
	string DeviceName = string.Empty;

	//csv log
	TextWriter file;
	public static string timestamp;
	public static bool startLog = false;
	bool islogging = false;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;


	// Use this for initialization
	void Awake () 
	{
		
		//check for CPU Arch and set programfiles folder path
		if(SystemDetails.is64Bit())
		{
			programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
		}
		else
		{
			programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		}
		
		eyeServerURL = programFiles+"\\Tobii\\Tobii EyeX\\Tobii.EyeX.Settings.exe";
		
	}


	void FixedUpdate () 
	{
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
		
	}

	// Update is called once per frame
	void Update () {
		
		if(MainGuiControls.TobiiEyeXMenu)
		{
			tobiieyeXCam.enabled = true;
		}
		else{
			tobiieyeXCam.enabled = false;
		}


		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "tobiieyex";
		}

		//start/stop logging
		if(startLog && !islogging)
		{
			logInit();
		}
		else if (!startLog && islogging)
		{
			endLog();
		}

		if(eyeServer)
			sendEyeXData();


	}

	void sendEyeXData()
	{

		//gazedisplay
		if(!DevicesLists.availableDev.Contains("TOBIIEYEX:TRACKING:GAZEDISPLAY:POSITION"))
		{
			DevicesLists.availableDev.Add("TOBIIEYEX:TRACKING:GAZEDISPLAY:POSITION");		
		}
		if(DevicesLists.selectedDev.Contains("TOBIIEYEX:TRACKING:GAZEDISPLAY:POSITION") && UDPData.flag==true)
		{					
			UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gazedisplay,position,"+GazePointVisualizer.gazeDisplay.x.ToString()+","+GazePointVisualizer.gazeDisplay.y.ToString()+";"); 
		}
		
		//gazeGUI
		if(!DevicesLists.availableDev.Contains("TOBIIEYEX:TRACKING:GAZEGUI:POSITION"))
		{
			DevicesLists.availableDev.Add("TOBIIEYEX:TRACKING:GAZEGUI:POSITION");		
		}
		if(DevicesLists.selectedDev.Contains("TOBIIEYEX:TRACKING:GAZEGUI:POSITION") && UDPData.flag==true)
		{					
			UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gazegui,position,"+GazePointVisualizer.gazeGUI.x.ToString()+","+GazePointVisualizer.gazeGUI.y.ToString()+";"); 
		}

		//gazeScreen
		if(!DevicesLists.availableDev.Contains("TOBIIEYEX:TRACKING:GAZESCREEN:POSITION"))
		{
			DevicesLists.availableDev.Add("TOBIIEYEX:TRACKING:GAZESCREEN:POSITION");		
		}
		if(DevicesLists.selectedDev.Contains("TOBIIEYEX:TRACKING:GAZESCREEN:POSITION") && UDPData.flag==true)
		{					
			UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gazescreen,position,"+GazePointVisualizer.gazeScreen.x.ToString()+","+GazePointVisualizer.gazeScreen.y.ToString()+";"); 
		}

		//gazeViewport
		if(!DevicesLists.availableDev.Contains("TOBIIEYEX:TRACKING:GAZEVIEWPORT:POSITION"))
		{
			DevicesLists.availableDev.Add("TOBIIEYEX:TRACKING:GAZEVIEWPORT:POSITION");		
		}
		if(DevicesLists.selectedDev.Contains("TOBIIEYEX:TRACKING:GAZEVIEWPORT:POSITION") && UDPData.flag==true)
		{					
			UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gazeviewport,position,"+GazePointVisualizer.gazeViewport.x.ToString()+","+GazePointVisualizer.gazeViewport.y.ToString()+";"); 
		}
		
	}

	void logInit()
	{
		islogging = true;
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Tobii/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml init
		if(XmlDataWriter.xml)
		{

		}
		
		//csv init
		if(XmlDataWriter.csv)
		{
			filepath = path + "EyeX_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
			
			string header = "timestamp, uptime, "+
				"gazedisplayX, gazedisplayY,"+
					"gazeguiX, gazeguiX,"+
					"gazescreenX, gazescreenY,"+
					"gazeviewportX, gazeviewportY";
			file.WriteLine(header);
			file.Close();
			
			InvokeRepeating("csvWrite", 1F, 0.016F);//62.5 Hz
		}
		
		UnityEngine.Debug.Log("Started EyeX Logging");
	}//log init

	void csvWrite()
	{
		uptime+= Time.deltaTime;
		//
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
		           GazePointVisualizer.gazeDisplay.x.ToString()+","+GazePointVisualizer.gazeDisplay.y.ToString()+"," +		
		           GazePointVisualizer.gazeGUI.x.ToString()+","+GazePointVisualizer.gazeGUI.y.ToString()+"," +	
		           GazePointVisualizer.gazeScreen.x.ToString()+","+GazePointVisualizer.gazeScreen.y.ToString()+"," +
		           GazePointVisualizer.gazeViewport.x.ToString()+","+GazePointVisualizer.gazeViewport.y.ToString());
		file.WriteLine("");
		file.Close();			
	}	

	void endLog()
	{
		if(XmlDataWriter.xml)
		{		
	
		}
		
		//csv
		if (XmlDataWriter.csv)
		{
			uptime = 0;
			CancelInvoke("csvWrite");
			islogging = false;
			file.Close();
			file.Dispose();
		}		
		UnityEngine.Debug.Log("Stoped EyeX Logging");
	}

	void OnGUI()
	{
		if(MainGuiControls.TobiiEyeXMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.TobiiEyeXMenu = false;
				MainGuiControls.hideMenus = false;
			}


			//Launch buttons
			GUI.enabled = !eyeServer;//enable gui if URL exists
	
				GUI.color = Color.green;
				if (GUI.Button (new Rect (20, Screen.height / 2+100, 200, 30), "Launch Tobii Server"))
				{
					launchTobiiServer(); 
				}
				GUI.color = Color.white;
		
//				GUI.color = Color.red;
//				if (GUI.Button (new Rect (20, Screen.height / 2+100, 200, 30), "Stop Tobii Server"))
//				{
//					killTobiiServer();
//				}
//				GUI.color = Color.white;

			GUI.enabled = true;		


			GUI.Label(new Rect(20, 110, 300, 20), "Gaze Display: " +GazePointVisualizer.gazeDisplay);
			GUI.Label(new Rect(20, 130, 300, 20), "Gaze GUI: " +GazePointVisualizer.gazeGUI);
			GUI.Label(new Rect(20, 150, 300, 20), "Gaze Screen: " +GazePointVisualizer.gazeScreen);
			GUI.Label(new Rect(20, 170, 300, 20), "Gaze Viewport: " +GazePointVisualizer.gazeViewport);
		
			
		}

		
	}


	//start TobiiEyeX
	void launchTobiiServer()
	{
		try
		{
			process1 = new Process();
			//process1.EnableRaisingEvents = false;
			process1.StartInfo.FileName = eyeServerURL;
			//process1.StartInfo.UseShellExecute = false;
			process1.StartInfo.CreateNoWindow = true;	
			process1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			process1.Start();
			//process1.BeginOutputReadLine();
			
			//messageStream = process1.StandardInput;
			eyeServer = true;
			inputData.Add("launching eyeServer client...");
			#if UNITY_EDITOR
			UnityEngine.Debug.Log( "Successfully launched eyeServer" );
			#endif
		}
		catch( Exception e )
		{
			errorMsg.Add("Unable to launch eyeServer: " + e.Message);
			eyeServer = false;
			#if UNITY_EDITOR
			UnityEngine.Debug.LogError( "Unable to launch eyeServer: " + e.Message );
			#endif
		}
	}
	
	
	void DataReceived( object sender, DataReceivedEventArgs eventArgs )
	{
		#if UNITY_EDITOR
		UnityEngine.Debug.Log( eventArgs.Data );
		#endif
		processOutput = eventArgs.Data;
		inputData.Add(processOutput);
	}
	
	
	void ErrorReceived( object sender, DataReceivedEventArgs eventArgs )
	{
		#if UNITY_EDITOR
		UnityEngine.Debug.LogError( eventArgs.Data );
		#endif
		errorMsg.Add(eventArgs.Data);
		processOutput = eventArgs.Data;
		eyeServer = false;
	}
	
	
	//kill TobiiServer
	void killTobiiServer()
	{
		process1.CloseMainWindow();
		process1.Kill();
		eyeServer = false;
		inputData.Add("closing eyeServer...");
		print("closing eyeServer...");
	}
	
	
	void OnApplicationQuit()
	{
		if( process1 != null && !process1.HasExited )
		{
			process1.CloseMainWindow();
			//          process1.Kill();
		}		
	}	


}
