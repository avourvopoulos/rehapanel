using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

public class UDPData : MonoBehaviour {
	
	private static int localPort;

    // prefs 
    public static string IP ;  // define in init
    public static int port ;  // define in init
	
	public static string ipField = "127.0.0.1";
	public static string portField = " ";
	
	public static bool flag=false;
		
	// "connection" 
    public static IPEndPoint remoteEndPoint;
    public static UdpClient client;

	//display raw data
	static List<string> inputData = new List<string>();
	public Vector2 scrollPosition1 = Vector2.zero;//
	float timer = 1f;//time to creal list

	public static float threadsleep = 1f;
	private bool showraw = false;
	
	// Use this for initialization
	void Start ()
	{
    //    init();	

		if (HandlePrefs.bciMode == "on") {

			portField = "1205";
			IP = ipField;
			port = int.Parse(portField);
			
			init();
			GeneralOptions.policyServer();//start policy server
			flag=true;
			UnityEngine.Debug.Log("Start UDP");

				}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
//		Time.fixedDeltaTime = threadsleep;

		//update available data list
		timer -= Time.deltaTime;
		if(timer <= 0)
		{
			//	checkList();
			inputData.Clear();
			timer = 1f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{	
	//	send UDP
	//	[$]<data type> , [$$]<device> , [$$]<item> , <transformation> , <param_1> , <param_2> , .... , <param_N>

//		if(inputData.Count>100)
//		{
//			inputData.Clear();
//		}
				
	}
	
	
	void OnGUI () 
	{
		
if(MainGuiControls.NetMenu)
{		
				
		//Network Group
		GUI.BeginGroup (new Rect (Screen.width - 220, Screen.height/2 - 270, 200, 120));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,120), "Send Data");
		GUI.color = Color.white;
		
GUI.enabled = !flag;
		
		GUI.Label(new Rect(10, 60, 100, 20), "Address:");
		GUI.Label(new Rect(10, 90, 100, 20), "Port:");
		ipField = GUI.TextField (new Rect (65, 60, 100, 20), ipField);
		portField = GUI.TextField (new Rect (65, 90, 50, 20), portField);
		
		//Start sending UDP
		if (GUI.Button (new Rect (10, 25, 90, 30), "Send")) 
		{
			IP = ipField;
			port = int.Parse(portField);
			
			init();
			GeneralOptions.policyServer();//start policy server
			flag=true;
			UnityEngine.Debug.Log("Start UDP");
		}	
GUI.enabled = true;	
		
GUI.enabled = flag;		
		//Stop sending UDP
		if (GUI.Button (new Rect (100, 25, 90, 30), "Disconnect")) 
		{		
			flag=false;
			client.Close();
			GeneralOptions.killPolicyServer();//stop policy server
			UnityEngine.Debug.Log("Stop UDP");
		//	inputData.Clear();
		}
 GUI.enabled = true;
		
		GUI.EndGroup (); // end network group


	//		GUI.Label(new Rect(Screen.width/2 - 50, Screen.height - 120, 100, 20), "Output");
			showraw = GUI.Toggle(new Rect(Screen.width/2 - 60, Screen.height - 120, 100, 20), showraw, "Show Output");
			if (showraw)
			{
				float yOffset = 0.0f;
				GUI.Box(new Rect(Screen.width/2 - 280, Screen.height - 100, 500, 90), " ");
				scrollPosition1 = GUI.BeginScrollView(new Rect(Screen.width/2 - 270, Screen.height - 100, 490, 80), scrollPosition1, new Rect(0, 0, 300, 100+(inputData.Count*10)));			
				foreach (string indt in inputData)//input data
				{
					GUI.Label(new Rect(5, 10+ yOffset, 450, 20), indt);
					yOffset += 25;
				}

				GUI.EndScrollView();
//
//				if (GUI.Button (new Rect (Screen.width - 270, Screen.height - 60, 40, 20), "Clear")) 
//				{
//					inputData.Clear();
//				}
			}


//			GUI.Label(new Rect(Screen.width - 160, Screen.height - 180, 300, 20), "Thread Speed: " + threadsleep.ToString("0.000") + "");
//			threadsleep = GUI.HorizontalSlider(new Rect(Screen.width - 160, Screen.height - 150, 120, 30), threadsleep, 0.01F, 1.0F);

}//if Arg

		 			
	}

	
	
	public static void init()
    {       			
//        Debug.Log("UDPSend.init()");     

        // define
//        IP="127.0.0.1";
//        port=1202;   
		
        // ----------------------------
        // Send
        // ----------------------------

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);

        client = new UdpClient();  

        // status
        print("Sending to "+IP+" : "+port);
    }
	
	
	 // sendData
    public static void sendString(string message)
    {
        try 
        {
               if (message != "") 
                {
                    // UTF8 encoding to binary format.
                     byte[] data = Encoding.ASCII.GetBytes(message);

					//Debug.Log(message);
					inputData.Add(message);//input data to list
			
					//byte[] data = Encoding.ASCII.GetBytes(message); asci

                    // Send the message to the remote client.
                   client.Send(data, data.Length, remoteEndPoint);

                }
        }

        catch (Exception err)
        {
			UnityEngine.Debug.Log(err.ToString());
        }

	//	Thread.Sleep(Convert.ToInt16(threadsleep));
    }//end of sendString
	
	
	 void OnApplicationQuit() 
		{
         	 if(flag)
			{
         	 client.Close();
			}
		}	
	
	
}
