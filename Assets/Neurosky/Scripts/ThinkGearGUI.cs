using UnityEngine;
using System.Collections;

public class ThinkGearGUI : MonoBehaviour {

enum AppState {
  Disconnected = 0,
  Connecting,
  Connected
}

public string portName;
	
public Texture2D backIcon;

private bool showErrorWindow = false;
private bool showConnectedWindow = false;
private bool showDisconnectedWindow = false;
private AppState state = AppState.Disconnected;
private Hashtable headsetValues;
private Rect windowRect = new Rect(100, 100, 150, 100);

public int miscmenu = 0;
//private var csScript : csscript;

string DeviceName = string.Empty;
public static bool neuroskyConnected = false;

void Awake()  
{  
    //Get the CSharp Script  
  //  csScript = this.GetComponent("csscript"); //Don't forget to place the 'CSharp1' file inside the 'Standard Assets' folder  
} 

 void Update()
 {
 	//miscmenu=csScript.miscmenu;

		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "neurosky";
		}


		if(state == AppState.Connected && headsetValues !=null)
		{
			foreach(string key in headsetValues.Keys)
			{
				if(!DevicesLists.availableDev.Contains("NEUROSKY:ANALOG:"+key.ToUpperInvariant()+":SIGNAL"))
				{
					DevicesLists.availableDev.Add("NEUROSKY:ANALOG:"+key.ToUpperInvariant()+":SIGNAL");		
				}
				if(DevicesLists.selectedDev.Contains("NEUROSKY:ANALOG:"+key.ToUpperInvariant()+":SIGNAL") && UDPData.flag==true)
				{					
					UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]"+key.ToUpperInvariant()+","+"signal,"+ headsetValues[key] +";"); 
				//	Debug.Log("[$]tracking,[$$]"+DeviceName+",[$$$]"+key.ToUpperInvariant()+","+"signal,"+ headsetValues[key] +";");
				}

			}

			neuroskyConnected = true;
		}
		else{
			neuroskyConnected = false;
		}
	

 } 
	


	void OnGUI()
	{
		if(MainGuiControls.NeuroskyMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.NeuroskyMenu = false;
				MainGuiControls.hideMenus = false;
			}		
			
			GUI.BeginGroup (new Rect (Screen.width / 2 - 200, (Screen.height/2 -240), 350, 420));
			GUI.Box (new Rect (0,0,350,420)," ");
			
			  GUILayout.BeginHorizontal();
			
			  switch(state){
			    case AppState.Disconnected:
			      // display UI for the user to enter in the port name and connect
			      GUILayout.Label("Port name:");                       
			      portName = GUILayout.TextField(portName, GUILayout.Width(150));
			     
			      if(GUILayout.Button("Connect")){
			        state = AppState.Connecting;
					SendMessage("OnHeadsetConnectionRequest", "\\\\.\\"+portName);
			      }
			
			      break;
			
			    case AppState.Connecting:
			      GUILayout.Label("Connecting...");
			      break;
			      
			    case AppState.Connected:
			      // display UI to allow a user to disconnect
			      GUILayout.Label("Connected");
			
			      if(GUILayout.Button("Disconnect"))
			        SendMessage("OnHeadsetDisconnectionRequest");
			
			      break;
			  }
			
			  GUILayout.EndHorizontal();
			
			  // only output the headset data if the headset is
			  // connected and transmitting data
			  if(state == AppState.Connected && headsetValues !=null){
			    foreach(string key in headsetValues.Keys){
			     //  vvalue = headsetValues[key];
			      GUILayout.Label(key + ": " + headsetValues[key]);
			    }
			  }
			
			  if(showErrorWindow)
			    GUILayout.Window(0, windowRect, ErrorWindow, "Error");
			
			  if(showConnectedWindow)
			    GUILayout.Window(0, windowRect, ConnectedWindow, "Connected");
			
			  if(showDisconnectedWindow)
			    GUILayout.Window(0, windowRect, DisconnectedWindow, "Disconnected");
			       
			GUI.EndGroup ();
			
		}//if neurosky menu
	}

/*
 * Event listeners
 */

void OnHeadsetConnected(){
  showConnectedWindow = true;
  state = AppState.Connected;
}

void OnHeadsetConnectionError(){
  showErrorWindow = true;
  state = AppState.Disconnected;
}

void OnHeadsetDisconnected(){
  showDisconnectedWindow = true;
  state = AppState.Disconnected;
}

void OnHeadsetDataReceived(Hashtable values){
  headsetValues = values;
}

/**
 * Disconnect the headset when the application quits.
 */
void OnApplicationQuit(){
  SendMessage("OnHeadsetDisconnectionRequest");
}

/*
 * Status windows
 */

void ErrorWindow(int windowID){
  GUILayout.Label("There was a connection error.");
  
  if(GUILayout.Button("Close"))
    showErrorWindow = false;
}

void ConnectedWindow(int windowID){
  GUILayout.Label("The headset has been successfully connected.");

  if(GUILayout.Button("Okay"))
    showConnectedWindow = false;
}

void DisconnectedWindow(int windowID){
  GUILayout.Label("The headset has been disconnected.");

  if(GUILayout.Button("Okay"))
    showDisconnectedWindow = false;
	}
		
}
