using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class TETSettings : MonoBehaviour {
	
	public Texture backIcon;
	
	public GameObject Calibration;
	public GameObject Gaze;
	
	public Camera calibrationCamera;
	public Camera gazeCamera;
	
	public static Vector2 gazeCoords;
	public static double eyeAngle;
	
	public static bool isConnected;
	
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
	Vector2 scrollPosition1 = Vector2.zero;//
	
	string DeviceName = string.Empty;
	public static bool startLog = false;
	bool islogging = false;

	//xml
	static XmlWriter writer;
	public static string timestamp, timestamp_old = String.Empty;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;

	//csv log
	TextWriter file;
	//public static string timestamp;
	
	
	// Use this for initialization
	void Awake () 
	{
//		calibrationCamera.enabled=false;
//		gazeCamera.enabled=false;
//		
		Gaze.SetActive(false);
		
		//check for CPU Arch and set programfiles folder path
		if(SystemDetails.is64Bit())
		{
			programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
		}
		else
		{
			programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		}
		
		eyeServerURL = programFiles+"\\EyeTribe\\Server\\EyeTribe.exe";
		
	}
	
	void Start () 
	{
		//check if directory exists
		if(File.Exists(eyeServerURL)) 
        {
			eyeServerExists = true;
		}
		else{eyeServerExists = false;}
	}
	
	void FixedUpdate () 
	{
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec

	}

	// Update is called once per frame
	void Update () 
	{
		sendTETData();
		
		//start/stop logging
//		if(startLog && !islogging)
//		{
//			XMLInit();
//		}
//		else if (!startLog && islogging)
//		{
//			endXML();
//		}		
		
		if(MainGuiControls.TETMenu)
		{
			if(Calibration.activeInHierarchy)
			{
				calibrationCamera.enabled=true;
				gazeCoords = Vector2.zero;
				eyeAngle = 0;
			}
			if(Gaze.activeInHierarchy)
			{
				gazeCamera.enabled=true;
			}
		}
		else{
			if(Calibration.activeInHierarchy)
			calibrationCamera.enabled=false;
			if(Gaze.activeInHierarchy)
			gazeCamera.enabled=false;			
		}
		
		//console window
		if(inputData.Count>1000)
		{
			inputData.Clear();
		}
		
		//check if process is running
		if(eyeServer)
		{
			if( process1 == null || process1.HasExited )
	        {
				eyeServer = false;
				inputData.Clear();
				errorMsg.Add("Launch failed!");
			}
		}		
		
		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "eyetribe";
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
	}
	
	void sendTETData()
	{
		if (gazeCoords != Vector2.zero)//tet
		{	
			/* Tracking */
			float gazeX = -gazeCoords.x;
			float gazeY = -gazeCoords.y;
			
			//centergazepoint
			if(!DevicesLists.availableDev.Contains("EYETRIBE:TRACKING:CENTERGAZEPOINT:POSITION"))
			{
				DevicesLists.availableDev.Add("EYETRIBE:TRACKING:CENTERGAZEPOINT:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("EYETRIBE:TRACKING:CENTERGAZEPOINT:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]centergazepoint,position,"+gazeX.ToString()+","+gazeY.ToString()+";"); 
			}
			
			//eye angle
			if(!DevicesLists.availableDev.Contains("EYETRIBE:TRACKING:EYEANGLE:ORIENTATION"))
			{
				DevicesLists.availableDev.Add("EYETRIBE:TRACKING:EYEANGLE:ORIENTATION");		
			}
			if(DevicesLists.selectedDev.Contains("EYETRIBE:TRACKING:EYEANGLE:ORIENTATION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]eyeangle,orientation,"+eyeAngle.ToString()+";"); 
			}			
		}
	}
	
	void OnGUI() 
	{
		if(MainGuiControls.TETMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.TETMenu = false;
				MainGuiControls.hideMenus = false;
			}
			
			GUI.Label(new Rect(20, 150, 400, 20), "Gaze Coordinates: "+gazeCoords.x.ToString("0.0") +" , "+gazeCoords.y.ToString("0.0"));
			GUI.Label(new Rect(20, 180, 100, 20), "Eye Angle: "+eyeAngle.ToString("0.0"));
			
			//Launch buttons
			GUI.enabled = eyeServerExists;//enable gui if URL exists
			if(!eyeServer)
			{
				GUI.color = Color.green;
	         	if (GUI.Button (new Rect (20, Screen.height / 2+100, 200, 30), "Launch Eye Server"))
				{
					launchEyeServer(); 
				}
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = Color.red;
	         	if (GUI.Button (new Rect (20, Screen.height / 2+100, 200, 30), "Stop Eye Server"))
				{
					killEyeServer();
				}
				GUI.color = Color.white;
			}
			GUI.enabled = true;		
			
			//console window
//			GUI.color = Color.yellow;
//			GUI.Label(new Rect(Screen.width/2 - 70, Screen.height/2 - 225, 400, 20), "Console Output:");
//			GUI.color = Color.white;
//			float yOffset = 0.0f;
//			GUI.Box(new Rect(Screen.width/2 - 260, Screen.height/2 - 200, 470, 300), " ");
//			scrollPosition1 = GUI.BeginScrollView(new Rect(Screen.width/2 - 250, Screen.height/2 - 200, 460, 290), scrollPosition1, new Rect(0, 0, 300, 300+(inputData.Count*10)));			
//			foreach (string indt in inputData)//input data
//			{
//				GUI.Label(new Rect(5, 10+ yOffset, 450, 20), indt);
//				yOffset += 25;
//			}
//			foreach (string err in errorMsg)//error msg's
//			{
//				GUI.color = Color.red;
//				GUI.Label(new Rect(5, 10+ yOffset, 450, 20), err);
//				GUI.color = Color.white;
//				yOffset += 25;
//			}			
//			GUI.EndScrollView();			
			
		}//tetmenu
	}//gui
	
	
	//start EyeServer
	void launchEyeServer()
    {
        try
        {
            process1 = new Process();
            //process1.EnableRaisingEvents = false;
            process1.StartInfo.FileName = eyeServerURL;
            //process1.StartInfo.UseShellExecute = false;
			process1.StartInfo.CreateNoWindow = true;	
			process1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			
			//process1.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            //process1.StartInfo.RedirectStandardOutput = true;
            //process1.StartInfo.RedirectStandardInput = true;
            //process1.StartInfo.RedirectStandardError = true;
            //process1.OutputDataReceived += new DataReceivedEventHandler( DataReceived );
            //process1.ErrorDataReceived += new DataReceivedEventHandler( ErrorReceived );
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

 
	//kill EyeServer
	void killEyeServer()
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


	void logInit()
	{
		islogging = true;
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/EyeTribe/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml init
		if(XmlDataWriter.xml)
		{
			string filepath2 = path + "EyeTribe_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			writer = XmlWriter.Create(filepath2,settings);
			writer.WriteStartDocument();
			writer.WriteStartElement("Log");
			
			InvokeRepeating("XMLWrite", 1F, 0.03f);//32.25 Hz
		}
		
		//csv init
		if(XmlDataWriter.csv)
		{
			filepath = path + "EyeTribe_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
			
			string header = "timestamp, uptime, "+
				"gazeX, gazeY, eyeAngle";
			file.WriteLine(header);
			file.Close();
			
			InvokeRepeating("csvWrite", 1F, 0.031F);//32.25 Hz
		}
		
	//	Debug.Log("Started TET Logging");
	}//log init
	
	
	void csvWrite()
	{
		uptime+= Time.deltaTime;

		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
		gazeCoords.x.ToString("0.0") + "," + gazeCoords.y.ToString("0.0") + "," + eyeAngle.ToString("0.0") );
		file.WriteLine("");
		file.Close();			
	}	
	
	
//	void XMLInit()
//	{
//		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/TET/";
//		if(!Directory.Exists(path))
//		{
//			System.IO.Directory.CreateDirectory(path);
//		}
//		filepath = path + "TETData_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
//		
//		XmlWriterSettings settings = new XmlWriterSettings();
//		settings.Indent = true;
//		settings.NewLineOnAttributes = true;
//		writer = XmlWriter.Create(filepath,settings);
//		writer.WriteStartDocument();
////		writer.WriteComment("Date: " + date);	//comment
//		writer.WriteStartElement("Log");
//		islogging = true;
//		InvokeRepeating("XMLWrite", 1F, 0.03f);//32.25 Hz
//
//		UnityEngine.Debug.Log("Started TET Logging");
//	}//xml init
	
	
	void XMLWrite()
	{
		uptime+= Time.deltaTime;
		
		writer.WriteComment("EyeTribe Data");
		
		writer.WriteStartElement("UtcTime");
		writer.WriteAttributeString("mmssuuuu", timestamp);
		writer.WriteElementString("Uptime", uptime.ToString());		
		
		writer.WriteStartElement("CenterGazePoint");
		writer.WriteElementString("x", gazeCoords.x.ToString("0.0"));
		writer.WriteElementString("y", gazeCoords.y.ToString("0.0"));
		writer.WriteEndElement();
		
		writer.WriteStartElement("EyeAngle");
		writer.WriteElementString("Degrees", eyeAngle.ToString("0.0"));
		writer.WriteEndElement();
		
		writer.WriteEndElement();//timestamp
	}
	
//	void endXML()
//	{
//		CancelInvoke("XMLWrite");
//		islogging = false;
//		writer.WriteEndElement();//log
//		writer.WriteComment("End of Log");
//		writer.WriteEndDocument();
//		writer.Flush();
//		writer.Close();		
//		UnityEngine.Debug.Log("Stoped TET Logging");
//	}


	void endLog()
	{
		if(XmlDataWriter.xml)
		{		
			CancelInvoke("XMLWrite");
			islogging = false;
			writer.WriteEndElement();//log
			writer.WriteComment("End of Log");
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();		
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
//		Debug.Log("Stoped tet Logging");
	}
	
	
	
}
