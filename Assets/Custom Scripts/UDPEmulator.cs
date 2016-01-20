using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;

public class UDPEmulator : MonoBehaviour {
	
 // receiving Thread
    Thread receiveThread; 
 // udpclient object
    UdpClient client; 
	
	public Texture2D backIcon;
//	public Texture2D mouseIcon;
//	public Texture2D leftClickIcon;
//	public Texture2D rightClickIcon;
	
	bool leftClick = false;
	bool rightClick = false;

    List<string> datatypelst = new List<string>();
	public static List<string> devicelst = new List<string>();
	public static List<string> jointslst = new List<string>();
	public static List<string> transformTypelst = new List<string>();
	List<string> emulst = new List<string>();
	List<string> emutracklst = new List<string>();
	List<string> emubuttonlstL = new List<string>();
	List<string> emubuttonlstR = new List<string>();
//	public static float[] param = new float[4]; 
	
	public static string datatype;
	public static string device;
	public static string joint;
	public static string transformationtype;
	public static float udpx, udpy, udpz, udpw;
//	public static float x, y, z, w = 0.0f;
 
    // public
    public int port; // define > init
	private string portField = "1202";
	public static bool isConnected=false;
	
//	private string text = String.Empty;
	
	public static string selection = "n/a";
	public static string udpDev = "n/a";
	public static bool tracking = false;	
	
	float xmax, xmin, ymax, ymin, zmax, zmin;
	
	private float axisH = 0.0f;
	private float axisV = 0.0f;
	
	public static float dirH;
	public static float dirV;
	
	public static Vector3 udppos;
	public static Quaternion udprot;
	
	List<float> xvalues = new List<float>();
	List<float> yvalues = new List<float>();
	List<float> zvalues = new List<float>();
	float scalex, scaley, scalez;
	bool calibrate = true;
	public GUIStyle txtstyle;
	
	public static float ALPHA = 0.55f;
	//float[] xin, xout = new float[10];//x in/out
	//float[] yin, yout = new float[10];//y in/out
	public float[] n_input = new float[2];//xy in/out
	public float[] n_output = new float[2];//xy in/out
	
//	public Transform start;
	Vector3 FilteredPosition;
//	public float smooth = 5.0f;//smoothing for lerp
	public static float delta;
	public static bool deltaon = true;
	
	bool yon = true;
	bool zon = false;
	
	public static string rawdata;
	
	public static bool emulate = false;
	public static bool btnemulate = false;
	public Vector2 scrollPositiontrc = Vector2.zero;
	public Vector2 scrollPositionbtnL = Vector2.zero;
	public Vector2 scrollPositionbtnR = Vector2.zero;
	//GUIStyle guistyle;
	
	string LButton;
	string RButton;
	
	//window
	public static bool activeWin = false;
	public Rect windowRect0 = new Rect(300, 150, 290, 400);//posx, posy, width, height
	bool activewin=false;
	
	int oldX, oldY =1;//temp int for click
		
    public void Start()
    {
    //    init(); 
	//	camera.nearClipPlane = 11.0F;
		
    }
	
	
	void FixedUpdate () 
	{
		
		if(Input.GetKey(KeyCode.Q))
		{
			btnemulate=false;//stop button emulation
		}
//		//test mouse click emulation
//		if(Input.GetKey(KeyCode.L))
//		{
//			Emulator.LeftMouseClick((int)Input.mousePosition.x, (int)Input.mousePosition.y);
//		}
//		if(Input.GetKey(KeyCode.R))
//		{
//			Emulator.RightMouseClick((int)Input.mousePosition.x, (int)Input.mousePosition.y);
//		}
		
			//apply Time.deltaTime to the lowpass filter
			if(deltaon)
			{
				delta = Time.deltaTime;
			}
			else
			{
				delta = 1.0f;
			}
			//use y or z coordinates for the y axis
			if(yon)
			{
				zon=false;
			}
			else
			{
				yon=false;
				zon=true;
			}
		
		if(tracking)
	    {	
			//ToDO: Calibrate for 5 sec!
			
			if(calibrate)	
			{
				//XYZ MIN/MAX
				if(udppos.x !=0)//x
				{
					xvalues.Add(-udppos.x);
				
					xvalues.Sort();
					xmin = xvalues[0];
					xmax = xvalues[xvalues.Count-1];
				}
				
				if(udppos.y !=0)//y
				{
					yvalues.Add(-udppos.y);
				
					yvalues.Sort();
					ymin = yvalues[0];
					ymax = yvalues[yvalues.Count-1];
				}	
				
				if(udppos.z !=0)//z
				{
					zvalues.Add(-udppos.z);
				
					zvalues.Sort();
					zmin = zvalues[0];
					zmax = zvalues[zvalues.Count-1];
				}
				
			//	calstatus = calibrate;
			}//end calibrate
			else
			{
				//ToDo:
			//	if(xvalues.Count>0)
			//	clearLists();
			}
			
			int rangemaxx = Screen.currentResolution.width; 
			int rangemaxy = Screen.currentResolution.height;
			int rangeminx = 1; 
			int rangeminy = 1;
			//scale raw data between -1 to 1
			//new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
			scalex = ((-udppos.x - xmin)/(xmax-xmin)) * (rangemaxx - (rangeminx)) + (rangeminx);
			scaley = ((-udppos.y - ymin)/(ymax-ymin)) * (rangemaxy - (rangeminy)) + (rangeminy);
			scalez = ((-udppos.z - zmin)/(zmax-zmin)) * (rangemaxy - (rangeminy)) + (rangeminy);
			
			if(scalex > rangemaxx){scalex=rangemaxx;}
			else if(scalex < rangeminx){scalex=rangeminx;}
			else{scalex=scalex;}
			
			if(scaley > rangemaxy){scaley=rangemaxy;}
			else if(scaley < rangeminy){scaley=rangeminy;}
			else{scaley=scaley;}
			
			if(scalez > rangemaxy){scalez=rangemaxy;}
			else if(scalez < rangeminy){scalez=rangeminy;}
			else{scalez=scalez;}
	
			//put last x,y coordinates to an array in order to feed the tracking to the low pass filter
				n_input[0]=scalex;
				//option for y or z axis to act like y.
				if(yon)
				{n_input[1]=scaley;}//y
				else
				{n_input[1]=scalez;}//z
			
			n_output[0] = lowPass(n_input, n_output)[0];
			n_output[1] = lowPass(n_input, n_output)[1];
			
			//lowPass(n_input, n_output); //[0]for x, [1]for y
			FilteredPosition = new Vector3(lowPass(n_input, n_output)[0], lowPass(n_input, n_output)[1]);	
			
		}//end tracking
		
				if(emulate)//ToDo: && datatype=="tracking"
				{
					//add mouse cursor code
					Emulator.MoveMouse((int)FilteredPosition.x, (int)FilteredPosition.y);
					//print("emulation");						
				
				if(Input.GetKey(KeyCode.Q))
				{
					emulate=false;//stop emulation
					btnemulate=false;
				}
				if(Input.GetKey(KeyCode.C))
				{
					calibrate=false;//stop calivration
					print("calibration stoped");
				}
				if(Input.GetKey(KeyCode.D))
				{
					if(!deltaon)
					{
					deltaon=true;//enable delta in lowpass
					print("Delta ON");
					Thread.Sleep(100);
					}
					else
					{
					deltaon=false;//disable delta in lowpass
					print("Delta OFF");
					Thread.Sleep(100);
					}
				}
		}//if emulate
		
	}//update
	
	
	IEnumerator Calibrate()
    {
			print("Calibrating...");
            yield return new WaitForSeconds(10);
			calibrate=false;//stop calivration
			print("Calibration finished!");
    }
	
	
	
	//low pass filter x,y
	float[] lowPass( float[] input, float[] output ) 
	{
     for(int i=0; i<input.Length; i++ ) 
		{
			 if( output == null || output.Length == 0 || float.IsNaN(output[0]) || float.IsNaN(output[1]) || float.IsInfinity(output[0]) || float.IsInfinity(output[1]))
			 {
			//	return input;
				output[i] = input[i];
			 }
			 else
			{
				output[i] = output[i] + ALPHA * (input[i] - output[i]) * delta;
			//	output[i] = (1-ALPHA)* input[i] + (ALPHA*output[i]); //ToDo: include a deltatime relative to ALPHA
				
			}
    	}
    		return output;
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
						TranslateData(data);
					//	rawdata = data; //print(rawdata);
					}
		
				
            }//try
            catch (Exception err) 
            {
                print(err.ToString());
            }
			
		//	clearLists();//clear lists
			
        }//while true		
	 
    }//ReceiveData
	
	
	void TranslateData(string n_data)
	{
		//	[$]<data  type> , [$$]<device> , [$$$]<joint> , <transformation> , <param_1> , <param_2> , .... , <param_N>
		
		// Decompose incoming data based on the protocol rules
		string[] separators = {"[$]","[$$]","[$$$]",",",";"," "};
			      
		string[] words = n_data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			 	         					
		datatype = words[0];
//		datatype = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(words[0].ToUpper());
		device = words[1];
		joint = words[2];
		transformationtype = words[3];
		
		string clickBool = words[4];
		
		string emustring = device+"::"+joint;
		
//		udpx = float.Parse(words[4]);
//		udpy = float.Parse(words[5]);
//		udpz = float.Parse(words[6]);
//		udpw = float.Parse(words[7]);	
		
		
		if(selection==emustring)	
		{
			if(transformationtype=="position")
			{
				udppos = new Vector3(float.Parse(words[4]),float.Parse(words[5]),float.Parse(words[6]));
			}
			else if (transformationtype=="rotation")
			{
				udprot = new Quaternion(float.Parse(words[4]),float.Parse(words[5]),float.Parse(words[6]),float.Parse(words[7]));
			}
		}
		if (LButton == emustring)
		{ 
			if(clickBool == "True" || clickBool == "true" || clickBool == "TRUE" || clickBool == "1")
			{
				Emulator.LeftMouseClick(0, 0);
				clickBool = "False";
				leftClick = true;
				Thread.Sleep(2000);
			}
			else
			{
				leftClick = false;
			}
		}
		if (RButton == emustring)
		{ 
			if(clickBool == "True" || clickBool == "true" || clickBool == "TRUE" || clickBool == "1")
			{
				Emulator.RightMouseClick(0, 0);
				clickBool = "False";		
				rightClick = true;
				Thread.Sleep(2000);
			}
			else
			{
				rightClick = false;
			}
		}
		
		
		
		if(!emubuttonlstL.Contains(emustring)&& datatype =="button")
		{
			emubuttonlstL.Add(emustring);
		//	emubuttonlstR.Add(emustring);
		}
		if(!emubuttonlstR.Contains(emustring)&& datatype =="button")
		{
			emubuttonlstR.Add(emustring);
		}		
		
		if(!emutracklst.Contains(emustring)&& datatype =="tracking")
		{
			emutracklst.Add(emustring);
		}
		//populate lists categorizing the segmeneted data
		if(!datatypelst.Contains(datatype)&& datatype!= String.Empty)
		{
			datatypelst.Add(datatype);//add to datatype list
		}
		if(!devicelst.Contains(device)&& device!=String.Empty)
		{
			devicelst.Add(device);//add to device list
		}
		if(!jointslst.Contains(joint)&& joint!=String.Empty)
		{
			jointslst.Add(joint);//add to joint list	
			emulst.Add(emustring);//emulation
		}	
		if(!transformTypelst.Contains(transformationtype)&& transformationtype!=String.Empty)
		{
			transformTypelst.Add(transformationtype);//add to transformaton type list
		}
		

	}//end of TranslateData()
	
	
	void clearLists()
	{	   
	  datatypelst.Clear();
	  devicelst.Clear();
	  jointslst.Clear();
	  transformTypelst.Clear();
	  	emubuttonlstL.Clear();
		emubuttonlstR.Clear();
		emutracklst.Clear();
		emulst.Clear();
		
	  xvalues.Clear();
	  yvalues.Clear();
	  zvalues.Clear();
	  xmax= 0; xmin = 0;
	  ymax= 0; ymin = 0;
	  zmax= 0; zmin = 0;
	}
	
	
	
	void OnGUI () 
	{
if(MainGuiControls.EmuMenu)
{		
	
		if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
		{
			MainGuiControls.EmuMenu = false;
			MainGuiControls.hideMenus = false;
		}	
			
		//Network Group
		GUI.BeginGroup (new Rect (Screen.width/2 - 100, (Screen.height / 2 - 220), 200, 150));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,150), "Data Emulation");
		GUI.color = Color.white;
		
		//display local ip address
		GUI.Label(new Rect(30, 30, 200, 20), "IP Address: "+LocalIPAddress());
				
		GUI.Label(new Rect(30, 65, 100, 20), "Port:");
		GUI.enabled = !isConnected;
		portField = GUI.TextField (new Rect (65, 65, 50, 20), portField);
		GUI.enabled = true;
		
	GUI.enabled = !isConnected;				
		//Start sending UDP
		if (GUI.Button (new Rect (10, 100, 90, 30), "Start")) 
		{
			port = int.Parse(portField);			
			init();		
			isConnected = true;
			print("Start Server");
		}
	GUI.enabled = true;	
		
	GUI.enabled = isConnected;		
		//Stop sending UDP
		if (GUI.Button (new Rect (100, 100, 90, 30), "Stop")) 
		{		
			isConnected = false;
			btnemulate = false;
			receiveThread.Abort();
			client.Close();
			clearLists();
			print("Stop UDP");
			tracking = false;
			calibrate = true;
		}
 	GUI.enabled = true;
		
				
		//display raw data
		GUI.color = Color.gray;
	//	GUI.Label(new Rect(10, 200, 100, 200), "Raw Data: "+lastReceivedUDPPacket);
		GUI.color = Color.white;
		
		GUI.EndGroup (); // end network group
		
		
		//list buttons of available joints dynamically
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2-60, 300, 130, 200), "Cursor Emulation:");
		GUI.color = Color.white;
		float yOffset = 0.0f;			
		scrollPositiontrc = GUI.BeginScrollView(new Rect(Screen.width/2-100, 320, 200, 130), scrollPositiontrc, new Rect(0, 0, 300, 180+(emutracklst.Count*20)));
			foreach(string emu in emutracklst)
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
				{
					selection = emu;//fix to point into a joint
					tracking=true;
					emulate=true;
					print("Selected: " + emu);
					StartCoroutine(Calibrate());//start calibration
	           	}			
	          yOffset += 25;
	         }	
        GUI.EndScrollView();
		
		
		//callibratoin pop-up window
			if (activeWin)
			{
	       	 windowRect0 = GUI.Window(0, windowRect0, CalibWindow, "Calibration");
			}
		GUI.enabled = isConnected;
		//callibratoin pop-up window
		  if (GUI.Button(new Rect((Screen.width/2)-50, Screen.height/2+130, 100, 20), "Filtering"))
			{activeWin = true;}
			GUI.Label(new Rect((Screen.width/2)-80, Screen.height/2+160, 200, 100), "Press 'q' to stop emulation");
		
		//display calibration dialog
		if(calibrate && emulate)
		{
		GUI.color = Color.yellow;	
		GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+220, 200, 100), "Calibrating...");
		GUI.color = Color.white;	
		}
		else if (!calibrate && emulate)
		{
		GUI.color = Color.green;	
		GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+220, 200, 100), "Calibration finished!");
		GUI.color = Color.white;
		}
		else{GUI.Label(new Rect((Screen.width/2)-70, Screen.height/2+220, 200, 100), " ");}
		
		GUI.enabled = true;	
		
			
		
		//left button
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2 - 310, Screen.height/2 - 230, 130, 200), "Left Button:");
		GUI.color = Color.white;
		float yOffset2 = 0.0f;			
		scrollPositionbtnL = GUI.BeginScrollView(new Rect(Screen.width/2- 360, Screen.height/2 - 200, 200, 130), scrollPositionbtnL, new Rect(0, 0, 300, 180+(emubuttonlstL.Count*20)));
			foreach(string emu in emubuttonlstL)
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset2, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
				{
					LButton = emu;
					btnemulate=true;
					print("Selected: " + emu);
					
				//	emubuttonlstR.Remove(emu);//remove from right button list
	           	}			
	          yOffset2 += 25;
	         }	
        GUI.EndScrollView();
			
			
		//right button
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2 + 210, Screen.height/2 - 230, 130, 200), "Right Button:");
		GUI.color = Color.white;
		float yOffset3 = 0.0f;			
		scrollPositionbtnR = GUI.BeginScrollView(new Rect(Screen.width/2 + 160, Screen.height/2 - 200, 200, 130), scrollPositionbtnR, new Rect(0, 0, 300, 180+(emubuttonlstR.Count*20)));
			foreach(string emu in emubuttonlstR)
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset3, 10+(emu.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(emu.ToUpper())))
				{
					RButton = emu;
					btnemulate=true;
					print("Selected: " + emu);
					
				//	emubuttonlstL.Remove(emu);//remove from left button list
	           	}			
	          yOffset3 += 25;
	         }	
        GUI.EndScrollView();		
			
		
		}//if emumenu
						
	}
	
	
		void CalibWindow(int windowID)
		{
			//Filtering & calibration
			GUI.BeginGroup (new Rect (30, 30, 230, 320));
			GUI.Box (new Rect (0,0,230,320), "Tracking Calibration");

			GUI.Label(new Rect(20, 40, 220, 20), "max: "+xmax.ToString("0.00")+","+ymax.ToString("0.00")+","+zmax.ToString("0.00"));
			GUI.Label(new Rect(20, 60, 220, 20), "min: "+xmin.ToString("0.00")+","+ymin.ToString("0.00")+","+zmin.ToString("0.00"));
			GUI.Label(new Rect(20, 90, 220, 20), "Scale: "+scalex.ToString("0.00")+","+scaley.ToString("0.00")+","+scalez.ToString("0.00"));
			
			yon = GUI.Toggle(new Rect(150, 120, 80, 20), yon, "use y");
			zon = GUI.Toggle(new Rect(150, 140, 80, 20), zon, "use z");
			
			//calibrate = GUI.Toggle(new Rect(30, 160, 100, 30), calibrate, "Calibrate");
			
			//filter
			GUI.Label(new Rect(20, 190, 200, 20), "Filtered: "+FilteredPosition.x.ToString("0.00")+","+FilteredPosition.y.ToString("0.00"));
			ALPHA = GUI.HorizontalSlider(new Rect(30, 250, 100, 30), ALPHA, 0.0F, 1.0F);
			GUI.Label(new Rect(50, 270, 200, 100), "Alpha: "+ALPHA.ToString("0.00"));
			deltaon = GUI.Toggle(new Rect(160, 250, 100, 30), deltaon, "Delta");
			GUI.EndGroup ();
		
			//close button
	        if (GUI.Button(new Rect((windowRect0.width/2)-50, windowRect0.height-30, 100, 20), "Close"))
			{
				activeWin = false;
	            print("closing calibration window");
			}
			
			//drag window
	         GUI.DragWindow(new Rect(0, 0, 10000, 20));
    	}

	
	
  	
	public string LocalIPAddress()
	 {
		if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
    	{
        return null;
    	}
		
	   IPHostEntry host;
	   string localIP = "";
	   host = Dns.GetHostEntry(Dns.GetHostName());
	   foreach (IPAddress ip in host.AddressList)
	   {
	     if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
	     {
	       localIP = ip.ToString();
	     }
	   }
	   return localIP;
	 }
		

	
	void OnDisable() 
	{ 
		emulate=false;//stop emulation
		btnemulate=false;
		
		if(isConnected)
    	{
		 receiveThread.Abort(); 
		 client.Close(); 
		}
	} 	
	
	void OnApplicationQuit () 
	{
		emulate=false;//stop emulation
		btnemulate=false;
		
		if(isConnected)
	    {
		  receiveThread.Abort();
          client.Close();
		}
    }
	


}