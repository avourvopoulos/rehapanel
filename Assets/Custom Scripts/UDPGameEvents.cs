using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UDPGameEvents : MonoBehaviour {
	
	// receiving Thread
    Thread receiveThread; 
	 // udpclient object
    UdpClient client; 
	
	// public
    public int port; // define > init
//	private string portField = "1205";
	public static bool isConnected=false;
	
	public static List<string> datatypelst = new List<string>();
	public static List<string> devicelst = new List<string>();
	public static List<string> jointslst = new List<string>();
	public static List<string> transformTypelst = new List<string>();
	
	public static string datatype;
	public static string device;
	public static string joint;
	public static string transformationtype;
	public static string GameName, dttype, udpstring, udpstring2 = string.Empty;
	public static float udpx, udpy, udpz, udpw;
	
//	public static bool writeXML=false;
	
	public static string[] words;

	//csv log
	TextWriter file;
	public static string timestamp;
	string filepath = String.Empty;

	
	void Start()
	{
		port = 1212;
		init();

		logInit ();
	}

	void FixedUpdate () 
	{
		timestamp = DateTime.UtcNow.Hour.ToString ("00") +DateTime.UtcNow.Minute.ToString ("00") + DateTime.UtcNow.Second.ToString ("00") + DateTime.Now.Millisecond.ToString ("0000"); //time in min:sec:usec
	}

   public void init()
    {		
        // Local endpoint define (where messages are received).
        // Create a new thread to receive incoming messages.
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;	
        receiveThread.Start();
		isConnected = true;
    }
 

    // receive thread 
    public void ReceiveData() 
    {	
		client = new UdpClient(port);
		print("Started UDP - port: " +port);
		
        while (isConnected)
        {
            try 
            { 
              	 // receive Bytes from 127.0.0.1
					IPEndPoint IP = new IPEndPoint(IPAddress.Loopback, 0);

        	        byte[] udpdata = client.Receive(ref IP);
				
                //  UTF8 encoding in the text format.
					string data = Encoding.UTF8.GetString(udpdata);	
				
			
					//PROTOCOL//
					if(data!=String.Empty)
					{
						LogData(data);
					//	rawdata = data; //print(rawdata);
					}
		
				
            }//try
            catch (Exception err) 
            {
                print(err.ToString());
            }
			
		Thread.Sleep(10);
			
        }//while true		
	 
    }//ReceiveData
	
	
	void LogData(string n_data)
	{
		//	[$]<data  type> , [$$]<device> , [$$$]<joint> , <transformation> , <param_1> , <param_2> , .... , <param_N>
		//	[$]GameData , [$$]TPT-VR , [$$$]<joint> , <transformation> , <param_1> , <param_2> , .... , <param_N>
		
		// Decompose incoming data based on the protocol rules
//		string[] separators = {"[$]","[$$]","[$$$]",",",";"," "};
			      
//		words = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			file = new StreamWriter(filepath, true);
			file.Write(timestamp +","+ n_data);
			file.WriteLine("");
			file.Close();

	}//end of TranslateData()


	void logInit()
	{
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/UDP/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}

			filepath = path + "UDP_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
			
//			string header = "timestamp, uptime, "+
//				"Accelerometerx, Accelerometery, Accelerometerz,"+
//					"pitch, roll, yaw,"+
//					"IRx, IRy,"+
//					"Nunchuky, Nunchuky,"+
//					"NunchukStickx, NunchukSticky";
//			file.WriteLine(header);
			file.Close();
		
		Debug.Log("Started UDP Logging");
	}//log init
	
	
	void OnGUI()
	{
		if(MainGuiControls.NetMenu)
		{

		GUI.Label(new Rect(Screen.width/2-50, Screen.height-50, 200, 30), "Logging port: "+port);	
			
		}
	}//on GUI


	void endLog()
	{
		file.Close();
		file.Dispose();
		Debug.Log("Stoped UDP Logging");
	}
	
	void clearFile()
	{
		if (new FileInfo (filepath).Length == 0) 
		{
			File.Delete(filepath);
			Debug.Log ("EMPTY FILE");
		} 
		else 
		{
			Debug.Log ("NOT");
		}
	}
	
	void OnDisable() 
	{ 
		if(isConnected)
    	{
		 receiveThread.Abort(); 
		 client.Close(); 
		}
		endLog();
	} 	
	
	void OnApplicationQuit () 
	{
		if(isConnected)
	    {
		  receiveThread.Abort();
          client.Close();
		}
		endLog();
		clearFile ();
    }
	
	
}
