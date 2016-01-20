using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class BCI2000 : MonoBehaviour {

	public Texture2D backIcon;

	// receiving Thread
	Thread receiveThread; 
	// udpclient object
	UdpClient client; 
	
	// public
	public int port; // define > init
	private string portField = "20321";
	public static bool isConnected=false;
	
	public static string data = string.Empty;

	//BCI2000 variables
	//CursorTask
	int Running;
	int Recording;
	int SourceTime;
	int TargetCode;
	int ResultCode;
	int Feedback;
	int PauseApplication;
	int CursorPosX;
	int CursorPosY;
	int CursorPosZ;
	int StimulusTime;
	double Signal0;
	double Signal1;
	
	//StimulusPresentation
	int StimulusCodeRes;
	int StimulusTypeRes;
	int StimulusCode;
	int StimulusType;
	int StimulusBegin;
	int PhaseInSequence;
	int SelectedStimulus;
	
	string DeviceName = string.Empty;

	double tempPos = 0;
	bool left, right;

	public Texture2D arrowLeft;
	public Texture2D arrowRight;
	
	// Use this for initialization
	void Start () 
	{
		port = int.Parse(portField);			
		init();		
		isConnected = true;
		print("Start BCI2000");

		InvokeRepeating("detectDirection", 0, 0.5f);
	}

	void FixedUpdate()
	{
		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "bci2000";
		}

		if (data != string.Empty)
		{	

			//enable network
//			if(!UDPData.flag)
//			{
//				if(!DevicesLists.selectedDev.Contains("BCI2000:BUTTON:ARROW:DIRECTION"))
//				{
//					DevicesLists.selectedDev.Add("BCI2000:BUTTON:ARROW:DIRECTION");
//				}
//				UDPData.IP = UDPData.ipField;
//				UDPData.port = int.Parse(UDPData.portField);
//				UDPData.init();
//				GeneralOptions.policyServer();//start policy server
//				UDPData.flag=true;
//				Debug.Log("Start UDP");
//			}

			if(!DevicesLists.availableDev.Contains("BCI2000:TRACKING:CURSOR:POSITION"))
			{
				DevicesLists.availableDev.Add("BCI2000:TRACKING:CURSOR:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("BCI2000:TRACKING:CURSOR:POSITION") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]cursor,position,"+CursorPosX.ToString()+","+CursorPosY.ToString()+","+CursorPosZ.ToString()+";");
				//test dummy kinect data
				//UDPData.sendString("[$]tracking,[$$]"+"kinect"+",[$$$]leftwrist, position,"+CursorPosX.ToString()+","+CursorPosY.ToString()+","+CursorPosZ.ToString()+";");
			}	
		
			//button arrow data
			if(!DevicesLists.availableDev.Contains("BCI2000:BUTTON:ARROW:DIRECTION"))
			{
				DevicesLists.availableDev.Add("BCI2000:BUTTON:ARROW:DIRECTION");		
			}
			if(DevicesLists.selectedDev.Contains("BCI2000:BUTTON:ARROW:DIRECTION") && UDPData.flag==true)
			{		
				if(left)
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,"+"1"+";");
				else
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,"+"0"+";");

				if (right)
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,"+"1"+";");
				else
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,"+"0"+";");
			}	

		}


		//detect direction
		//if (tempPos < Signal0) //CursorPosX
		if (Signal0<0) 
		{
			left = true;
			right = false;
		}
	//	else if(tempPos > Signal0)
		else if (Signal0>0) 
		{
			right = true;
			left = false;
		}
		else{
			right = false;
			left = false;
		}

		//print ("temp " + tempPos + " CursorPosX " + CursorPosX);

	}

	void detectDirection()
	{
		tempPos = Signal0;
	}
	
	public void init()
	{		
		// Local endpoint define (where messages are received).
		// Create a new thread to receive incoming messages.
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;	
		receiveThread.Start();
	}
	
	
	// receive thread 
	public void ReceiveData() 
	{	
		client = new UdpClient(port);
		print("Started UDP - port: " + port);
		
		while (isConnected)
		{
			try 
			{ 
				// receive Bytes from 127.0.0.1
				IPEndPoint IP = new IPEndPoint(IPAddress.Loopback, 0);
				
				byte[] udpdata = client.Receive(ref IP);
				
				//  UTF8 encoding in the text format.
				data = Encoding.UTF8.GetString(udpdata);	
				
				
				//	print(data);
				TranslateData(data);
				
				
			}//try
			catch (Exception err) 
			{
				print(err.ToString());
			}
			
			Thread.Sleep(1);
			
		}//while true		
		
	}//ReceiveData
	
	
	
	void TranslateData(string n_data)
	{
		
		if (n_data.Contains("Running"))
		{
			string[] separators = {"Running", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			Running = int.Parse(cleanData[0]);
		}
		if (n_data.Contains("Recording"))
		{
			string[] separators = {"Recording", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			Recording = int.Parse(cleanData[0]);
		}
		if (n_data.Contains("SourceTime"))
		{
			string[] separators = {"SourceTime", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			SourceTime = int.Parse(cleanData[0]);
		}		
		if (n_data.Contains("TargetCode"))
		{
			string[] separators = {"TargetCode", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			TargetCode = int.Parse(cleanData[0]);
		}		
		if (n_data.Contains("ResultCode"))
		{
			string[] separators = {"ResultCode", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			ResultCode = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("Feedback"))
		{
			string[] separators = {"Feedback", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			Feedback = int.Parse(cleanData[0]);
		}			
		if (n_data.Contains("PauseApplication"))
		{
			string[] separators = {"PauseApplication", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			PauseApplication = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("CursorPosX"))
		{
			string[] separators = {"CursorPosX", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			CursorPosX = -int.Parse(cleanData[0]);
		}		
		if (n_data.Contains("CursorPosY"))
		{
			string[] separators = {"CursorPosY", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			CursorPosY = -int.Parse(cleanData[0]);
		}		
		if (n_data.Contains("CursorPosZ"))
		{
			string[] separators = {"CursorPosZ", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			CursorPosZ = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("StimulusTime"))
		{
			string[] separators = {"StimulusTime", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusTime = int.Parse(cleanData[0]);
		}
		
		
		//stimulus presentation		
		if (n_data.Contains("StimulusCodeRes"))
		{
			string[] separators = {"StimulusCodeRes", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusCodeRes = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("StimulusTypeRes"))
		{
			string[] separators = {"StimulusTypeRes", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusTypeRes = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("StimulusCode"))
		{
			string[] separators = {"StimulusCode", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusCode = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("StimulusType"))
		{
			string[] separators = {"StimulusType", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusType = int.Parse(cleanData[0]);
		}			
		if (n_data.Contains("StimulusBegin"))
		{
			string[] separators = {"StimulusBegin", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			StimulusBegin = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("PhaseInSequence"))
		{
			string[] separators = {"PhaseInSequence", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			PhaseInSequence = int.Parse(cleanData[0]);
		}	
		if (n_data.Contains("SelectedStimulus"))
		{
			string[] separators = {"SelectedStimulus", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			SelectedStimulus = int.Parse(cleanData[0]);
		}	
		
		
		if (n_data.Contains("Signal(0,0)"))
		{
			string[] separators = {"Signal(0,0)", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			Signal0 = double.Parse(cleanData[0]);
		}	
		if (n_data.Contains("Signal(1,0)"))
		{
			string[] separators = {"Signal(1,0)", " "};      
			string[] cleanData = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);		
			Signal1 = double.Parse(cleanData[0]);
		}		
		
		
		
	}
	
	
	void OnGUI () 
	{
		if (LaunchApps.bci2000) {

			//back button	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.thirdPartyMenu = true;
				LaunchApps.bci2000 = false;
			}	

			//display direction arrow
			if(left)
				GUI.Label (new Rect (Screen.width/2- 80, 120, 150, 150), arrowLeft);
			else if (right)
				GUI.Label (new Rect (Screen.width/2- 80, 120, 150, 150), arrowRight);
			else
				GUI.Label (new Rect (Screen.width/2- 80, 80, 150, 20), " ");


						GUI.Label (new Rect (20, 80, 150, 20), "localhost: " + port);
		
						GUI.BeginGroup (new Rect (90, Screen.height/2-70, 230, 250));
						GUI.color = Color.yellow;
						GUI.Box (new Rect (0, 0, 230, 250), "Status");
						GUI.color = Color.white;
						//Status
						GUI.Label (new Rect (10, 20, 150, 20), "Running: " + Running);
						GUI.Label (new Rect (10, 50, 150, 20), "Recording: " + Recording);
						GUI.Label (new Rect (10, 80, 150, 20), "SourceTime: " + SourceTime);
						GUI.Label (new Rect (10, 110, 150, 20), "StimulusTime: " + StimulusTime);
						GUI.Label (new Rect (10, 140, 150, 20), "Signal(0,0): " + Signal0);
						GUI.Label (new Rect (10, 170, 150, 20), "Signal(1,0): " + Signal1);
						GUI.Label (new Rect (10, 200, 150, 20), "PauseApplication: " + PauseApplication);
						GUI.EndGroup ();
		

						GUI.BeginGroup (new Rect (Screen.width / 2 - 115, Screen.height/2-70, 230, 250));
						GUI.color = Color.yellow;
						GUI.Box (new Rect (0, 0, 230, 250), "Cursor Task");
						GUI.color = Color.white;	
						//CursorTask
						GUI.Label (new Rect (10, 20, 150, 20), "TargetCode: " + TargetCode);
						GUI.Label (new Rect (10, 50, 150, 20), "ResultCode: " + ResultCode);
						GUI.Label (new Rect (10, 80, 150, 20), "Feedback: " + Feedback);
						GUI.Label (new Rect (10, 110, 150, 20), "CursorPosX: " + CursorPosX);
						GUI.Label (new Rect (10, 140, 150, 20), "CursorPosY: " + CursorPosY);
						GUI.Label (new Rect (10, 170, 150, 20), "CursorPosZ: " + CursorPosZ);
						GUI.EndGroup ();
		

						GUI.BeginGroup (new Rect (Screen.width / 2 + 190, Screen.height/2-70, 230, 250));
						GUI.color = Color.yellow;
						GUI.Box (new Rect (0, 0, 230, 250), "Stimulus Presentation");
						GUI.color = Color.white;	
						//StimulusPresentation
						GUI.Label (new Rect (10, 20, 150, 20), "StimulusCodeRes: " + StimulusCodeRes);
						GUI.Label (new Rect (10, 50, 150, 20), "StimulusTypeRes: " + StimulusTypeRes);
						GUI.Label (new Rect (10, 80, 150, 20), "StimulusCode: " + StimulusCode);
						GUI.Label (new Rect (10, 110, 150, 20), "StimulusType: " + StimulusType);
						GUI.Label (new Rect (10, 140, 150, 20), "StimulusBegin: " + StimulusBegin);
						GUI.Label (new Rect (10, 170, 150, 20), "PhaseInSequence: " + PhaseInSequence);		
						GUI.Label (new Rect (10, 200, 150, 20), "SelectedStimulus: " + SelectedStimulus);		
						GUI.EndGroup ();

				}
	}
	
	
	
	void OnDisable() 
	{ 
		if(isConnected)
		{
			receiveThread.Abort(); 
			client.Close(); 
		}
		CancelInvoke("detectDirection");
	} 	
	
	void OnApplicationQuit () 
	{
		if(isConnected)
		{
			receiveThread.Abort();
			client.Close();
		}
		CancelInvoke("detectDirection");
	}
	

	
}
