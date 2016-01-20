using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class TobiiData : MonoBehaviour {
	
	public Camera TobiiCam;
	public Texture backIcon;
	
	float eyeGazeX;
	float eyeGazeY;
	
	//xml
	static XmlWriter writer;
	public static string timestamp, timestamp_old = String.Empty;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;
	public static bool startLog = false;
	bool islogging = false;
	
	EyeTracking eye;
	Calibration cal;
	MainGuiControls maingui;
	
	public static bool trackerStatus;
	public static bool eyeStatus;
	public static bool calibStatus;
	
	string DeviceName = string.Empty;
	
	bool addDataToList = true;
	
	void Awake ()
	{
		TobiiCam.enabled = false;
	}
	
	void Start () 
	{
		eye = GetComponent<EyeTracking>();
		cal = GetComponent<Calibration>();
		maingui = GetComponent<MainGuiControls>();
	}
	
	void FixedUpdate () 
	{	
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
		
		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "tobii";
		}
		
		eyeGazeX = -eye.CenterGazePoint.x;
		eyeGazeY = eye.CenterGazePoint.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//tracker status
		if (eye.IsConnected){
			trackerStatus = true;}
		else{
			trackerStatus = false;}
		//calibration status
		if (cal.IsCalibrationdone){
			calibStatus = true;}
		else{
			calibStatus = false;} 
		//eye status
		if (eye.NoEyeFound){
			eyeStatus = false;}
		else{
			eyeStatus = true;} 		
		
		//start/stop logging
		if(startLog && !islogging)
		{
			XMLInit();
		}
		else if (!startLog && islogging)
		{
			endXML();
		}		
		
		//enable/disable camera
		if(MainGuiControls.TobiiMenu)
		{
			TobiiCam.enabled=true;
		}
		else
		{
			TobiiCam.enabled=false;
		}
		
		if(eye.IsConnected)
		{
			if(!DevicesLists.availableDev.Contains("TOBII:TRACKING:CENTERGAZEPOINT:POSITION"))
			{
				DevicesLists.availableDev.Add("TOBII:TRACKING:CENTERGAZEPOINT:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("TOBII:TRACKING:CENTERGAZEPOINT:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]centergazepoint,position,"+eyeGazeX.ToString()+","+eyeGazeY.ToString()+";"); 
			}	
			
			//add joint to list and send via udp
			if(addDataToList)
			{
				//add data to list
				foreach(string adev in DevicesLists.availableDev)
				{
					if(adev.Contains("TOBII") && !DevicesLists.selectedDev.Contains(adev))
					{
						DevicesLists.selectedDev.Add(adev);
					}
				}
			addDataToList=false;	
			
			}
			
		}
	}
	
	void OnGUI() 
	{
		if(MainGuiControls.TobiiMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.TobiiMenu = false;
				MainGuiControls.hideMenus = false;
			}
		
			#if UNITY_EDITOR
			GUI.Label(new Rect (Screen.width/2-110 , 100 , 300,60), " "+eye.CenterGazePoint.x.ToString()+","+eye.CenterGazePoint.y.ToString());
			#endif
		}
	}
	
	
	void XMLInit()
	{
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Tobii/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		filepath = path + "TobiiData_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
		
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.NewLineOnAttributes = true;
		writer = XmlWriter.Create(filepath,settings);
		writer.WriteStartDocument();
//		writer.WriteComment("Date: " + date);	//comment
		writer.WriteStartElement("Log");
		islogging = true;
		InvokeRepeating("XMLWrite", 1F, 0.015f);//66 Hz

		Debug.Log("Started Tobii Logging");
	}//xml init
	

	void XMLWrite()
	{
		uptime+= Time.deltaTime;
		
		writer.WriteComment("Tobii Data");
		
		writer.WriteStartElement("UtcTime");
		writer.WriteAttributeString("mmssuuuu", timestamp);
		writer.WriteElementString("Uptime", uptime.ToString());		
			
		writer.WriteStartElement("GazePointPosition");
		writer.WriteElementString("x", eyeGazeX.ToString());
		writer.WriteElementString("y", eyeGazeY.ToString());
		writer.WriteEndElement();
		
		writer.WriteEndElement();//timestamp
	}
	
	void endXML()
	{
		CancelInvoke("XMLWrite");
		islogging = false;
		writer.WriteEndElement();//log
		writer.WriteComment("End of Log");
		writer.WriteEndDocument();
		writer.Flush();
		writer.Close();		
		Debug.Log("Stoped Tobii Logging");
	}	
	
	
}
