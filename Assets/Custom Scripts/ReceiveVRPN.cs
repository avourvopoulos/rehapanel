using UnityEngine;
using System.Collections;

public class ReceiveVRPN : MonoBehaviour {

	public static string analogVRPN = string.Empty;//analog
	public static double analog = 0.0;//analog
	public static bool lbtn = false; //left button
	public static bool rbtn = false; //right button
	public static bool lda1 = false;
	public static bool lda2 = false;

	double thres = 0; //threshold value
	public string threshold = "0.1";

	//window
	public Texture2D arrowUpV;
	public Texture2D arrowDownV;
	bool activeWin = false;//activate window
	public Rect windowRect1 = new Rect(Screen.width - 220, Screen.height / 2-20, 180, 150);//posx, posy, width, height
	public GUIStyle winStyleV;
	
	string DeviceName = string.Empty;
	string buttonbool;

	public static bool server = false;
	public static bool started = false;

	public static bool isTraining = false;
	public static bool isEmulating = false;
	public static string statusMSG =  "";

	// Use this for initialization
	void Start () {

		if (HandlePrefs.bciMode == "on") {
			startReceiving();
			activeWin = true;
		}

		VRPN.vrpnAnalog("openvibe-vrpn@localhost", 0);
		VRPN.vrpnButton("openvibe-vrpn@localhost", 0);
		VRPN.vrpnButton("openvibe-vrpn@localhost", 1);

	}
	
	// Update is called once per frame
	void Update () {
	
//		Debug.Log (VRPN.vrpnAnalog("openvibe-vrpn@localhost", 0)+ "\n L:" +VRPN.vrpnButton("openvibe-vrpn@localhost", 0) + " R:"+VRPN.vrpnButton("openvibe-vrpn@localhost", 1)+ "\n");

	}

	void FixedUpdate () 
	{
		
		if (started) {
			
			GetDeviceData ();
			
			sendVRPN ();//send received VRPN(UIVA) data via UDP
			
			LDAOutput ();//lda two classes-to-button

			
			//select name of device
			if (DevicesLists.device) {
				DeviceName = DevicesLists.newDevice;
			} else {
				DeviceName = "openvibe";
			}
			
			if (lbtn) {
				buttonbool = "10";
			} else if (rbtn) {
				buttonbool = "01";
			} else {
				buttonbool = "00";
			}
			
		}
		
	}

	void sendVRPN()
	{
			
		//analog
		if(!DevicesLists.availableDev.Contains("VRPN:ANALOG:OPENVIBE:SIGNAL"))
		{
			DevicesLists.availableDev.Add("VRPN:ANALOG:OPENVIBE:SIGNAL");		
		}
		if (DevicesLists.selectedDev.Contains ("VRPN:ANALOG:OPENVIBE:SIGNAL") && UDPData.flag == true) 
		{	
			UDPData.sendString ("[$]analog,[$$]" + DeviceName + ",[$$$]vrpn,signal," + analog.ToString ("0.0000") + ";"); 
			
			UDPData.sendString ("[$]analog,[$$]" + DeviceName + ",[$$$]vrpn,both," + analog.ToString ("0.0000") + "," + buttonbool + ";");
			
			//UnityEngine.Debug.Log (anlg.ToString ("0.000") + ", " + lbtn + ", " + rbtn);
			
		}
		//button
		if(!DevicesLists.availableDev.Contains("VRPN:BUTTON:OPENVIBE:BOOL"))
		{
			DevicesLists.availableDev.Add("VRPN:BUTTON:OPENVIBE:BOOL");		
		}
		if(DevicesLists.selectedDev.Contains("VRPN:BUTTON:OPENVIBE:BOOL") && UDPData.flag==true)
		{	
			if(lbtn)
			{
				//	UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]vrpn,left,"+"true"+";");
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,1;");
				
			}
			else
			{
				//	UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]vrpn,left,"+"false"+";");
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,0;");
			}
			if(rbtn)
			{
				//	UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]vrpn,right,"+"true"+";");
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,1;");
			}
			else
			{
				//	UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]vrpn,right,"+"false"+";");
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,0;");
			}
		}
		
		
		//analog to button (LDA output)
		if(!DevicesLists.availableDev.Contains("VRPN:BUTTON_LDA:OPENVIBE:BOOL"))
		{
			DevicesLists.availableDev.Add("VRPN:BUTTON_LDA:OPENVIBE:BOOL");		
		}
		if(DevicesLists.selectedDev.Contains("VRPN:BUTTON_LDA:OPENVIBE:BOOL") && UDPData.flag==true)
		{	
			if(lda2)//left
			{
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,1;");
			}
			else
			{
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftarrow,event,0;");
			}
			if(lda1)//right
			{
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,1;");
			}
			else
			{
				UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightarrow,event,0;");
			}
		}
		
		
	}

	void LDAOutput()
	{
		//get threshold value from guibox
		thres = double.Parse(threshold);
		
		if (analog > 0 + thres) // right hand
		{
			lda2 = false;
			lda1 = true;
		}
		else if (analog < 0 - thres) // left hand
		{
			lda1 = false;
			lda2 = true;
		}
		else
		{
			lda1 = false;
			lda2 = false;
		}
	}


	void OnGUI()
	{
		
		if(MainGuiControls.NetMenu)
		{	
			
			if(activeWin == false)
			{
				if (GUI.Button (new Rect (Screen.width - 220, Screen.height / 2-50, 200, 30), "OpenVibe to VR")) 
				{activeWin = true;}
				GUI.Label(new Rect(Screen.width - 50, Screen.height / 2-57, 40, 40), arrowDownV);
			}
			else if (activeWin == true)
			{
				if (GUI.Button (new Rect (Screen.width - 220, Screen.height / 2-50, 200, 30), "OpenVibe to VR")) 
				{activeWin = false;}
				GUI.Label(new Rect(Screen.width - 50, Screen.height / 2-57, 40, 40), arrowUpV);
			}
			
			if(activeWin)
			{
				windowRect1 = GUI.Window(0, windowRect1, VRPNWindow, "", winStyleV);	
			}	
			
		}
		
		
	}
	
	void VRPNWindow(int windowID) 
	{
		GUI.BeginGroup (new Rect (0, 0, 190, 180));
		
		GUI.color = Color.grey;
		GUI.Label(new Rect(60, 03, 200, 25), "openvibe-vrpn");
		GUI.color = Color.white;
		
		//display analog
		GUI.Label(new Rect(10, 30, 150, 25), "Analog: " + analog.ToString("0.0"));
		//display button
		if (lbtn)
			GUI.Label(new Rect(10, 60, 100, 25), "Button: " + "Left");
		else if (rbtn)
			GUI.Label(new Rect(10, 60, 100, 25), "Button: " + "Right");
		else
			GUI.Label(new Rect(10, 60, 100, 25), "Button: " + " ");	
		
		//display lda outcome
		if (lda2)
			GUI.Label(new Rect(140, 30, 50, 25), "<");
		else if (lda1)
			GUI.Label(new Rect(140, 30, 50, 25), ">");
		else
			GUI.Label(new Rect(140, 30, 50, 25), "-");
		//threshold
		threshold = GUI.TextField(new Rect(160, 30, 25, 20), threshold, 25);
		
//		isTraining = GUI.Toggle(new Rect(10, 90, 60, 30), isTraining, "Training");
		isEmulating = GUI.Toggle(new Rect(90, 90, 100, 30), isEmulating, "Emulate Keys");
		
		GUI.enabled = !started;				
		//Start sending UDP
		if (GUI.Button (new Rect (10, 120, 90, 30), "Start")) 
		{
			startReceiving();
			
		}
		GUI.enabled = true;	
		
		GUI.enabled = started;		
		//Stop sending UDP
		if (GUI.Button (new Rect (100, 120, 90, 30), "Stop")) 
		{		
			stopReceiving();
		}
		GUI.enabled = true;		
		
		
		GUI.EndGroup ();//end network group
		
		
		GUI.color = Color.red;
		GUI.Label(new Rect(25, 150, 200, 20), statusMSG);
		GUI.color = Color.white;
		
	}
	
	void startReceiving(){
		
		DevicesLists.selectedDev.Add ("VRPN:ANALOG:OPENVIBE:SIGNAL");
		DevicesLists.selectedDev.Add ("VRPN:BUTTON:OPENVIBE:BOOL");
		DevicesLists.selectedDev.Add ("VRPN:BUTTON_LDA:OPENVIBE:BOOL");
		
		server=true;
		started = true;

	}
	
	public static void stopReceiving(){
		DevicesLists.selectedDev.Remove ("VRPN:ANALOG:OPENVIBE:SIGNAL");
		DevicesLists.selectedDev.Remove ("VRPN:BUTTON:OPENVIBE:BOOL");
		DevicesLists.selectedDev.Remove ("VRPN:BUTTON_LDA:OPENVIBE:BOOL");
		
		started = false;
		server=false;
		analogVRPN = "0";
		analog = 0;
		lbtn = false;  rbtn = false; 
		lda1 = false;  lda2 = false;
	}
	
	void GetDeviceData()
	{


		analog = VRPN.vrpnAnalog("openvibe-vrpn@localhost", 0);
		lbtn = VRPN.vrpnButton("openvibe-vrpn@localhost", 0);
		rbtn = VRPN.vrpnButton("openvibe-vrpn@localhost", 1);
		
		analogVRPN = ("[$]analog,[$$]" + "openvibe" + ",[$$$]vrpn,both," + analog.ToString ("0") + "," + buttonbool + ";");
		
		//	UnityEngine.Debug.Log("Analog: "+ anlg + " Button: "+lbtn+" "+rbtn);
		//UnityEngine.Debug.Log(analogVRPN);
	}

	void OnApplicationQuit() 
	{
		stopReceiving();
		
	}

}
