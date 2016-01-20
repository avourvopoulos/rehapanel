using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Threading;

public class WiiData : MonoBehaviour {

	//csv log
	TextWriter file;
	public static string timestamp;
	
	//xml
	static XmlWriter writer;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;
	
	//sensor data
	private int x;
	private int y;
	private int z;
	private float pitch;
	private float roll;
	private float yaw;
	private float nx;
	private float ny;
	private float nsx;
	private float nsy;
	private float irx;
	private float iry;
	
	public static bool startLog = false;
	bool islogging = false;
	
	string DeviceName = string.Empty;

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () 
	{
		x = -WiimoteTestController.x;
		y = -WiimoteTestController.y;
		z = -WiimoteTestController.z;
		
		pitch = WiimoteTestController.pitch;
		roll = WiimoteTestController.roll;
		yaw = WiimoteTestController.yaw;
		
		irx = WiimoteTestController.ir_x;
		iry = WiimoteTestController.ir_y;
		
		nx = WiimoteTestController.nx;
		ny = WiimoteTestController.ny;
		
		nsx = WiimoteTestController.nsx;
		nsy = WiimoteTestController.nsy;		
		
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
		
				//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "wii";
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		sendWiiData();
		
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
	
	void sendWiiData()
	{
		if (WiimoteTestController.wiimoteCount>0)//wii
		{	
			/* Tracking */
			
			//accel
			if(!DevicesLists.availableDev.Contains("WII:TRACKING:ACCELEROMETER:POSITION"))
			{
				DevicesLists.availableDev.Add("WII:TRACKING:ACCELEROMETER:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("WII:TRACKING:ACCELEROMETER:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]accelerometer,position,"+x.ToString()+","+y.ToString()+","+z.ToString()+";"); 
			}
			//pich, roll, yaw
			if(!DevicesLists.availableDev.Contains("WII:TRACKING:PITCHROLLYAW:ORIENTATION"))
			{
				DevicesLists.availableDev.Add("WII:TRACKING:PITCHROLLYAW:ORIENTATION");		
			}
			if(DevicesLists.selectedDev.Contains("WII:TRACKING:PITCHROLLYAW:ORIENTATION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]pitchrollyaw,orientation,"+pitch.ToString()+","+roll.ToString()+","+yaw.ToString()+";"); 
			}			
			//IR x,y
			if(!DevicesLists.availableDev.Contains("WII:TRACKING:IR:POSITION"))
			{
				DevicesLists.availableDev.Add("WII:TRACKING:IR:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("WII:TRACKING:IR:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]ir,position,"+irx.ToString()+","+iry.ToString()+";"); 
			}
			//Nunchuck Accel
			if(!DevicesLists.availableDev.Contains("WII:TRACKING:NUNCHUKACC:POSITION"))
			{
				DevicesLists.availableDev.Add("WII:TRACKING:NUNCHUKACC:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("WII:TRACKING:NUNCHUKACC:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]nunchukacc,position,"+nx.ToString()+","+ny.ToString()+";"); 
			}				
			//Nunchuck Stick
			if(!DevicesLists.availableDev.Contains("WII:TRACKING:NUNCHUKSTICK:POSITION"))
			{
				DevicesLists.availableDev.Add("WII:TRACKING:NUNCHUKSTICK:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("WII:TRACKING:NUNCHUKSTICK:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]nunchukstick,position,"+nsx.ToString()+","+nsy.ToString()+";"); 
			}
			
			
			/* Buttons */
			
			//Rumble
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:RUMBLE:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:RUMBLE:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:RUMBLE:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rumble,bool,"+WiimoteTestController.rumble.ToString()+";");
			//	Debug.Log("[$]button,[$$]wii,[$$$]rumble, bool,"+WiimoteTestController.rumble.ToString()+","+"0"+"0"+"0"+";");
			}
			
			//Button A
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:A:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:A:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:A:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]a,bool,"+WiimoteTestController.btnA.ToString()+";");
			//	Debug.Log("[$]button,[$$]wii,[$$$]a, bool,"+WiimoteTestController.btnA.ToString()+","+"0"+"0"+"0"+";");
			}
			//Button B
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:B:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:B:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:B:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]b,bool,"+WiimoteTestController.btnB.ToString()+";"); 
			}
			//Button 1
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:1:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:1:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:1:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]1,bool,"+WiimoteTestController.btn1.ToString()+";"); 
			}			
			//Button 2
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:2:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:2:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:2:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]2,bool,"+WiimoteTestController.btn2.ToString()+";"); 
			}						
			//Button Plus
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:PLUS:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:PLUS:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:PLUS:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]plus,bool,"+WiimoteTestController.btnPlus.ToString()+";"); 
			}				
			//Button Minus
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:MINUS:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:MINUS:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:MINUS:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]minus,bool,"+WiimoteTestController.btnMinus.ToString()+";"); 
			}				
			//Button Home
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:HOME:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:HOME:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:HOME:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]home,bool,"+WiimoteTestController.btnHome.ToString()+";"); 
			}		
			//Button Dpad Up
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:UP:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:UP:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:UP:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]up,bool,"+WiimoteTestController.upDpad.ToString()+";"); 
			}				
			//Button Dpad Down
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:DOWN:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:DOWN:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:DOWN:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]down,bool,"+WiimoteTestController.downDpad.ToString()+";"); 
			}				
			//Button Dpad Left
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:LEFT:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:LEFT:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:LEFT:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]left,bool,"+WiimoteTestController.leftDpad.ToString()+";"); 
			}							
			//Button Dpad Right
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:RIGHT:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:RIGHT:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:RIGHT:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]right,bool,"+WiimoteTestController.rightDpad.ToString()+";"); 
			}				
			//Button C
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:C:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:C:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:C:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]c,bool,"+WiimoteTestController.btnC.ToString()+";"); 
			}
			//Button Z
			if(!DevicesLists.availableDev.Contains("WII:BUTTON:Z:BOOL"))
			{
				DevicesLists.availableDev.Add("WII:BUTTON:Z:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("WII:BUTTON:Z:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]z,bool,"+WiimoteTestController.btnZ.ToString()+";"); 
			}			
			
		}//if wiimoteCount>0
		
	}//send wii data
	
	
	void logInit()
	{
		islogging = true;
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Wii/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml init
		if(XmlDataWriter.xml)
		{
			string filepath2 = path + "Wii_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
			
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
			filepath = path + "Wii_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
	
			string header = "timestamp, uptime, "+
							"Accelerometerx, Accelerometery, Accelerometerz,"+
							"pitch, roll, yaw,"+
							"IRx, IRy,"+
							"Nunchuky, Nunchuky,"+
							"NunchukStickx, NunchukSticky";
	        file.WriteLine(header);
	        file.Close();
		
			InvokeRepeating("csvWrite", 1F, 0.031F);//32.25 Hz
		}

		Debug.Log("Started Wii Logging");
	}//log init
	
	
	void csvWrite()
	{
		uptime+= Time.deltaTime;
		
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
					x.ToString() + "," + y.ToString() + "," + z.ToString() + "," +	
					pitch.ToString() + "," + roll.ToString() + "," + yaw.ToString() + "," +
					irx.ToString() + "," + iry.ToString() + "," +
					nx.ToString() + "," + ny.ToString() + "," +
					nsx.ToString() + "," + nsy.ToString());
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
		
		writer.WriteStartElement("Accelerometer");
		writer.WriteElementString("x", x.ToString());
		writer.WriteElementString("y", y.ToString());
		writer.WriteElementString("z", z.ToString());
		writer.WriteEndElement();
		
		writer.WriteStartElement("Orientation");
		writer.WriteElementString("pitch", pitch.ToString());
		writer.WriteElementString("roll", roll.ToString());
		writer.WriteElementString("yaw", yaw.ToString());
		writer.WriteEndElement();
		
		writer.WriteStartElement("IR");
		writer.WriteElementString("x", irx.ToString());
		writer.WriteElementString("y", iry.ToString());
		writer.WriteEndElement();
		
		writer.WriteStartElement("Nunchuk");
		writer.WriteElementString("x", nx.ToString());
		writer.WriteElementString("y", ny.ToString());
		writer.WriteEndElement();
		
		writer.WriteStartElement("NunchukStick");
		writer.WriteElementString("x", nsx.ToString());
		writer.WriteElementString("y", nsy.ToString());
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
		Debug.Log("Stoped Wii Logging");
	}
}
