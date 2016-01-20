using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DevicesLists : MonoBehaviour {
	
	public Texture2D backIcon;
	public Texture2D arrowsIcon;
	
	public static List<string> availableDev = new List<string>();
	public static List<string> selectedDev = new List<string>();
	
	public Vector2 scrollPosition = Vector2.zero;//scrolling window
	public Vector2 scrollPosition2 = Vector2.zero;//scrolling window
	
 	public string devToSend = "RehabNetCP";
	public static string newDevice;
	private bool toggleDevice = false;
	public static bool device = false;
	
	float timer = 10;

	// Use this for initialization
	void Start () 
	{
		 
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//update available data list
		timer -= Time.deltaTime;
		if(timer <= 0)
		{
		//	checkList();
			availableDev.Clear();
//			XmlDataWriter.checkList();
			XmlDataWriter.logDev.Clear();
			timer = 10;
		}
		
		device = toggleDevice;
		//print("toggleDevice: "+device);
		//device name
		newDevice = devToSend;
	}
	
	void checkList()
	{
		if(selectedDev.Count != 0)	
			foreach (string dev in selectedDev)
			{
				if (!availableDev.Contains(dev))
					selectedDev.Remove(dev);
			}
	}
	
	
	void OnGUI()
	{
		if(MainGuiControls.NetMenu)
		{	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
			MainGuiControls.NetMenu = false;
			MainGuiControls.hideMenus = false;
			}	
			
		//list buttons of available joints dynamically
		GUI.Box(new Rect(Screen.width/2 - 450, Screen.height/2 - 260, 310, 330), " ");
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - 250, 130, 200), "Available Data:");
		GUI.color = Color.white;
		float yOffset = 0.0f;			
		scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2- 440, Screen.height/2 - 230, 280, 280), scrollPosition, new Rect(0, 0, 300, 320*(availableDev.Count/4)));
			foreach(string dev in UDPReceive.DataList)//udp data
//			foreach(string dev in UDPReceive.devicelst)
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(dev.Length*9), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dev.ToUpper())))
				{
					print("Selected: " + dev);
					
					if(!selectedDev.Contains(dev))
					selectedDev.Add(dev);//add to selected data list
	           	}			
	          yOffset += 25;
	         }
			foreach(string dev in availableDev)//local data
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(dev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dev.ToUpper())))
				{
					print("Selected: " + dev);
					
					if(!selectedDev.Contains(dev))
					selectedDev.Add(dev);//add to selected data list
	           	}			
	          yOffset += 25;
	         }		
        GUI.EndScrollView();
			
//			//refresh list button
//			if(GUI.Button (new Rect (Screen.width/2- 250, Screen.height/2+120, 80, 20), "Refresh"))
//			{
//				UDPReceive.clearLists();
//				availableDev.Clear();
//			}	

			
		//selected data
		GUI.Box(new Rect(Screen.width/2 - 60, Screen.height/2 - 260, 310, 330), " ");	
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2+ 60, Screen.height/2 - 250, 130, 200), "Selected Data:");
		GUI.color = Color.white;
		float yOffset2 = 0.0f;			
		scrollPosition2 = GUI.BeginScrollView(new Rect(Screen.width/2- 50, Screen.height/2 - 230, 280, 280), scrollPosition2, new Rect(0, 0, 300, 300+(selectedDev.Count*10)));
			foreach(string sdev in selectedDev)
	        {
//			   GUI.Label(new Rect (5, 20+ yOffset2, 10+(sdev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sdev.ToUpper()));
	           if(GUI.Button (new Rect (5, 20+ yOffset2, 10+(sdev.Length*10), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sdev.ToUpper())))
				{
					selectedDev.Remove(sdev);
					print("removing: " + sdev);
	           	}			
	          yOffset2 += 25;
			}
        GUI.EndScrollView();	
			
			//change device name
			toggleDevice = GUI.Toggle(new Rect(Screen.width/2, Screen.height/2 + 80, 100, 30), toggleDevice, "Send as: ");
			GUI.enabled = !UDPData.flag && toggleDevice;			
			//GUI.Label(new Rect(Screen.width/2+ 20, Screen.height/2 + 80, 130, 200), "Send as: ");
			devToSend = GUI.TextField(new Rect(Screen.width/2+ 80, Screen.height/2+80, 100, 20), devToSend, 25);
			GUI.enabled = true;	
			
			//clear list button
			if(GUI.Button (new Rect (Screen.width/2+ 50, Screen.height/2+120, 80, 20), "Clear List"))
			{
				selectedDev.Clear();
			}
			
			//send all available data to selected data
			if(GUI.Button (new Rect (Screen.width/2- 340, Screen.height/2+120, 80, 20), "Send All"))
			{
				foreach(string udev in UDPReceive.DataList)
				{
					selectedDev.Add(udev);
				}
				foreach(string adev in availableDev)
				{
					selectedDev.Add(adev);
				}
			}
			
			//arrows icon
			GUI.Label(new Rect (Screen.width/2 - 120, Screen.height/2 - 150, 50, 50), arrowsIcon);
			
		}//if net menu
		
	}//on gui
	
	
}
