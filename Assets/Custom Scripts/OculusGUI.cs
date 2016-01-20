using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class OculusGUI : MonoBehaviour {

	//csv log
	TextWriter file;
	
	//xml
	static XmlWriter writer;
	public static string timestamp, timestamp_old = String.Empty;
	static string date = String.Empty;
	string filepath = String.Empty;
	string filepath2 = String.Empty;
	public float uptime;
	public static bool startLog = false;
	bool islogging = false;
	
	public Texture2D backIcon;
	
	public Camera leftCam;
	public Camera rightCam;
	
	string DeviceName = string.Empty;

	private bool toggleCamera = false;

	// Use this for initialization
	void Awake() 
	{
		leftCam.enabled = false;
		rightCam.enabled = false;
	}
	
	void FixedUpdate () 
	{
		if(OVRDevice.IsHMDPresent())
		{
			oculusData();
		}
		
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
		
				//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "oculus";
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
		
		
		if(MainGuiControls.OculusMenu)
		{
			leftCam.camera.enabled = true;
			rightCam.camera.enabled = true;
		}
		if(!MainGuiControls.OculusMenu)
		{
			leftCam.camera.enabled = false;
			rightCam.camera.enabled = false;
		}
		if(!MainGuiControls.OculusMenu && toggleCamera)
		{
			leftCam.camera.enabled = true;
			rightCam.camera.enabled = true;
		}
		
//		Debug.Log("Cam: "+leftCam.transform.rotation.eulerAngles);
//		Debug.Log("Raw: "+OVRCamera.newOrientation);
		
	}
	
	void OnGUI()
	{
		if(MainGuiControls.OculusMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.OculusMenu = false;
				MainGuiControls.hideMenus = false;
			}
			
			if(OVRDevice.IsHMDPresent())
			{
				GUI.Label(new Rect (Screen.width/2-50, Screen.height/2-120, 100, 20), "x: "+OVRCamera.newOrientation.x.ToString());
				GUI.Label(new Rect (Screen.width/2-50, Screen.height/2-100, 100, 20), "y: "+OVRCamera.newOrientation.y.ToString());
				GUI.Label(new Rect (Screen.width/2-50, Screen.height/2-80, 100, 20), "z: "+OVRCamera.newOrientation.z.ToString());
			}

			toggleCamera = GUI.Toggle(new Rect(Screen.width/2-50, Screen.height/2, 100, 30), toggleCamera, "Keep Enabled");
			
		}//end if
		
	}//ongui
	
	
	void oculusData()
	{


		//accel
		if(!DevicesLists.availableDev.Contains("OCULUS:TRACKING:GYRO:ORIENTATION"))
		{
			DevicesLists.availableDev.Add("OCULUS:TRACKING:GYRO:ORIENTATION");		
		}
		if(DevicesLists.selectedDev.Contains("OCULUS:TRACKING:GYRO:ORIENTATION") && UDPData.flag==true)
		{					
		//	UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gyro, orientation,"+leftCam.transform.rotation.eulerAngles.x.ToString()+","+leftCam.transform.rotation.eulerAngles.y.ToString()+","+leftCam.transform.rotation.eulerAngles.z.ToString()+","+"0"+";"); 
			UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]gyro, orientation,"+OVRCamera.newOrientation.x.ToString()+","+OVRCamera.newOrientation.y.ToString()+","+OVRCamera.newOrientation.z.ToString()+";"); 
			Debug.Log(OVRCamera.newOrientation.x.ToString());
		}		
	}//oculusData
	
	
	void XMLInit()
	{
		islogging = true;

		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Oculus/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}

		//xml init
		if (XmlDataWriter.xml) {
			filepath2 = path + "OculusData_" + DateTime.Now.ToString ("yyyyMMddHHmmss") + ".xml";//save on Desktop
		
			XmlWriterSettings settings = new XmlWriterSettings ();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			writer = XmlWriter.Create (filepath2, settings);
			writer.WriteStartDocument ();
//		writer.WriteComment("Date: " + date);	//comment
			writer.WriteStartElement ("Log");
			InvokeRepeating ("XMLWrite", 1F, 0.03f);//32.25 Hz
		}

		//csv init
		if(XmlDataWriter.csv)
		{
			filepath = path + "Wii_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
			
			string header = "timestamp, uptime, "+
				"OrientationX, OrientationY, OrientationZ";
			file.WriteLine(header);
			file.Close();
			
			InvokeRepeating("csvWrite", 1F, 0.031F);//32.25 Hz
		}

		Debug.Log("Started Oculus Logging");
	}//xml init


	void csvWrite()
	{
		uptime+= Time.deltaTime;
		
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
		           OVRCamera.newOrientation.x.ToString() + "," + 
		           OVRCamera.newOrientation.y.ToString() + "," + 
		           OVRCamera.newOrientation.z.ToString());
		file.WriteLine("");
		file.Close();			
	}	
	
	void XMLWrite()
	{
		uptime+= Time.deltaTime;
		
		writer.WriteComment("Oculus Data");
		
		writer.WriteStartElement("UtcTime");
		writer.WriteAttributeString("mmssuuuu", timestamp);
		writer.WriteElementString("Uptime", uptime.ToString());		
			
		writer.WriteStartElement("Orientation");
		writer.WriteElementString("x", OVRCamera.newOrientation.x.ToString());
		writer.WriteElementString("y", OVRCamera.newOrientation.y.ToString());
		writer.WriteElementString("z", OVRCamera.newOrientation.z.ToString());
		writer.WriteEndElement();
		
		writer.WriteEndElement();//timestamp
	}
	
	void endXML()
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
		Debug.Log("Stoped Oculus Logging");
		
	}
	
	
}
