using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Threading;
  
public class XmlDataWriter : MonoBehaviour 
{	
	public Texture csvTex;
	public Texture xmlTex;
	
	public Texture2D backIcon;
	public Texture2D arrowsIcon;
	public Vector2 scrollPosition = Vector2.zero;//scrolling window
	public Vector2 scrollPosition2 = Vector2.zero;//scrolling window
	
//	public Texture2D rec;
//	public Texture2D recOff;
	private bool blink = false;
	private int counter = 0;
	public int blinkSpeed = 10;
	float timer = 3;
	
	public static string timestamp, timestamp_old = String.Empty;
	static string date = String.Empty;
		
	string filepath = String.Empty;
	//XmlDocument xmlDoc;
	static XmlWriter writer;
	static XmlWriter writer2;
	
	static List<string> jointlst = new List<string>();
	public static List<string> logDev = new List<string>();//available devices for logging
	public static List<string> selectedLogDev = new List<string>();//selected devices for logging
	
	Transform Body,Head,Neck,Torso,Waist,LeftShoulder,LeftElbow,LeftWrist,RightShoulder,RightElbow,RightWrist,LeftHip,LeftKnee,LeftAnkle,RightHip,RightKnee,RightAnkle;

	public static bool runthread=false;
	
//	static double oldAF3,oldF7,oldF3, oldFC5, oldT7, oldP7, oldO1, oldO2, oldP8, oldT8, oldFC6, oldF4, oldF8, oldAF4, oldcounter = -1;
	
	static float Old_posx = -1;
	
	public static bool capture = false;
	public static bool writeXml = false;
	
	public int loopflag;
	
	public float uptime;
	public float rate = 0.0f;
	int count=0;
	bool writeXmlStatus=false;
	
	bool logALL=true;//log all data toggle button
	
	//toggle buttons
	public static bool xml=false;
	public static bool csv=true;
	
	public static bool startLog = false;
//	public static Dictionary<string, string> acqdict = new Dictionary<string, string>();
	
	bool availableData = false;
	
	string stopDevice;

	TextWriter file;
	
	void Awake()
	{
		date = DateTime.Now.ToString("hh:mm dd-MM-yyyy"); // get date
		
	}
	
	
	void FixedUpdate()
	{
		
		timestamp = DateTime.UtcNow.Minute.ToString("00")+DateTime.UtcNow.Second.ToString("00")+DateTime.Now.Millisecond.ToString("0000"); //time in min:sec:usec
		
		if(capture)	
		{
			uptime+= Time.deltaTime;
		}
		
//		if(writeXml)
//		if(!capture)	
//		{
//			uptime+= Time.deltaTime;
//			
//			  if(counter != blinkSpeed)
//			    {
//			        blink = true;
//					counter++;
//			        
//			    } 
//			    else
//			        blink = false;
//			    	counter = 0;
//		}
//		else{uptime=0;}	
		
		if(selectedLogDev.Count != 0)
		{
			availableData = true;
		}
		else{availableData = false;}
		
	}
	
	 void Update()
	 { 
		
		string[] separators = {":"," "};
		
		foreach(string dev in DevicesLists.availableDev)
		{
			string[] words = dev.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			string device = words[0];
			
			if (!logDev.Contains(device) && !selectedLogDev.Contains(device))
			logDev.Add(device);
		}
		foreach(string dev in UDPReceive.DataList)//udp
		{
			string[] words = dev.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			string device = words[0];
			
			if (!logDev.Contains(device) && !selectedLogDev.Contains(device))
			logDev.Add(device);
		}		
		
//		if(selectedLogDev.Count !=0)
//		{
//			capture=true;
//		}
//		else{capture=false;}
		
		
		if(CheckUpdates.clinicVersion && KinectGUI.startzigfu)
		{
			//start/stop logging
			if(startLog && !capture)
			{
				capture=true;
				startLogSelectedDevices();
			}
			else if (!startLog && capture)
			{
				stopAllLogging();
				capture=false;
				selectedLogDev.Clear();;
			}
		}
		
	 }
	
	
	void logInit()
	{
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Kinect/";
	//	filepath = Application.dataPath + @"/Data/Log/DataLog_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml logging initialisation	
		if (xml)
		{
			string filepath2 = path + "Kinect_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			writer = XmlWriter.Create(filepath2,settings);
			writer.WriteStartDocument();
			writer.WriteComment("Date: " + date);	//comment
			writer.WriteStartElement("Log");
			
			writeXml = true;			
			InvokeRepeating("XMLWrite", 1F, 0.031F);//32.25 Hz
		}
		
		//csv logging initialisation	
		if(csv)
		{
				
			filepath = path + "Kinect_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";//save on Desktop
			
			file = new StreamWriter(filepath, false);
	
			string header = "timestamp, uptime, "+
							"Radarx, Radary, "+
							"BodyPosx, BodyPosy, BodyPosz, BodyRotx, BodyRoty, BodyRotz," +
							"HeadPosx, HeadPosy, HeadPosz, HeadRotx, HeadRoty, HeadRotz," +
							"NeckPosx, NeckPosy, NeckPosz, NeckRotx, NeckRoty, NeckRotz," +
							"TorsoPosx, TorsoPosy, TorsoPosz, TorsoRotx, TorsoRoty, TorsoRotz," +
							"WaistPosx, WaistPosy, WaistPosz, WaistRotx, WaistRoty, WaistRotz," +
							"LeftShoulderPosx, LeftShoulderPosy, LeftShoulderPosz, LeftShoulderRotx, LeftShoulderRoty, LeftShoulderRotz," +
							"LeftElbowPosx, LeftElbowPosy, LeftElbowPosz, LeftElbowRotx, LeftElbowRoty, LeftElbowRotz," +
							"LeftWristPosx, LeftWristPosy, LeftWristPosz, LeftWristRotx, LeftWristRoty, LeftWristRotz," +
							"RightShoulderPosx, RightShoulderPosy, RightShoulderPosz, RightShoulderRotx, RightShoulderRoty, RightShoulderRotz," +
							"RightElbowPosx, RightElbowPosy, RightElbowPosz, RightElbowRotx, RightElbowRoty, RightElbowRotz," +
							"RightWristPosx, RightWristPosy, RightWristPosz, RightWristRotx, RightWristRoty, RightWristRotz";
					
					
	        file.WriteLine(header);
	        file.Close();
			writeXml = true;	
			InvokeRepeating("csvWrite", 1F, 0.031F);//32.25 Hz
			}
		 
		Debug.Log("Started Kinect Logging");
	}

	
	void csvWrite()
	{
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +
		ZigUsersRadar.radarPosition.x.ToString() + "," + ZigUsersRadar.radarPosition.y.ToString() + "," +
		ZigSkeleton.pBody.x.ToString() + "," + ZigSkeleton.pBody.y.ToString() + "," + ZigSkeleton.pBody.z.ToString() + ","+ ZigSkeleton.rBody.eulerAngles.x.ToString() + "," + ZigSkeleton.rBody.eulerAngles.y.ToString() + "," + ZigSkeleton.rBody.eulerAngles.z.ToString() + "," + 
		ZigSkeleton.pHead.x.ToString() + "," + ZigSkeleton.pHead.y.ToString() + "," + ZigSkeleton.pHead.z.ToString() + ","+ ZigSkeleton.rHead.eulerAngles.x.ToString() + "," + ZigSkeleton.rHead.eulerAngles.y.ToString() + "," + ZigSkeleton.rHead.eulerAngles.z.ToString() + "," + 
		ZigSkeleton.pNeck.x.ToString() + "," + ZigSkeleton.pNeck.y.ToString() + "," + ZigSkeleton.pNeck.z.ToString() + ","+ ZigSkeleton.rNeck.eulerAngles.x.ToString() + "," + ZigSkeleton.rNeck.eulerAngles.y.ToString() + "," + ZigSkeleton.rNeck.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pTorso.x.ToString() + "," + ZigSkeleton.pTorso.y.ToString() + "," + ZigSkeleton.pTorso.z.ToString()+ "," + ZigSkeleton.rTorso.eulerAngles.x.ToString() + "," + ZigSkeleton.rTorso.eulerAngles.y.ToString() + "," + ZigSkeleton.rTorso.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pWaist.x.ToString() + "," + ZigSkeleton.pWaist.y.ToString() + "," + ZigSkeleton.pWaist.z.ToString() + ","+ ZigSkeleton.rWaist.eulerAngles.x.ToString() + "," + ZigSkeleton.rWaist.eulerAngles.y.ToString() + "," + ZigSkeleton.rWaist.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pLeftShoulder.x.ToString() + "," + ZigSkeleton.pLeftShoulder.y.ToString() + "," + ZigSkeleton.pLeftShoulder.z.ToString() + ","+ ZigSkeleton.rLeftShoulder.eulerAngles.x.ToString() + "," + ZigSkeleton.rLeftShoulder.eulerAngles.y.ToString() + "," + ZigSkeleton.rLeftShoulder.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pLeftElbow.x.ToString() + "," + ZigSkeleton.pLeftElbow.y.ToString() + "," + ZigSkeleton.pLeftElbow.z.ToString() + ","+ ZigSkeleton.rLeftElbow.eulerAngles.x.ToString() + "," + ZigSkeleton.rLeftElbow.eulerAngles.y.ToString() + "," + ZigSkeleton.rLeftElbow.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pLeftWrist.x.ToString() + "," + ZigSkeleton.pLeftWrist.y.ToString() + "," + ZigSkeleton.pLeftWrist.z.ToString() + ","+ ZigSkeleton.rLeftWrist.eulerAngles.x.ToString() + "," + ZigSkeleton.rLeftWrist.eulerAngles.y.ToString() + "," + ZigSkeleton.rLeftWrist.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pRightShoulder.x.ToString() + "," + ZigSkeleton.pRightShoulder.y.ToString() + "," + ZigSkeleton.pRightShoulder.z.ToString() + ","+ ZigSkeleton.rRightShoulder.eulerAngles.x.ToString() + "," + ZigSkeleton.rRightShoulder.eulerAngles.y.ToString() + "," + ZigSkeleton.rRightShoulder.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pRightElbow.x.ToString() + "," + ZigSkeleton.pRightElbow.y.ToString() + "," + ZigSkeleton.pRightElbow.z.ToString()+ "," + ZigSkeleton.rRightElbow.eulerAngles.x.ToString() + "," + ZigSkeleton.rRightElbow.eulerAngles.y.ToString() + "," + ZigSkeleton.rRightElbow.eulerAngles.z.ToString() + "," + 			
		ZigSkeleton.pRightWrist.x.ToString() + "," + ZigSkeleton.pRightWrist.y.ToString() + "," + ZigSkeleton.pRightWrist.z.ToString()+ "," + ZigSkeleton.rRightWrist.eulerAngles.x.ToString() + "," + ZigSkeleton.rRightWrist.eulerAngles.y.ToString() + "," + ZigSkeleton.rRightWrist.eulerAngles.z.ToString());
        file.WriteLine("");
		file.Close();		
	}
		
	void XMLWrite()
	{
		
		
		if(timestamp != timestamp_old && selectedLogDev.Count !=0)
		{
			writer.WriteStartElement("UtcTime");
			writer.WriteAttributeString("mmssuuuu", timestamp);
			writer.WriteElementString("Uptime", uptime.ToString());
		
//			if(MainGuiControls.startzigfu && selectedLogDev.Contains("KINECT"))
//			{
				
					writer.WriteComment("Kinect Radar Data");	
				    writer.WriteStartElement("Radar");
					writer.WriteElementString("x", ZigUsersRadar.radarPosition.x.ToString());  
					writer.WriteElementString("y", ZigUsersRadar.radarPosition.y.ToString());
					writer.WriteEndElement();//radar
			
					writer.WriteComment("Kinect Tracking Data");	
				    writer.WriteStartElement("Joints");
				
				
								writer.WriteStartElement("Body");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pBody.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pBody.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pBody.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rBody.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rBody.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rBody.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("Head");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pHead.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pHead.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pHead.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rHead.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rHead.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rHead.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
								writer.WriteStartElement("Neck");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pNeck.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pNeck.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pNeck.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rNeck.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rNeck.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rNeck.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("Torso");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pTorso.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pTorso.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pTorso.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rTorso.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rTorso.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rTorso.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("Waist");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pWaist.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pWaist.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pWaist.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rWaist.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rWaist.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rWaist.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("LeftShoulder");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pLeftShoulder.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pLeftShoulder.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pLeftShoulder.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rLeftShoulder.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rLeftShoulder.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rLeftShoulder.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
					
				
								writer.WriteStartElement("LeftElbow");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pLeftElbow.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pLeftElbow.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pLeftElbow.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rLeftElbow.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rLeftElbow.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rLeftElbow.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
					

								writer.WriteStartElement("LeftWrist");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pLeftWrist.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pLeftWrist.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pLeftWrist.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rLeftWrist.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rLeftWrist.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rLeftWrist.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("RightShoulder");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pRightShoulder.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pRightShoulder.y.ToString()); //print(ZigSkeleton.pRightShoulder);
								writer.WriteElementString("z", ZigSkeleton.pRightShoulder.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rRightShoulder.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rRightShoulder.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rRightShoulder.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("RightElbow");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pRightElbow.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pRightElbow.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pRightElbow.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rRightElbow.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rRightElbow.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rRightElbow.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				
								writer.WriteStartElement("RightWrist");
								writer.WriteStartElement("Position");
								writer.WriteElementString("x", ZigSkeleton.pRightWrist.x.ToString());  
								writer.WriteElementString("y", ZigSkeleton.pRightWrist.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.pRightWrist.z.ToString());
								writer.WriteEndElement();//position	
												
								writer.WriteStartElement("Rotation");
								writer.WriteElementString("x", ZigSkeleton.rRightWrist.eulerAngles.x.ToString()); 
								writer.WriteElementString("y", ZigSkeleton.rRightWrist.eulerAngles.y.ToString());
								writer.WriteElementString("z", ZigSkeleton.rRightWrist.eulerAngles.z.ToString());				
								writer.WriteEndElement();//rotation
								writer.WriteEndElement();//joint name
				
				
				    writer.WriteEndElement();//joints

				
//			}//end if(MainGuiControls.startzigfu)	
		
			
		  writer.WriteEndElement();//timestamp
			
		  timestamp_old = timestamp;
			
		}//if time	
		
		count++;
		rate = count/uptime;
		
	}//end of xmlWrite
	
	
	
	void endLog()
	{
		
		//stop xml logging	
		if(xml)
		{
			writeXml = false;
			uptime = 0;
			CancelInvoke("XMLWrite");
			
				writer.WriteStartElement("KinectParameters");
			writer.WriteElementString("Angle", KinectGUI.angle.ToString());  
			writer.WriteElementString("SeatedMode", KinectGUI.SeatedMode.ToString());  
			writer.WriteElementString("TrackSkeletonInNearMode", KinectGUI.TrackSkeletonInNearMode.ToString());
			writer.WriteElementString("NearMode", KinectGUI.NearMode.ToString());
			writer.WriteElementString("toggleMirror", KinectGUI.toggleMirror.ToString());
				writer.WriteEndElement();//Kinect Parameters
				
				writer.WriteStartElement("SmoothingParameters");
			writer.WriteElementString("Smoothing", KinectGUI.Smoothing.ToString());  
			writer.WriteElementString("Correction", KinectGUI.Correction.ToString());
			writer.WriteElementString("Prediction", KinectGUI.Prediction.ToString());
			writer.WriteElementString("JitterRadius", KinectGUI.JitterRadius.ToString());
			writer.WriteElementString("MaxDeviationRadius", KinectGUI.MaxDeviationRadius.ToString());
				writer.WriteEndElement();//Smoothing Parameters

			writer.WriteEndElement();//log
			writer.WriteComment("End of Log");
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();	
				
			}
			
			//stop csv logging
			if (csv)
			{
				writeXml = false;
				uptime = 0;
				CancelInvoke("csvWrite");
				//csv = false;
			//	file.Flush();
				file.Close();
			}
			
			Debug.Log("Stoped Data Logging");	
	}
	
	
	// Clears everybody and returns true to help setting the desired one
	bool setMeOnly()
	{ 
		csv = xml = false;
		return true;
	}
	
	void OnGUI() 
	{
		
		if(MainGuiControls.LogMenu)
		{
		
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
				{
					MainGuiControls.LogMenu = false;
					MainGuiControls.hideMenus = false;
				}				
					
				#if UNITY_EDITOR
		//		GUI.Label(new Rect(Screen.width/2+300, (Screen.height/2+20), 100, 200), "kinect xml rate: " + rate.ToString("0.00"));
				#endif
								
					
		//		GUI.BeginGroup (new Rect (Screen.width - 215, (Screen.height/2 - 170), 180, 60));
		//		GUI.Box (new Rect (0,0,180,60), " ");
		//			
		//		GUI.Label(new Rect(10, 10, 100, 200), "Incoming Data Frequency: ");
		//			
		//		GUI.EndGroup ();
					
		GUI.enabled = selectedLogDev.Count !=0;		
					
				if(!capture)
					{
						GUI.color = Color.green;
						if(GUI.Button (new Rect (Screen.width/2 - 45, Screen.height-180, 80, 30), "Start"))
						{
							capture=true;
							startLogSelectedDevices();
						}
						GUI.color = Color.white;
						
						//csv, xml toggle buttons
						if(GUI.Toggle(new Rect(Screen.width/2 - 70, Screen.height-100, 50, 50), csv, csvTex))csv=setMeOnly();
						if(GUI.Toggle(new Rect(Screen.width/2 + 10, Screen.height-100, 50, 50), xml, xmlTex))xml=setMeOnly();
					}
				else
					{
						GUI.color = Color.red;
						if(GUI.Button (new Rect (Screen.width/2 - 45, Screen.height-180, 80, 30), "Stop"))
						{
							stopAllLogging();
							capture=false;
							selectedLogDev.Clear();
						}	
						GUI.color = Color.white;
					}
							
		GUI.enabled = true;	//enable 		
			
				//list buttons of available joints dynamically
				GUI.Box(new Rect(Screen.width/2 - 370, Screen.height/2 - 230, 310, 330), " ");
				GUI.color = Color.yellow;
				GUI.Label(new Rect(Screen.width/2 - 270, Screen.height/2 - 220, 130, 200), "Available Devices:");
				GUI.color = Color.white;
				float yOffset = 0.0f;			
				scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2- 360, Screen.height/2 - 200, 280, 280), scrollPosition, new Rect(0, 0, 300, 320*(logDev.Count/4)));
					foreach(string dev in logDev)//local data
			        {
			           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(dev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dev.ToUpper())))
						{
							print("Logging: " + dev);
							
							if(!selectedLogDev.Contains(dev))
							{
							selectedLogDev.Add(dev);//add to selected data list
							}
							
						//	startLogSelectedDevices();//start logging
							
							logDev.Remove(dev);//remove from previous list	
		
			           	}			
			          yOffset += 25;
			         }		
				GUI.EndScrollView();
					
					
		
				//selected data
				GUI.Box(new Rect(Screen.width/2 + 60, Screen.height/2 - 230, 310, 330), " ");	
				GUI.color = Color.yellow;
				GUI.Label(new Rect(Screen.width/2+ 180, Screen.height/2 - 220, 130, 200), "Logging:");
				GUI.color = Color.white;
				float yOffset2 = 0.0f;			
				scrollPosition2 = GUI.BeginScrollView(new Rect(Screen.width/2 + 70, Screen.height/2 - 200, 280, 280), scrollPosition2, new Rect(0, 0, 300, 300+(selectedLogDev.Count*10)));
					foreach(string sdev in selectedLogDev)
			        {
						GUI.Label(new Rect (5, 20+ yOffset2, 10+(sdev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sdev.ToUpper()));
		//			if(GUI.Button (new Rect (5, 20+ yOffset2, 10+(sdev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sdev.ToUpper())))
		//				{
		//					selectedLogDev.Remove(sdev);
		//					logDev.Add(sdev);
		//					stopDevice = sdev;
		//					stopLogSelectedDevices();
		//	
		//				}
			          yOffset2 += 25;
					}
		        GUI.EndScrollView();	
					
					
					//clear list button
					if(GUI.Button (new Rect (Screen.width/2 + 170, Screen.height/2+110, 80, 20), "Clear List"))
					{
						selectedLogDev.Clear();
						//stop logging
						if(capture)
					    {
							capture=false;
							
							stopAllLogging();
							
						}
					}
					
					
					//arrows icon
					GUI.Label(new Rect (Screen.width/2 - 20 , Screen.height/2 - 120, 50, 50), arrowsIcon);
					
					
		}//if log menu	
			
					
				//rec blinking
				
		//		if(blink)
		//       	GUI.Label(new Rect(Screen.width-160, 20, 100, 20), rec);
		//    	else
		//       	GUI.Label(new Rect(Screen.width-160, 20, 100, 20), "");
		
		
    }//end GUI
	
	void stopAllLogging()
	{
		//ToDo: Terminate all devices logging
		if(writeXml)//kinect
		{endLog();}
		if(EmoEngineInst.writeXml)//emotiv
		{EmoEngineInst.endLog();}
		if(WiiData.startLog) //wii
		{WiiData.startLog = false;}
		if(Vuzix.startLog)//vuzix
		{Vuzix.startLog = false;}		
		if(KeyMouseEvents.startLog)//keyboard,mouse
		{KeyMouseEvents.startLog = false;}
		if(OculusGUI.startLog)//oculus
		{OculusGUI.startLog = false;}	
		if(TobiiData.startLog)//tobii
		{TobiiData.startLog = false;}
		if(TETSettings.startLog)//TET
		{TETSettings.startLog = false;}
		if(TobiiEyeXGUI.startLog)//tobii eyeX
		{TobiiEyeXGUI.startLog = false;}
	}
	
	void startLogSelectedDevices()
	{
		foreach(string sdev in selectedLogDev)
		{
			//start logging
			switch (sdev)
			{
			    case "MOUSE":
				{
					if(!KeyMouseEvents.startLog)
					KeyMouseEvents.startLog = true;
					Debug.Log("Start Mouse Log");
					break;
				}
			    case "KEYBOARD":
				{
					if(!KeyMouseEvents.startLog)
					KeyMouseEvents.startLog = true;
					Debug.Log("Start Keyb Log");
					break;
				}					
			    case "KINECT":
				{
					if(!writeXml)
					capture=true;
					logInit();
					Debug.Log("Start Kinect Log");
					break;
				}
				case "EMOTIV":
				{
					Debug.Log("Start Emotiv Log");
					EmoEngineInst.logInit();//log raw EEG	
					break;
				}
				case "WII":
				{
					if(!WiiData.startLog)
					Debug.Log("Start Wii Log");
					WiiData.startLog = true;//	
					break;
				}
				case "VUZIX":
				{
					if(!Vuzix.startLog)
					Debug.Log("Start Vuzix Log");
					Vuzix.startLog = true;//
					break;
				}
				case "OCULUS":
				{
					if(!Vuzix.startLog)
					Debug.Log("Start Oculus Log");
					OculusGUI.startLog = true;//	
					break;
				}	
				case "TOBII":
				{
					if(!TobiiData.startLog)
					Debug.Log("Start Tobii Log");
					TobiiData.startLog = true;//	
					break;
				}
				case "TET":
				{
					if(!TETSettings.startLog)
					Debug.Log("Start TET Log");
					TETSettings.startLog = true;//
					break;
				}
				case "TOBIIEYEX":
				{
					if(!TobiiEyeXGUI.startLog)
					Debug.Log("Start EyeX Log");
					TobiiEyeXGUI.startLog = true;//
					break;
				}		
				default:
				{
	
					break;					
				}
			}
		}
	}
	
	void stopLogSelectedDevices()
	{
		//stop logging	
		switch (stopDevice)
		{
		    case "MOUSE":
			{
				//ToDo:
				Debug.Log("Start Mouse Log");
				break;
			}
		    case "KEYBOARD":
			{
				//ToDo:
				Debug.Log("Start Keyb Log");
				break;
			}						
		    case "KINECT":
			{
				Debug.Log("Stop Kinect Log");
				capture=false;
				endLog();
				break;
			}
			case "EMOTIV":
			{
				Debug.Log("Stop Emotiv Log");
				EmoEngineInst.endLog();//log raw EEG	
				break;
			}
			case "WII":
			{
				Debug.Log("Stop Wii Log");
				WiiData.startLog = false;//log raw EEG	
				break;
			}
			case "VUZIX":
			{
				Debug.Log("Stop Vuzix Log");
				Vuzix.startLog = false;//log raw EEG	
				break;
			}
			case "OCULUS":
			{
				Debug.Log("Stop Oculus Log");
				OculusGUI.startLog = false;//log raw EEG	
				break;
			}			
			case "TOBII":
			{
				Debug.Log("Stop Tobii Log");
				TobiiData.startLog = false;//log raw EEG	
				break;
			}	
			case "TET":
			{
				Debug.Log("Stop TET Log");
				TETSettings.startLog = false;//log raw EEG	
				break;
			}
			case "TOBIIEYEX":
			{
				Debug.Log("Stot EyeX Log");
				TobiiEyeXGUI.startLog = false;//
				break;
			}	
			default:
			{

				break;					
			}			
		}//switch
	}
	
	void OnApplicationQuit () 
	{
		if(capture)
	    {
			capture=false;
			endLog();
		}
    }

	
	
  
}