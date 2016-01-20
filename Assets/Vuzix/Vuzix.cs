using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Runtime.InteropServices;
using System.IO;
using DWORD = System.UInt32;

public class Vuzix : MonoBehaviour {
	
		//xml
	static XmlWriter writer;
	public static string timestamp, timestamp_old = String.Empty;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;
	public static bool startLog = false;
	bool islogging = false;
	
	public Camera cam;
	
	public Texture2D backIcon;
	
	Vector3 vHead;
	
	int yaw, pitch, roll = 0 ;
	float maxRotation = 32768;
	public float smoothness = 100;
	public static bool trackerStatus = false;
	
	public static bool vuzixtoggle = false;
	
	string DeviceName = string.Empty;
	
// Our wrapper which receives the gyroscope information (yaw, pitch, roll) from the Vuzix iWear VR920.
public class VRWrapper
	{
	[DllImport("iWearWrapper")]
    public static extern char WrapIWROpenTracker();

    [DllImport("iWearWrapper")]
    public static extern char WrapIWRGetTracking(ref int yaw, ref int pitch, ref int roll);
	
	[DllImport("iWearWrapper")]
    public static extern void WrapIWRCloseTracker();	
	}

	// Use this for initialization
	void Awake () 
	{
		cam.enabled=false;
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
			DeviceName = "vuzix";
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
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
		if(MainGuiControls.VuzixMenu)
		{
			cam.enabled=true;
		}
		else
		{
			cam.enabled=false;
		}
		
		if (trackerStatus)
		{
			// Retrieve the laster gyroscope information.
        	VRWrapper.WrapIWRGetTracking(ref yaw, ref pitch, ref roll);
			
			
			if(!DevicesLists.availableDev.Contains("VUZIX:TRACKING:GYRO:ORIENTATION"))
			{
				DevicesLists.availableDev.Add("VUZIX:TRACKING:GYRO:ORIENTATION");		
			}
			if(DevicesLists.selectedDev.Contains("VUZIX:TRACKING:GYRO:ORIENTATION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gyro, orientation,"+vHead.x.ToString()+","+vHead.y.ToString()+","+vHead.z.ToString()+";"); 
			//	Debug.Log("[$]tracking,[$$]vuzix,[$$$]head, orientation,"+vHead.x.ToString()+","+vHead.y.ToString()+","+vHead.z.ToString()+","+"0"+";");
			}	
				
		}
		
		// Change the rotation of the head according to the gyroscope information.
		cam.transform.eulerAngles = new Vector3(-(pitch / maxRotation) * 180, -((yaw / maxRotation) * 180 ), (roll / maxRotation) * 180 );
		cam.transform.eulerAngles = Vector3.Slerp(cam.transform.eulerAngles, new Vector3(-(pitch/maxRotation)*180, -((yaw/maxRotation)*180), (roll/maxRotation)*180 ), Time.deltaTime*smoothness);
		
		vHead = Vector3.Slerp(cam.transform.eulerAngles, new Vector3(-(pitch/maxRotation)*180, -((yaw/maxRotation)*180), (roll/maxRotation)*180 ), Time.deltaTime*smoothness);
	
		
	}
	
	void OnGUI() 
	{
		if(MainGuiControls.VuzixMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.VuzixMenu = false;
				MainGuiControls.hideMenus = false;
			}			
			
			GUI.BeginGroup (new Rect (20-KinectGUI.gone, (Screen.height/2 -210), 200, 220));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,220), "Vuzix iWear VR920");
			GUI.color = Color.white;
	
	       	GUI.Label(new Rect(20, 90, 300, 20), "pitch: " + vHead.x);
			GUI.Label(new Rect(20, 120, 300, 20), "yaw : " + vHead.y);
			GUI.Label(new Rect(20, 150, 300, 20), "roll : " + vHead.z);
			
			
			GUI.enabled = !trackerStatus;
			if (GUI.Button(new Rect(20, 30, 100, 20), "Open Tracker"))
			{
				VRWrapper.WrapIWROpenTracker();
				trackerStatus=true;
	            Debug.Log("Open");
			}
			GUI.enabled = true;
	        
			GUI.enabled = trackerStatus;
	        if (GUI.Button(new Rect(20, 60, 100, 20), "Close Tracker"))
			{
				VRWrapper.WrapIWRCloseTracker();
				trackerStatus = false;
	            Debug.Log("Closed");
			}
			GUI.enabled = true;
			
			GUI.EndGroup ();
			
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
		}//if vuzix menu
    }//gui
	
	
	void XMLInit()
	{
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Vuzix/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		filepath = path + "VuzixData_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
		
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.NewLineOnAttributes = true;
		writer = XmlWriter.Create(filepath,settings);
		writer.WriteStartDocument();
//		writer.WriteComment("Date: " + date);	//comment
		writer.WriteStartElement("Log");
		islogging = true;
		InvokeRepeating("XMLWrite", 1F, 0.03f);//32.25 Hz

		Debug.Log("Started Vuzix Logging");
	}//xml init
	
	
	
	void XMLWrite()
	{
		uptime+= Time.deltaTime;
		
		writer.WriteComment("Vuzix Data");
		
		writer.WriteStartElement("UtcTime");
		writer.WriteAttributeString("mmssuuuu", timestamp);
		writer.WriteElementString("Uptime", uptime.ToString());		
			
		writer.WriteStartElement("Orientation");
		writer.WriteElementString("pitch", vHead.x.ToString());
		writer.WriteElementString("yaw", vHead.y.ToString());
		writer.WriteElementString("roll", vHead.z.ToString());
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
		Debug.Log("Stoped Vuzix Logging");
	}
	
	
	
}
