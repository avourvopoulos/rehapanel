using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class PlotData : MonoBehaviour {
	
	public Texture2D backIcon;
	
	public Vector2 scrollPosition = Vector2.zero;//scrolling window
	
	string selectedData = "n/a";

	// Use this for initialization
	void Start () 
	{
        //  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
        PlotManager.Instance.PlotCreate("Datax", -1, 1, Color.green, new Vector2(200,100));	
		PlotManager.Instance.PlotCreate("Datay", -1, 1, Color.red, "Datax");
		PlotManager.Instance.PlotCreate("Dataz", -1, 1, Color.blue, "Datax");
		PlotManager.Instance.PlotCreate("Dataw", -1, 1, Color.magenta, "Datax");
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		plotUDPData();
	}
	
	void plotUDPData()
	{
		if(selectedData == UDPReceive.tempstr)
		{
			PlotManager.Instance.PlotAdd("Datax", UDPReceive.udpx);
			PlotManager.Instance.PlotAdd("Datay", UDPReceive.udpy);
			PlotManager.Instance.PlotAdd("Dataz", UDPReceive.udpz);
			PlotManager.Instance.PlotAdd("Dataw", UDPReceive.udpw);
//			for(int i=4; i<= UDPReceive.words.Length-1; i++)
//			{
//				PlotManager.Instance.PlotAdd("Data", float.Parse(UDPReceive.words[i]));
//				Debug.Log(UDPReceive.words[i]);
//			}
		}		
	}
	
	void OnGUI()
	{
		if(MainGuiControls.VisMenu)
		{	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
			MainGuiControls.VisMenu = false;
			MainGuiControls.hideMenus = false;
			}	
			
		//list buttons of available joints dynamically
		GUI.Box(new Rect(Screen.width/2 - 450, Screen.height/2 - 240, 310, 330), " ");
		GUI.color = Color.yellow;
		GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - 230, 130, 200), "Available Data:");
		GUI.color = Color.white;
		float yOffset = 0.0f;			
		scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2- 440, Screen.height/2 - 210, 280, 280), scrollPosition, new Rect(0, 0, 300, 320*(UDPReceive.DataList.Count/4)));
			foreach(string dev in UDPReceive.DataList)//udp data
	        {
	           if(GUI.Button (new Rect (5, 20+ yOffset, 10+(dev.Length*9), 20), System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dev.ToUpper())))
				{
					print("Plotting: " + dev);
					selectedData = dev;
	           	}			
	          yOffset += 25;
	         }
			GUI.EndScrollView();
		}
	}
	
}
