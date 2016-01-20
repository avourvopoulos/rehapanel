using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityOSC;

public class OSCGUI : MonoBehaviour {
	
	public Texture2D arrowUp;
	public Texture2D arrowDown;
	
	private Dictionary<string, ServerLog> servers;
	
	public Vector2 scrollPositiontrc = Vector2.zero;
	public int port; // define > init
	private string portField = "6666";
	public static bool isConnected=false;
	string selection = "";
	List<string> osclst = new List<string>();
	
	public static bool writeXML=false;
	
	bool activeWin = false;//activate window
	public Rect windowRect0 = new Rect(Screen.width - 220, Screen.height / 2-60, 180, 220);//posx, posy, width, height
	public GUIStyle winStyle;
	
	float timer = 3;
	
	//string servname, address, data
	
	// Use this for initialization
	void Start () 
	{
		windowRect0 = new Rect(Screen.width - 220, Screen.height / 2-60, 200, 150);//posx, posy, width, height
		
		OSCHandler.Instance.Init(); //init OSC
		servers = new Dictionary<string, ServerLog>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		OSCHandler.Instance.UpdateLogs();
		servers = OSCHandler.Instance.Servers;
		
		foreach( KeyValuePair<string, ServerLog> item in servers )
		{
			// If we have received at least one packet,
			// show the last received from the log in the Debug console
			if(item.Value.log.Count > 0) 
			{
				int lastPacketIndex = item.Value.packets.Count - 1;
				
				string addr = item.Value.packets[lastPacketIndex].Address;
				
				//add items to list
				if (!osclst.Contains(addr) && !DevicesLists.availableDev.Contains(addr))
				{
					osclst.Add(addr);
					DevicesLists.availableDev.Add(addr);
				}
				
				//refresh list
				timer -= Time.deltaTime;
				if(timer <= 0)
				{
					timer = 3;
					osclst.Clear();
				}
				
//				UnityEngine.Debug.Log(String.Format("SERVER: {0} ADDRESS: {1} VALUE 0: {2}", 
//				                                    item.Key, // Server name
//				                                    item.Value.packets[lastPacketIndex].Address, // OSC address
//				                                    item.Value.packets[lastPacketIndex].Data[0].ToString())+
//												item.Value.packets[lastPacketIndex].Data[1].ToString()+
//												item.Value.packets[lastPacketIndex].Data[2].ToString()); //First data value
				
		if(DevicesLists.selectedDev.Contains(addr) && UDPData.flag)
		{
		//	print("[$]"+"tracking"+",[$$]"+DevicesLists.newDevice+",[$$$]"+addr+","+"position"+","+item.Value.packets[lastPacketIndex].Data[0].ToString()+","+item.Value.packets[lastPacketIndex].Data[1].ToString()+","+item.Value.packets[lastPacketIndex].Data[2].ToString()+","+"0"+";");
			
			UDPData.sendString("[$]"+"tracking"+",[$$]"+DevicesLists.newDevice+",[$$$]"+addr+","+"position"+","+item.Value.packets[lastPacketIndex].Data[0].ToString()+","+item.Value.packets[lastPacketIndex].Data[1].ToString()+","+item.Value.packets[lastPacketIndex].Data[2].ToString()+","+"0"+";");		
		}
					
//					if(selection==addr)	
//					{
//						writeXML=true;
//					
//						if(UDPData.flag && item.Value.packets[lastPacketIndex].Data.Count < 3)
//						{
//							float x = -(float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString()));
//							float y = -(float.Parse(item.Value.packets[lastPacketIndex].Data[1].ToString()));
//						
//							UDPData.sendString("[$]"+"Tracking"+",[$$]"+"OSC"+",[$$$]"+item.Value.packets[lastPacketIndex].Address+","+"position"+","+x.ToString()+","+y.ToString()+","+"0.0"+","+"0.0"+";");
//							
//							print("[$]"+"Tracking"+",[$$]"+"OSC"+",[$$$]"+item.Value.packets[lastPacketIndex].Address+","+"position"+","+x.ToString()+","+y.ToString()+","+"0.0"+","+"0.0"+";");
//
//						}
//						else
//						{
//							float x = -(float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString()));
//							float y = -(float.Parse(item.Value.packets[lastPacketIndex].Data[1].ToString()));
//							float z = float.Parse(item.Value.packets[lastPacketIndex].Data[2].ToString());
//						
//							UDPData.sendString("[$]"+"Tracking"+",[$$]"+"OSC"+",[$$$]"+item.Value.packets[lastPacketIndex].Address+","+"position"+","+x.ToString()+","+y.ToString()+","+z.ToString()+","+"0.0"+";");
//						
//							print("[$]"+"Tracking"+",[$$]"+"OSC"+",[$$$]"+item.Value.packets[lastPacketIndex].Address+","+"position"+","+x.ToString()+","+y.ToString()+","+z.ToString()+","+"0.0"+";");
//
//						}
//						
//					}
			
				
			}
	    }
		
	}
	
	
	void OSCWindow(int windowID) 
	{
		GUI.BeginGroup (new Rect (0, 0, 190, 150));
			
		//display local ip address
		GUI.Label(new Rect(20, 5, 200, 20), "IP Address: "+LocalIPAddress());
				
		GUI.Label(new Rect(20, 35, 100, 20), "Port:");
		GUI.enabled = !isConnected;
		portField = GUI.TextField (new Rect (55, 35, 50, 20), portField);
		GUI.enabled = true;
		
	GUI.enabled = !isConnected;				
		//Start sending UDP
		if (GUI.Button (new Rect (10, 80, 90, 30), "Start")) 
		{
			port = int.Parse(portField);			
			OSCHandler.CreateServer("localhost",port);
			print("Start OSC Server");
			isConnected = true;
		}
	GUI.enabled = true;	
		
	GUI.enabled = isConnected;		
		//Stop sending UDP
		if (GUI.Button (new Rect (100, 80, 90, 30), "Stop")) 
		{		
			foreach(KeyValuePair<string,ServerLog> pair in OSCHandler._servers)
			{
				pair.Value.server.Close();
			}
			
			OSCHandler._instance = null;
			OSCHandler._servers.Clear();
			osclst.Clear(); 
			isConnected = false;
		}
 	GUI.enabled = true;		
		
		//copy address to clipboard
		if (GUI.Button (new Rect (30,120,150,20), "Copy IP to Clipboard"))
		{
			ClipboardHelper.clipBoard = LocalIPAddress();
		}
	
		GUI.EndGroup ();//end network group
		
     //   GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}
	
	
	void OnGUI () 
	{
		
if(MainGuiControls.NetMenu)
{	
			
		if(activeWin == false)
		{
			if (GUI.Button (new Rect (Screen.width - 220, Screen.height / 2-90, 200, 30), "OSC Server")) 
			{activeWin = true;}
			GUI.Label(new Rect(Screen.width - 50, Screen.height / 2-97, 40, 40), arrowDown);
		}
		else if (activeWin == true)
		{
			if (GUI.Button (new Rect (Screen.width - 220, Screen.height / 2-90, 200, 30), "OSC Server")) 
			{activeWin = false;}
			GUI.Label(new Rect(Screen.width - 50, Screen.height / 2-97, 40, 40), arrowUp);
		}
			
		if(activeWin)
		{
			windowRect0 = GUI.Window(0, windowRect0, OSCWindow, "", winStyle);	
		}	
			
//		//Network Group
//		GUI.BeginGroup (new Rect (Screen.width / 2 - 240-MainGuiControls.gone, (Screen.height / 2 + 60), 200, 260));
//		GUI.color = Color.yellow;
//		GUI.Box (new Rect (0,0,200,260), "OSC Server");
//		GUI.color = Color.white;
//		
//		//display local ip address
//		GUI.Label(new Rect(30, 30, 200, 20), "IP Address: "+LocalIPAddress());
//				
//		GUI.Label(new Rect(30, 65, 100, 20), "Port:");
//		GUI.enabled = !isConnected;
//		portField = GUI.TextField (new Rect (65, 65, 50, 20), portField);
//		GUI.enabled = true;
//		
//	GUI.enabled = !isConnected;				
//		//Start sending UDP
//		if (GUI.Button (new Rect (10, 100, 90, 30), "Start")) 
//		{
//			port = int.Parse(portField);			
//			OSCHandler.CreateServer("localhost",port);
//			print("Start OSC Server");
//			isConnected = true;
//		}
//	GUI.enabled = true;	
//		
//	GUI.enabled = isConnected;		
//		//Stop sending UDP
//		if (GUI.Button (new Rect (100, 100, 90, 30), "Stop")) 
//		{
//			foreach(KeyValuePair<string,ServerLog> pair in OSCHandler._servers)
//			{
//			pair.Value.server.Close();
//			}
//			
//			OSCHandler._instance = null;
//			OSCHandler._servers.Clear();
//			osclst.Clear();
//			isConnected = false;
//	
//		}
// 	GUI.enabled = true;
//		
//		
////		//copy address to clipboard
////		if (GUI.Button (new Rect (30,150,150,20), "Copy IP to Clipboard"))
////		{
////			ClipboardHelper.clipBoard = LocalIPAddress();
////		}
//				
//		
//		GUI.EndGroup (); // end network group
//		
//		
//		//list buttons of available joints dynamically
//		GUI.color = Color.yellow;
//		GUI.Label(new Rect(Screen.width/2-170-MainGuiControls.gone, (Screen.height / 2 + 200), 130, 200), "OSC Data:");
//		GUI.color = Color.white;
//		float yOffset = 0.0f;			
//		scrollPositiontrc = GUI.BeginScrollView(new Rect(Screen.width/2- 240-MainGuiControls.gone, (Screen.height / 2 + 210), 200, 110), scrollPositiontrc, new Rect(0, 0, 300, 150));
//			foreach(string osc in osclst)
//	        {
//GUI.enabled = UDPData.flag;			
//	           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(osc.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(osc.ToUpper())))
//				{
//					
//					print("Selected: " + osc);
//					selection = osc;
//
//	           	}	
//GUI.enabled = true;			
//	          yOffset += 25;
//	         }	
//        GUI.EndScrollView();
		}
		
		
	}//onGUI
	
	
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
	
	
	
}
