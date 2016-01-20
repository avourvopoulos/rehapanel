using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

public class EmoEngineInst : MonoBehaviour 
{
    //----------------------------------------
    public static EmoEngine engine = EmoEngine.Instance;
    //ConsoleKeyInfo cki = new ConsoleKeyInfo();
    //----------------------------------------
    public static int[] Cq;
    public static int nChan;
    public static bool IsStarted = false;
    public static int numOfGoodContacts = 0;
	public static int userID = -1;
    //----------------------------------------
   	EdkDll.EE_SignalStrength_t signal;
	public static string AF3,F7,F3, FC5, T7, P7, O1, O2,P8, T8, FC6, F4,F8, AF4, COUNTER;
	
	public static uint srate = 0; //sampling rate
	public static bool run;
	
	public static float secs = 0.0005F;//0.0039 for 256.4Hz; 0.0078 for 128hz
	
	public static float uptime;
	public float rate = 0.0f;
//	bool dataflag = false;
	
	static bool runthread=false;
	static bool threadstatus=false;
	
	public static int _bufferSize;
	
	public static List<string> chanlst = new List<string>();
	public static Dictionary<string, string> dict = new Dictionary<string, string>();
//	private System.Object lockThis = new System.Object();
	public static AutoResetEvent autoEvent;
	
	public static Dictionary<EdkDll.EE_DataChannel_t, double[]> data =  new Dictionary<EdkDll.EE_DataChannel_t, double[]>();
	
	//----------------------------------------
	static string filepath = String.Empty;
	static string date = String.Empty;
	XmlDocument xmlDoc;
	public static XmlWriter writer;
	public static bool writeXml;
		//csv log
	static TextWriter file;
	public static string timestamp;
	//----------------------------------------
	
    void Start()
    { 
	
		autoEvent = new AutoResetEvent(false);//synchronization event
		
		dict.Add("COUNTER","-1");
		dict.Add("AF3",null);
		dict.Add("F7",null);
		dict.Add("F3",null);
		dict.Add("FC5",null);
		dict.Add("T7",null);
		dict.Add("P7",null);
		dict.Add("O1",null);
		dict.Add("O2",null);
		dict.Add("P8",null);
		dict.Add("T8",null);
		dict.Add("FC6",null);
		dict.Add("F4",null);
		dict.Add("F8",null);
		dict.Add("AF4",null);
		
    }
	
	static void keyHandler(ConsoleKey key)
	{}
	
	
    void FixedUpdate()
    {	
		if (IsStarted)
        {
		uptime+= Time.deltaTime;	
		}
		else{uptime=0;}
    }
	
	void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
    {
        //Console.WriteLine("User Added Event has occured");

        // record the user 
        userID = (int)e.userId;

        // enable data aquisition for this user.
        engine.DataAcquisitionEnable((uint)userID, true);

        // ask for up to 1 second of buffered data
        engine.EE_DataSetBufferSizeInSec(1);

    }

    static void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
        EmoState es = e.emoState;
        Int32 numCqChan = es.GetNumContactQualityChannels();
        EdkDll.EE_EEG_ContactQuality_t[] cq = es.GetContactQualityFromAllChannels();
        nChan = numCqChan;
        int temp = 0;
        for (Int32 i = 0; i < numCqChan; ++i)
        {
            if (cq[i] != es.GetContactQuality(i))
            {
                throw new Exception();
            }
           
            switch (cq[i])
            {
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_NO_SIGNAL:
                    Cq[i] = 0;
                    break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_VERY_BAD:
                    Cq[i] = 1;
                    break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_POOR:
                    Cq[i] = 2;
                    break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_FAIR:
                    Cq[i] = 3;
                    break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_GOOD:
                    temp++;
                    Cq[i] = 4;
                    break;
            }

            //---------------------
        }
        numOfGoodContacts = temp;
        //EdkDll.EE_SignalStrength_t signalStrength = es.GetWirelessSignalStatus();
        Int32 chargeLevel = 0;
        Int32 maxChargeLevel = 0;
        es.GetBatteryChargeLevel(out chargeLevel, out maxChargeLevel);

        EdkDll.EE_SignalStrength_t signalStrength = es.GetWirelessSignalStatus(); 
        if (signalStrength == EdkDll.EE_SignalStrength_t.NO_SIGNAL)
        {
            for (Int32 i = 0; i < numCqChan; ++i)
            {
                Cq[i] = 0;
            }
        }
		
    }
    static void engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
    {
        
    }
    static void engine_EmoEngineDisconnected(object sender, EmoEngineEventArgs e)
    {
        
    }
	
	void Enable()
	{
		if (!IsStarted)
        {	
            Cq = new int[18];
            engine.EmoEngineConnected +=
                 new EmoEngine.EmoEngineConnectedEventHandler(engine_EmoEngineConnected);
            engine.EmoEngineDisconnected +=
                new EmoEngine.EmoEngineDisconnectedEventHandler(engine_EmoEngineDisconnected);
            engine.EmoEngineEmoStateUpdated +=
                new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoEngineEmoStateUpdated);
            engine.Connect(); 
            //engine.RemoteConnect("127.0.0.1",1726);
			engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
			
            IsStarted = true;
			
			Thread w = new Thread (GetRaw);
			threadstatus=true;
			w.Start();// running thread
			print("1. main thread starting worker thread...");
			
        }  
	}
	
	
    void Stop()
    {
        engine.Disconnect();
    }
	
	public static void logInit()
	{

		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Emotiv/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		//xml init
		if(XmlDataWriter.xml)
		{		
			string filepath2 = path + "Epoc_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";//save on Desktop		
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			writer = XmlWriter.Create(filepath2,settings);
			writer.WriteStartDocument();
			writer.WriteComment("Date: " + date);	//comment
			writer.WriteStartElement("Log");
			writeXml=true;
		}
		
		//csv init
		if(XmlDataWriter.csv)
		{
			filepath = path + "Epoc_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
	
        string header = "timestamp, uptime,"+
						"COUNTER,INTERPOLATED,RAW_CQ,AF3,F7,F3, FC5, T7, P7, O1, O2,P8" +
                        ", T8, FC6, F4,F8, AF4,GYROX, GYROY, TIMESTAMP, ES_TIMESTAMP" +
                        "FUNC_ID, FUNC_VALUE, MARKER, SYNC_SIGNAL,";
	        file.WriteLine(header);
	        file.Close();
		
			writeXml=true;
		}		
		
		Debug.Log("Started EEG Logging");
	}
	
	public static void endLog()
	{
		//xml
		if(XmlDataWriter.xml)
		{	
			writeXml=false;
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
			writeXml=false;;
			file.Close();
			file.Dispose();
		}			
		
		Debug.Log("Stoped EEG Logging");
	}
  
	
	void GetRaw()
	{ 	
		int count =0;
		runthread=threadstatus;
		
		while(runthread)
		{
			
//		 Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);
			data = engine.GetData((uint)userID);
	
			//Debug.Log("GetRaw");
			 // Handle any waiting events
	        engine.ProcessEvents();
			
	        // If the user has not yet connected, do not proceed
	        if ((int)userID == -1)
				{
			//	Debug.Log("No User");	
	         //  return; 
			//	runthread=threadstatus;
				}
	
			//if no data, do not proceed
	       else if (data == null)
				{
				Debug.Log("NULL data");
			//	return;
			//	runthread=threadstatus;
				}
	            		
			//else acquire data
			else{
				 _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;	
				
			 	if(writeXml)
				{
					if(XmlDataWriter.csv)
					{
					    // Write the data to a file
					    TextWriter file = new StreamWriter(filepath, true);
					
				        for (int i = 0; i < _bufferSize; i++)
			        	{
							
							dict["COUNTER"] = data[EdkDll.EE_DataChannel_t.COUNTER][i].ToString();
							dict["AF3"] = data[EdkDll.EE_DataChannel_t.AF3][i].ToString();
							dict["F7"] = data[EdkDll.EE_DataChannel_t.F7][i].ToString();
							dict["F3"] = data[EdkDll.EE_DataChannel_t.F3][i].ToString();
							dict["FC5"] = data[EdkDll.EE_DataChannel_t.FC5][i].ToString();
							dict["T7"] = data[EdkDll.EE_DataChannel_t.T7][i].ToString();
							dict["P7"] = data[EdkDll.EE_DataChannel_t.P7][i].ToString();
							dict["O1"] = data[EdkDll.EE_DataChannel_t.O1][i].ToString();
							dict["O2"] = data[EdkDll.EE_DataChannel_t.O2][i].ToString();
							dict["P8"] = data[EdkDll.EE_DataChannel_t.P8][i].ToString();
							dict["T8"] = data[EdkDll.EE_DataChannel_t.T8][i].ToString();
							dict["FC6"] = data[EdkDll.EE_DataChannel_t.FC6][i].ToString();
							dict["F4"] = data[EdkDll.EE_DataChannel_t.F4][i].ToString();
							dict["F8"] = data[EdkDll.EE_DataChannel_t.F8][i].ToString();
							dict["AF4"] = data[EdkDll.EE_DataChannel_t.AF4][i].ToString();
							
							
							file.Write(XmlDataWriter.timestamp + "," + uptime.ToString()+ "," );
				            // now write the data
				            foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
				                file.Write(data[channel][i] + ",");
				            	file.WriteLine("");
			
			        	}
			        	file.Close();				
					
					}
				
					if(XmlDataWriter.xml)
					{
						for (int i = 0; i < _bufferSize; i++)
						{
							dict["COUNTER"] = data[EdkDll.EE_DataChannel_t.COUNTER][i].ToString();
							dict["AF3"] = data[EdkDll.EE_DataChannel_t.AF3][i].ToString();
							dict["F7"] = data[EdkDll.EE_DataChannel_t.F7][i].ToString();
							dict["F3"] = data[EdkDll.EE_DataChannel_t.F3][i].ToString();
							dict["FC5"] = data[EdkDll.EE_DataChannel_t.FC5][i].ToString();
							dict["T7"] = data[EdkDll.EE_DataChannel_t.T7][i].ToString();
							dict["P7"] = data[EdkDll.EE_DataChannel_t.P7][i].ToString();
							dict["O1"] = data[EdkDll.EE_DataChannel_t.O1][i].ToString();
							dict["O2"] = data[EdkDll.EE_DataChannel_t.O2][i].ToString();
							dict["P8"] = data[EdkDll.EE_DataChannel_t.P8][i].ToString();
							dict["T8"] = data[EdkDll.EE_DataChannel_t.T8][i].ToString();
							dict["FC6"] = data[EdkDll.EE_DataChannel_t.FC6][i].ToString();
							dict["F4"] = data[EdkDll.EE_DataChannel_t.F4][i].ToString();
							dict["F8"] = data[EdkDll.EE_DataChannel_t.F8][i].ToString();
							dict["AF4"] = data[EdkDll.EE_DataChannel_t.AF4][i].ToString();
//					

								writer.WriteStartElement("UtcTime");
								writer.WriteAttributeString("mmssuuuu", XmlDataWriter.timestamp);
								writer.WriteElementString("Uptime",uptime.ToString());
						
								writer.WriteComment("Emotiv EPOC Raw EEG Data");	
								writer.WriteStartElement("EEG");
								writer.WriteElementString("COUNTER",dict["COUNTER"]);
								writer.WriteElementString("AF3",dict["AF3"]);
								writer.WriteElementString("F7",dict["F7"]);
								writer.WriteElementString("F3",dict["F3"]);
								writer.WriteElementString("FC5",dict["FC5"]);
								writer.WriteElementString("T7",dict["T7"]);
								writer.WriteElementString("P7",dict["P7"]);
								writer.WriteElementString("O1",dict["O1"]);
								writer.WriteElementString("O2",dict["O2"]);
								writer.WriteElementString("P8",dict["P8"]);
								writer.WriteElementString("T8",dict["T8"]);
								writer.WriteElementString("FC6",dict["FC6"]);
								writer.WriteElementString("F4",dict["F4"]);
								writer.WriteElementString("F8",dict["F8"]);
								writer.WriteElementString("AF4",dict["AF4"]);
								writer.WriteEndElement();//EEG
						
								writer.WriteEndElement();//timestamp
							}
				

			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////			
						
		
					writer.WriteComment("Emotiv EPOC Gyro Data");	
					writer.WriteStartElement("Gyro");
								writer.WriteElementString("HeadPosition", EmoGyroData.headPosition.ToString());
								writer.WriteElementString("GyroX", EmoGyroData.GyroX.ToString("0.00"));
								writer.WriteElementString("GyroY", EmoGyroData.GyroY.ToString("0.00"));
					writer.WriteEndElement();//Gyro
		
					writer.WriteComment("Emotiv EPOC Affectiv Data");	
					writer.WriteStartElement("Affectiv");
								writer.WriteElementString("LongTermExcitement", EmoAffectiv.longTermExcitementScore.ToString("0.00"));
								writer.WriteElementString("ShortTermExcitement", EmoAffectiv.shortTermExcitementScore.ToString("0.00"));
								writer.WriteElementString("Meditation", EmoAffectiv.meditationScore.ToString("0.00"));
								writer.WriteElementString("Frustration", EmoAffectiv.frustrationScore.ToString("0.00"));
								writer.WriteElementString("Boredom", EmoAffectiv.boredomScore.ToString("0.00"));
					writer.WriteEndElement();//Affectiv	
		
					writer.WriteComment("Emotiv EPOC Expressiv Data");	
					writer.WriteStartElement("Expressiv");
								writer.WriteElementString("Blink", EmoExpressiv.isBlink.ToString());
								writer.WriteElementString("LookingUp", EmoExpressiv.isLookingUp.ToString());
								writer.WriteElementString("LookingDown", EmoExpressiv.isLookingDown.ToString());
								writer.WriteElementString("LookingLeft", EmoExpressiv.isLookingLeft.ToString());
								writer.WriteElementString("LookingRight", EmoExpressiv.isLookingRight.ToString());
								writer.WriteElementString("EyeLocationX", EmoExpressiv.eyeLocationX.ToString("0.00"));
								writer.WriteElementString("EyeLocationY", EmoExpressiv.eyeLocationY.ToString("0.00"));
								writer.WriteElementString("EyebrowExtent", EmoExpressiv.eyebrowExtent.ToString("0.00"));
								writer.WriteElementString("SmileExtent", EmoExpressiv.smileExtent.ToString("0.00"));
								writer.WriteElementString("UpperFacePower", EmoExpressiv.upperFacePower.ToString("0.00"));
								writer.WriteElementString("LowerFacePower", EmoExpressiv.lowerFacePower.ToString("0.00"));
					writer.WriteEndElement();//Expressiv	
			
					}//xml write
				
				}//if write xml
				
    			}//else if
			
				count++;
				rate = count/uptime;
			runthread=threadstatus;//
			
			Thread.Sleep(1);
			
		//chanlst.Clear();
		}//while
  		
	}//getraw
	
	
			
	void OnGUI()
    {
if(MainGuiControls.EmotivMenu)
{			
		#if UNITY_EDITOR
		GUI.Label(new Rect(Screen.width/2+210, Screen.height/2, 100, 200), "eeg aqc rate: " + rate.ToString("0.00"));
		#endif
		
		// EMOTIV EPOC SDK
		GUI.BeginGroup (new Rect (Screen.width - 212, (Screen.height/2 - 270), 200, 90));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,90), "ON/OFF");
		GUI.color = Color.white;
		

			
		if(!IsStarted)
		{
			GUI.color = Color.green;
			if(GUI.Button (new Rect (60, 25, 80, 30), "Start"))
			{
	//			dataflag = true;//flag to restart invoke call		
				Enable();
				Debug.Log("Started Emotiv Epoc");
			}
				GUI.color = Color.white;
			}
		else
			{
				GUI.color = Color.red;
				if(GUI.Button (new Rect (60, 25, 80, 30), "Stop"))
			{
				IsStarted = false;
	//			dataflag = false;
				threadstatus=false;//stop thdread	
				Stop();
				Debug.Log("Stoped Emotiv Epoc");
			}
			GUI.color = Color.white;
		}

		
		//Emotiv Uptime
		GUI.Label(new Rect(40, 65, 200, 20), "Up Time: " + uptime.ToString("0.00")+" sec");
		
		GUI.EndGroup();
		
		
		// EMOTIV EPOC SDK
			GUI.BeginGroup (new Rect (10-KinectGUI.gone, (Screen.height/2 - 270), 200, 500));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,500), " ");
		GUI.color = Color.white;		
		
		//Gyro Data
		GUI.color = Color.yellow;
		GUI.Label(new Rect(65, 5, 200, 20), "Gyro Data");
		GUI.color = Color.white;
		
		//Head position
		GUI.Label(new Rect(30, 40, 200, 20), "Head Position: " + EmoGyroData.headPosition);
		//GYRO X Y
		//GUI.color = Color.grey;
		GUI.Label(new Rect(30, 70, 200, 20), "GYRO X: " + EmoGyroData.GyroX +"  Y:"+EmoGyroData.GyroY);
		//GUI.Label(new Rect(30, 90, 200, 20), "GYRO Y: " + EmoGyroData.nGyroY);
		GUI.color = Color.white;
		
		
		//Affectiv Data
		GUI.color = Color.yellow;
		GUI.Label(new Rect(60, 120, 200, 20), "Affectiv Data");
		GUI.color = Color.white;
		//GUI.color = Color.grey;
		GUI.Label(new Rect(20, 140, 200, 50), "Long Term Excitement: " + EmoAffectiv.longTermExcitementScore.ToString("0.00"));
		GUI.Label(new Rect(20, 160, 200, 50), "Short Term Excitement: " + EmoAffectiv.shortTermExcitementScore.ToString("0.00"));
		GUI.Label(new Rect(20, 180, 200, 20), "Meditation: " + EmoAffectiv.meditationScore.ToString("0.00"));
		GUI.Label(new Rect(20, 200, 200, 20), "Frustration: " + EmoAffectiv.frustrationScore.ToString("0.00"));
		GUI.Label(new Rect(20, 220, 200, 20), "Boredom: " + EmoAffectiv.boredomScore.ToString("0.00"));
		GUI.color = Color.white;
		
		
		//Expressiv Data
		GUI.color = Color.yellow;
		GUI.Label(new Rect(60, 250, 200, 20), "Expressiv Data");
		GUI.color = Color.white;
		//GUI.color = Color.grey;
		GUI.Label(new Rect(20, 270, 200, 50), "Blink: " + EmoExpressiv.isBlink);
		GUI.Label(new Rect(20, 290, 200, 50), "Looking Up: " + EmoExpressiv.isLookingUp);
		GUI.Label(new Rect(20, 310, 200, 50), "Looking Down: " + EmoExpressiv.isLookingDown);
		GUI.Label(new Rect(20, 330, 200, 50), "Looking Left: " + EmoExpressiv.isLookingLeft);
		GUI.Label(new Rect(20, 350, 200, 50), "Looking Right: " + EmoExpressiv.isLookingRight);
		GUI.Label(new Rect(20, 370, 200, 50), "Eye Location X: " + EmoExpressiv.eyeLocationX.ToString("0.00"));
		GUI.Label(new Rect(20, 390, 200, 50), "Eye Location Y: " + EmoExpressiv.eyeLocationY.ToString("0.00"));
		GUI.Label(new Rect(20, 410, 200, 50), "Eyebrow Extent: " + EmoExpressiv.eyebrowExtent.ToString("0.00"));
		GUI.Label(new Rect(20, 430, 200, 50), "Smile Extent: " + EmoExpressiv.smileExtent.ToString("0.00"));
		GUI.Label(new Rect(20, 450, 200, 50), "Upper Face Power: " + EmoExpressiv.upperFacePower.ToString("0.00"));
		GUI.Label(new Rect(20, 470, 200, 50), "Lower Face Power: " + EmoExpressiv.lowerFacePower.ToString("0.00"));
		GUI.color = Color.white;
		
		GUI.EndGroup();
		
		
		// RAW EEG
			GUI.BeginGroup (new Rect (Screen.width / 2- 290-KinectGUI.gone, (Screen.height/2 + 190), 580, 160));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,580,160), "Raw EEG");
		GUI.color = Color.white;
		
		GUI.Label(new Rect(20, 20, 200, 50), "AF3: " + dict["AF3"]);
		GUI.Label(new Rect(20, 50, 200, 50), "AF4: " + dict["AF4"]);
		GUI.Label(new Rect(20, 80, 200, 50), "F3: " + dict["F3"]);
		GUI.Label(new Rect(20, 110, 200, 50), "F4: " + dict["F4"]);		
		GUI.Label(new Rect(20, 140, 200, 50), "F7: " + dict["F7"]);
		
		GUI.Label(new Rect(220, 20, 200, 50), "F8: " + dict["F8"]);
		GUI.Label(new Rect(220, 50, 200, 50), "FC5: " + dict["FC5"]);
		GUI.Label(new Rect(220, 80, 200, 50), "FC6: " + dict["FC6"]);
		GUI.Label(new Rect(220, 110, 200, 50), "T7: " + dict["T7"]);	
		GUI.Label(new Rect(220, 140, 200, 50), "T8: " + dict["T8"]);
		
		GUI.Label(new Rect(420, 20, 200, 50), "P7: " + dict["P7"]);
		GUI.Label(new Rect(420, 50, 200, 50), "P8: " + dict["P8"]);
		GUI.Label(new Rect(420, 80, 200, 50), "O1: " + dict["O1"]);
		GUI.Label(new Rect(420, 110, 200, 50), "O2: " + dict["O2"]);
		
//		GUI.color = Color.gray;
//		GUI.Label(new Rect(420, 140, 200, 50), "Sampling Rate: " + srate);
		GUI.color = Color.white;
		GUI.EndGroup();	
		
//		GUI.Label(new Rect(420, 110, Screen.width/2, Screen.height/2+50), "Sampling Rate: " + frate);
			
		}//emotiv menu	
	}//onGUI
	
	
	void OnApplicationQuit () 
	{
		if(writeXml)
		endLog();//stop raw eeg log
	}

}