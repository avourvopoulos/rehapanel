using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Threading;
using LSL;

public class KeyMouseEvents : MonoBehaviour {

	// LSL - create stream info and outlet
//	static liblsl.StreamInfo info;
//	static liblsl.StreamOutlet outlet;
//	static float[] data;
	
	//csv log
	TextWriter file;
	string filepath = String.Empty;	
	
	//xml log
	static XmlWriter writer;
	public static string timestamp = String.Empty;
	static string date = String.Empty;
	public float uptime;
	public static bool startLog = false;
	bool islogging = false;
	
	public string display;
	string keyClick;
	string mouseClick;
	
	//keyboard
	float horizontalAxis = 0.0f;
	float verticalAxis = 0.0f;

	float mouseX;
	float mouseY;
	
	Vector3 mousePos;
	
	// Use this for initialization
	void Start () 
	{
		// LSL - create stream info and outlet
//		info = new liblsl.StreamInfo("mouse", "tracking", 2, 60, liblsl.channel_format_t.cf_float32, "rehabnet");
//		outlet = new liblsl.StreamOutlet(info);
//		data = new float[2];


		System.Type kType = typeof(KeyCode);
	}
	
	void FixedUpdate () 
	{	
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
	}
	
	// Update is called once per frame
	void Update () 
	{

		//LSL - send mouse data
//		data [0] = mouseX;
//		data [1] = mouseY;
//		outlet.push_sample (data);


		//Debug.Log (keyClick.ToLower ());

		horizontalAxis = -Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
		
		mouseX = -Input.mousePosition.x;
		mouseY = Input.mousePosition.y;
		
		if(!DevicesLists.availableDev.Contains("MOUSE:TRACKING:CURSOR:POSITION"))
		{
			DevicesLists.availableDev.Add("MOUSE:TRACKING:CURSOR:POSITION");
			DevicesLists.availableDev.Add("MOUSE:BUTTON:CLICK:EVENT");
			DevicesLists.availableDev.Add("KEYBOARD:BUTTON:CLICK:EVENT");
			DevicesLists.availableDev.Add("KEYBOARD:TRACKING:AXIS:EVENT");
		}	
		if(DevicesLists.selectedDev.Contains("KEYBOARD:BUTTON:CLICK:EVENT") && UDPData.flag==true)
		{
			if(Input.GetKey(KeyCode.LeftArrow))
			{
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"leftarrow"+",event,1;");
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"rightarrow"+",event,0;");
				UDPData.sendString ("[$]button,[$$]" + DevicesLists.newDevice + ",[$$$]vrpn,both," + "-1" + "," + "10" + ";");

			}
			else if(Input.GetKey(KeyCode.RightArrow))
			{
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"leftarrow"+",event,"+"0"+";");
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"rightarrow"+",event,"+"1"+";");
				UDPData.sendString ("[$]button,[$$]" + DevicesLists.newDevice + ",[$$$]vrpn,both," + "1" + "," + "01" + ";");
			}
			else{
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"leftarrow"+",event,"+"0"+";");
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"rightarrow"+",event,"+"0"+";");
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+keyClick.ToLower()+",event,"+"1"+";");
				UDPData.sendString ("[$]button,[$$]" + DevicesLists.newDevice + ",[$$$]vrpn,both," + "0" + "," + "00" + ";");
			}

//			if(keyClick != string.Empty)
//			{
////				if(keyClick.ToLower() == "leftarrow"){
////					UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"leftarrow"+",event,"+"1"+";");
////					UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"rightarrow"+",event,"+"0"+";");
////				}
////				else if(keyClick.ToLower() == "rightarrow"){
////					UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"leftarrow"+",event,"+"0"+";");
////					UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+"rightarrow"+",event,"+"1"+";");
////				}
////				else{
////					UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+keyClick.ToLower()+",event,"+"1"+";");
//
//				}
//	
//			}
//			else{
//			UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+keyClick.ToLower()+",event,"+"0"+";");
//
//			}
		}
		if(DevicesLists.selectedDev.Contains("MOUSE:BUTTON:CLICK:EVENT") && UDPData.flag==true)
		{
			if(mouseClick != string.Empty){
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+mouseClick.ToLower()+",event,"+"1"+";"); 
			}
			else{
				UDPData.sendString("[$]button,[$$]"+DevicesLists.newDevice+",[$$$]"+mouseClick.ToLower()+",event,"+"0"+";");
			}
		}		
		if(DevicesLists.selectedDev.Contains("MOUSE:TRACKING:CURSOR:POSITION") && UDPData.flag==true)
		{					
				UDPData.sendString("[$]tracking,[$$]"+DevicesLists.newDevice+",[$$$]cursor,position,"+mouseX.ToString()+","+mouseY.ToString()+";"); 
		}
		if(DevicesLists.selectedDev.Contains("KEYBOARD:TRACKING:AXIS:EVENT") && UDPData.flag==true)
		{					
				UDPData.sendString("[$]tracking,[$$]"+DevicesLists.newDevice+",[$$$]axis,position,"+horizontalAxis+","+verticalAxis+";"); 
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
	
	void OnGUI()
	{
		Event e = Event.current;
        if (e.isKey)
		{
			keyClick = e.keyCode.ToString();
//			display = e.keyCode.ToString();
        //    Debug.Log("Detected key code: " + e.keyCode);
		}
		else{keyClick = string.Empty;}
		
		if (e.isMouse)
		{
			mouseClick = e.button.ToString();
//			display = e.button.ToString();
		//	Debug.Log("Detected mouse key: " + e.button);
		}
		else{mouseClick = string.Empty;}
	//	else {display = " ";}
		
		#if UNITY_EDITOR
//		GUI.Label(new Rect(Screen.width/2-30, Screen.height-50, 120, 100), "Keys: " + display);
//		GUI.Label(new Rect(Screen.width/2-60, Screen.height-50, 120, 100), horizontalAxis.ToString()+" , " + verticalAxis.ToString());
		#endif
	}
	
	
	void logInit()
	{
		islogging = true;
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Mouse_Keyboard/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml init
		if(XmlDataWriter.xml)
		{
			string filepath2 = path + "MouseKeyboard_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
			
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
			filepath = path + "MouseKeyboard_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
	
			string header = "timestamp, uptime, "+
							"Mousex, Mousey, MouseClicks,"+
							"KeybHorizontalAxis, KeybVerticalAxis, KeyboardClick";
	
	        file.WriteLine(header);
	        file.Close();
		
			InvokeRepeating("csvWrite", 1F, 0.031F);//32.25 Hz
		}
		
		Debug.Log("Started Mouse_Keyboard Logging");
		
	}//log init
	
	void csvWrite()
	{
		uptime+= Time.deltaTime;
		
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
					mouseX.ToString() + "," + mouseY.ToString() + "," + mouseClick + "," +	
					horizontalAxis.ToString() + "," + verticalAxis.ToString() + "," + keyClick);
        file.WriteLine("");
		file.Close();			
	}
	
	void XMLWrite()
	{
		uptime+= Time.deltaTime;
		
		writer.WriteComment("Wii Data");
		
		writer.WriteStartElement("UtcTime");
		writer.WriteAttributeString("mmssuuuu", timestamp);
		writer.WriteElementString("Uptime", uptime.ToString());		
		
		writer.WriteStartElement("Mouse");
		writer.WriteElementString("x", mouseX.ToString());
		writer.WriteElementString("y", mouseY.ToString());
		writer.WriteElementString("mouseClick",mouseClick);
		writer.WriteEndElement();
		
		writer.WriteStartElement("Keyboard");
		writer.WriteElementString("horizontalAxis", horizontalAxis.ToString());
		writer.WriteElementString("verticalAxis", verticalAxis.ToString());
		writer.WriteElementString("keyboardClick",keyClick);
		writer.WriteEndElement();
		
		writer.WriteEndElement();//timestamp
  
	}
	

	
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
		
		Debug.Log("Stoped Mouse_Keyboard Logging");
	}
	
	
}
