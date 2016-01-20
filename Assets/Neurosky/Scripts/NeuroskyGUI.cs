using UnityEngine;
using System.Collections;

public class NeuroskyGUI : MonoBehaviour {
	
	bool neurotoggle=false;
	public static bool neurostatus=false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//check if receiving data
		if (ThinkGearController.packetCount> 0)
		{neurostatus=true;}
		else{neurostatus=false;}
		
		//send udp data
		if(neurotoggle && UDPData.flag)
		{
		//	UDPData.sendString("[$]EEG,[$$]Neurosky,[$$$]head,orientation"+yaw.ToString()+","+pitch.ToString()+","+roll.ToString());
			print("sending neurodata...");
		}
	}
	
	void OnGUI() 
	{
		
		GUI.BeginGroup (new Rect (Screen.width / 2 - 200, (Screen.height/2 -270)*MainGuiControls.miscmenu, 350, 30));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,350,30),"Neurosky Headset");
		GUI.EndGroup ();
		
		GUI.BeginGroup (new Rect (Screen.width - 212, (Screen.height/2 - 40)*MainGuiControls.miscmenu, 200, 100));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,100), "Send Neurosky Data");
		GUI.color = Color.white;
		
		GUI.enabled = neurostatus;
		neurotoggle = GUI.Toggle(new Rect(40, 40, 100, 30), neurotoggle, "Neurosky Data");
		GUI.enabled = true;
			
		GUI.EndGroup ();
	}
	
	
}
