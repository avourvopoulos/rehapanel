using UnityEngine;
using System.Collections;
#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
#endregion
using System.Threading;
using LSL;

public class OpenBCIConnection : MonoBehaviour {

	public Texture2D backIcon;

	// create stream info and outlet LSL
	static liblsl.StreamInfo info;
	static liblsl.StreamOutlet outlet;
	static float[] data;
		
	SerialPort sp = new SerialPort("COM5", 115200);
	
	int count = 0;
	bool isReceiving = true;
	
	public float repeatrate = 0.004f;//250Hz
	
	public static string incomingdata;
	public static string[] parts;
	
	public static bool isConnected = false;
	public static bool paused = false;
	
	//fields
	public string comport = "COM5";
	public string baudrate = "115200";
	public string chan = "8";
	public string freq = "250";

	public static string exMsg = "";
	public static string[] ports;

	// receiving Thread
	Thread receiveThread;

	
	void Start()
	{
//		init ();//start thread

		// Get a list of serial port names.
		ports = SerialPort.GetPortNames();

		Debug.Log("The following serial ports were found:");
		
		// Display each port name to the console.
		foreach(string port in ports)
		{
			Debug.Log("port: " + port);
		}


	}

	public void init()
	{		
		// Local endpoint define (where messages are received).
		// Create a new thread to receive incoming messages.
		isConnected = true;
		receiveThread = new Thread(readData);
		receiveThread.IsBackground = true;	
//		receiveThread.Start();
		//	Debug.Log ("receiveThread");		
	}

	void initLSL(){
		// LSL - create stream info and outlet
		info = new liblsl.StreamInfo("openBCI", "EEG", 9, 2048, liblsl.channel_format_t.cf_float32, "rehabnet");
		outlet = new liblsl.StreamOutlet(info);
		data = new float[8];
		}
	
	void Update()
	{
//		if(Input.GetKey(KeyCode.X))
//			sp.WriteLine("x");
//		if(Input.GetKey(KeyCode.S))
//			sp.WriteLine("s");

		if (sp.IsOpen) {isConnected = true;}
		else {isConnected = false;}

	}
	
	void openConnection()
	{
		sp = new SerialPort(comport, int.Parse(baudrate));
		repeatrate = 1 / float.Parse(freq);

		try{
			sp.Open();
			sp.ReadTimeout = 5;
			sp.WriteLine("x");
			InvokeRepeating("readData", 0.1f, repeatrate);//250Hz
	//		receiveThread.Start();
			Debug.Log("start acquisition");
				exMsg = "";
		}
		catch(Exception ex){
			Debug.Log(ex);
			exMsg = ex.ToString();
				}

	}
	
	void closeConnection()
	{
		CancelInvoke("readData");
//		receiveThread.Abort(); 
		
		if (sp.IsOpen)
		{
			try{
				sp.WriteLine("s");
				sp.Close();
				Debug.Log("stop acquisition");
				exMsg = "";
			}
			catch(Exception ex){
				Debug.Log(ex);
				exMsg = ex.ToString();
			}
		}

	}
	
	void readData()
	{
//		while (isConnected){
			if (sp.IsOpen)
			{
				
				try
				{
					//			Debug.Log(sp.ReadLine());
					incomingdata = sp.ReadLine();
					
				//	chunkData(incomingdata);

					sendData();//send data via udp


					char[] delimiters = new char[] {',', '\r', '\n' };
					parts = incomingdata.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
					//LSL
					for (int k = 0; k < data.Length; k++){
						data[k] =  float.Parse(parts[k]);
						outlet.push_sample(data);
					}

				}
				catch(System.Exception)
				{
					//				Debug.LogError("Cannot read");
				}
			}
//		}
	}
	
	
	void chunkData(string data)
	{
		char[] delimiters = new char[] {',', '\r', '\n' };
		parts = data.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
		//		for (int i = 0; i < parts.Length; i++)
		//		for (int i = 1; i < 9; i++)
		//		{
		//			if(isNumber(parts[i]))//check if incoming string is numeric
		//			Debug.Log(i+": "+parts[i]);
		//			Debug.Log("size: "+parts.Length);
		//		}
	}

	
	bool isNumber(string num)
	{
		int n;
		bool isNumeric = int.TryParse(num, out n);
		
		return isNumeric;
	}


	void sendData()
	{
//		//LSL
//		for (int k = 0; k < data.Length; k++){
//			data[k] =  float.Parse(parts[k]);
//			outlet.push_sample(data);
//		}

		if(!DevicesLists.availableDev.Contains("OPENBCI:ANALOG:EEG:SIGNAL"))
		{
			DevicesLists.availableDev.Add("OPENBCI:ANALOG:EEG:SIGNAL");		
		}
		if(DevicesLists.selectedDev.Contains("OPENBCI:ANALOG:EEG:SIGNAL") && UDPData.flag==true)
		{			
			for (int i = 0; i < parts.Length; i++)
			{
				if(isNumber(parts[i]))//check if incoming string is numeric
				{
					UDPData.sendString("[$]analog,[$$]"+"openbci"+",[$$$]channel,"+i+","+parts[i]+";");
				}
			}
			 
		}
	}


	void OnGUI() 
	{
		
		if (MainGuiControls.openbciMenu) 
		{
			
			//back button	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.openbciMenu = false;
				MainGuiControls.hideMenus = false;
			}	
			

			
			if (!sp.IsOpen) {
				GUI.color = Color.green;
				if (GUI.Button (new Rect (50, 100, 120, 30), "Open Connection")){
					openConnection ();
					initLSL();
				}
				GUI.color = Color.white;
				
			}
			else{
				GUI.color = Color.red;
				if(GUI.Button(new Rect(50, 100, 120, 30), "Close Connection"))
					closeConnection();
				GUI.color = Color.white;
			}

	
			//fields
			comport = GUI.TextField(new Rect(110, 150, 50, 20), comport, 25);
			GUI.Label (new Rect (30, 150, 100, 20), "COM Port: ");
			baudrate = GUI.TextField(new Rect(110, 170, 80, 20), baudrate, 25);
			GUI.Label (new Rect (30, 170, 100, 20), "Baudrate: ");
			chan = GUI.TextField(new Rect(110, 190, 30, 20), chan, 25);
			GUI.Label (new Rect (30, 190, 100, 20), "Channels: ");
			freq = GUI.TextField(new Rect(110, 210, 30, 20), freq, 25);
			GUI.Label (new Rect (30, 210, 100, 20), "Frequency: ");
			

			if (sp.IsOpen && !paused){ 	
				if (GUI.Button (new Rect (420, 100, 100, 30), "Pause")){
					paused = true;
					sp.WriteLine("s");
				}
			}
			else if (sp.IsOpen && paused) {
				if (GUI.Button (new Rect (420, 100, 100, 30), "Resume")){
					paused = false;
					sp.WriteLine("x");
				}
			}


			//display raw data
			if (sp.IsOpen) 
			{
				//			if (isNumber (parts [0])) {//check if incoming string is numeric
				//				GUI.Label (new Rect (120, 90, 100, 20), "Packets: " + parts [0]);
				//			}
				
				for (int i = 1; i < parts.Length; i++)
				{
					if(isNumber(parts[i]))//check if incoming string is numeric
					{
						GUI.Label (new Rect (420, 150, 100, 20), "Packets: " + parts [0]);
						GUI.Label(new Rect(420, 170+i*20, 200, 20), "CH "+i+": "+parts[i] + " uV");
					}
				}
				
			}

			GUI.Label(new Rect(Screen.width/2-150, Screen.height/2+120, 500, 50), exMsg);


			GUI.Label(new Rect(30, 260, 500, 50), "The following serial ports were found:");
			int offset = 0;
			foreach(string port in ports)
			{
				GUI.Label(new Rect(30, 290+offset, 500, 50), port);
				offset = offset+20;
			}

			
		}//MainGuiControls.openbciMenu
		
	}//onGUI
	


	
	void OnDisable() 
	{
		closeConnection();
	}
	
	void OnApplicationQuit() 
	{
		closeConnection ();
	}
	



}
