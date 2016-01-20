using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;

public class MyomoGUI : MonoBehaviour {
	
	public Texture2D elbow;
	public Texture2D myomoui;
	public Texture2D onoff, tricep, bicep, mode;
	public Texture2D lighton, lightoff;
	Texture2D tled1, tled2, tled3;//tricep leds
	Texture2D bled1, bled2, bled3;//bicep leds
	
	public Texture2D backIcon;
	
	public GUIStyle buttonstyle;
	
	string textFieldString; //serial monitor textfield
	
	private Vector2 scrollViewVector = Vector2.zero;
	
	public static string innerText = "Serial Monitor";
	
	public static string timestamp = "";
	
	int countb,countt=0;
	
	private bool toggleEmg = false;
	
	//-------------------------------------
	public static string serialnumber, arm, opmode;
	public static double battery;
	public static int encoderValue, romLimit, biEmgCalib, triEmgCalib;
	public static int tricepEMG, bicepEMG;
	
	//-------------------------------------

	// Use this for initialization
	void Start () 
	{
		tled1 = tled2 = tled3 = lightoff; 
		bled1 = bled2 = bled3 = lightoff;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		timestamp = DateTime.Now.ToString("HH:mm:ss"); // get time
	
		if (MyomoConnection.sp.IsOpen && toggleEmg && UDPData.flag)
			{
			//	print(MyomoFunctions.GetEMG()[0].ToString());
			//	print(MyomoFunctions.GetEMG()[1].ToString());
				UDPData.sendString("[$]EMG,[$$]Myomo,[$$$]Tricep,"+tricepEMG.ToString());
				UDPData.sendString("[$]EMG,[$$]Myomo,[$$$]Bicep,"+bicepEMG.ToString());
			}
	}
	
	
	IEnumerator GetData()
    {
        while(MyomoConnection.sp.IsOpen)
        {
            tricepEMG = MyomoFunctions.GetEMG()[0];
			Thread.Sleep(100);
			bicepEMG = MyomoFunctions.GetEMG()[1];
			Thread.Sleep(100);
			battery = MyomoFunctions.GetBatteryLevel();
			Thread.Sleep(100);
            yield return new WaitForSeconds(1);
        }
    }
	
	
	void OnGUI()
	{
		
	if(MainGuiControls.MyomoMenu)		
	{	
			
		if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
		{
			MainGuiControls.MyomoMenu = false;
			MainGuiControls.hideMenus = false;
		}	
			
		GUI.BeginGroup (new Rect (20-KinectGUI.gone, (Screen.height/2 -270), 200, 250));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,250), "Myomo Data");
		GUI.color = Color.white;
		
		GUI.Label (new Rect (20,30,200,20), "Battery: "+battery+" volts");
		GUI.Label (new Rect (20,60,200,20), "Tricep EMG: "+tricepEMG);
		GUI.Label (new Rect (20,90,200,20), "Bicep EMG: "+bicepEMG);
		
		GUI.EndGroup ();
		
		
			GUI.BeginGroup (new Rect (Screen.width / 2 - 200-KinectGUI.gone, (Screen.height/2 - 270), 400, 540));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,400,440), "Myomo mPower 1000");
		GUI.color = Color.white;
		
		//MYOMO UI
		GUI.Label (new Rect (10,40,400,380), myomoui);
		
		GUI.enabled = !MyomoConnection.sp.IsOpen;
		//comport field
		GUI.Label (new Rect (120,285, 300, 200), "COM Port:");
		MyomoConnection.comport = GUI.TextField (new Rect (190, 285, 80, 20), MyomoConnection.comport, 25);
		GUI.enabled = true;
		
		//on/off button
		if(GUI.Button(new Rect(150,310,100,100),onoff, buttonstyle))
			{
				if (MyomoConnection.sp.IsOpen)
				{
					//Disconnecting from Myomo
	            	MyomoConnection.CloseConnection();
					//switch LEDs off
					tled1 = tled2 = tled3 = lightoff; 
					bled1 = bled2 = bled3 = lightoff;
				}
				else
				{
					//Connecting to Myomo
	            	MyomoConnection.OpenConnection();
					Thread.Sleep(100);
					StartCoroutine(GetData());
				}
			}
		
		if (!MyomoConnection.sp.IsOpen)
		{
			GUI.Label (new Rect (250,345, 30, 30), lightoff);
		}
		else
		{
			GUI.Label (new Rect (250,345, 30, 30), lighton);
			
		}
		
		
		//tricep LEDs
		GUI.Label (new Rect (125,175, 30, 30), tled3);
		GUI.Label (new Rect (120,210, 30, 30), tled2);
		GUI.Label (new Rect (105,240, 30, 30), tled1);
		
		
		//tricept  button
		if(GUI.Button(new Rect(30,170,80,80),tricep, buttonstyle))
		{
			if (MyomoConnection.sp.IsOpen)
			{
				countt++;
	            if(countt > 3)
				{countt=0;}
				
				//led textures
				switch (countt)
				{
				case 0:
				//MyomoFunctions.SetTricepAssistLevel(0);
				innerText=timestamp+" "+MyomoFunctions.SetTricepAssistLevel(0);	
				tled1 = tled2 = tled3 = lightoff;
				Thread.Sleep(100);
				break;
			    case 1:
				//MyomoFunctions.SetTricepAssistLevel(1);
				innerText=timestamp+" "+MyomoFunctions.SetTricepAssistLevel(1);		
				tled1=lighton; tled2 = tled3 = lightoff;
				Thread.Sleep(100);	
				break;
			    case 2:
				//MyomoFunctions.SetTricepAssistLevel(2);	
				innerText=timestamp+" "+MyomoFunctions.SetTricepAssistLevel(2);		
				tled1= tled2 = lighton; tled3 = lightoff;
				Thread.Sleep(100);
				break;
				case 3:
				//MyomoFunctions.SetTricepAssistLevel(3);
				innerText=timestamp+" "+MyomoFunctions.SetTricepAssistLevel(3);		
				tled1 = tled2 = tled3 = lighton;
				Thread.Sleep(100);
				break;	
				}			
			}
		}
		
		//bicep LEDs
		GUI.Label (new Rect (253,175, 30, 30), bled3);
		GUI.Label (new Rect (250,210, 30, 30), bled2);
		GUI.Label (new Rect (265,240, 30, 30), bled1);

		//bicept  button
		if(GUI.Button(new Rect(285,170,80,80),bicep, buttonstyle))
		{
			if (MyomoConnection.sp.IsOpen)
			{
				countb++;
	            if(countb > 3)
				{countb=0;}
				
				//led textures
				switch (countb)
				{
				case 0:
				MyomoFunctions.SetBicepAssistLevel(0);	
				bled1 = bled2 = bled3 = lightoff;
				break;
			    case 1:
				MyomoFunctions.SetBicepAssistLevel(1);	
				bled1=lighton; bled2 = bled3 = lightoff;	
				break;
			    case 2:
				MyomoFunctions.SetBicepAssistLevel(2);	
				bled1= bled2 = lighton; bled3 = lightoff;	
				break;
				case 3:
				MyomoFunctions.SetBicepAssistLevel(3);	
				bled1 = bled2 = bled3 = lighton;
				break;	
				}	
			}
		}
		
		//mode  button
		if(GUI.Button(new Rect(155,52,90,90),mode, buttonstyle))
		{
			if (MyomoConnection.sp.IsOpen)
			{
			
			}
		}
		
		
		GUI.EndGroup ();
		
		
		
		// Begin the ScrollView
			scrollViewVector = GUI.BeginScrollView (new Rect ((Screen.width/2)-100-KinectGUI.gone, (Screen.height - 150), 200, 60), scrollViewVector, new Rect (0, 0, 250, 100));
		// Message inside the ScrollView
		innerText = GUI.TextArea (new Rect (0, 0, 250, 100), innerText);//ToDO: read log from a text file in drive
		// End the ScrollView
		GUI.EndScrollView();
		
		
		//////////////////////////////////////////////
		
		GUI.BeginGroup (new Rect (Screen.width - 212, (Screen.height/2 - 160), 200, 110));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,110), "Send Myomo Data");
		GUI.color = Color.white;
		
		 toggleEmg = GUI.Toggle(new Rect(40, 40, 100, 30), toggleEmg, "Send EMG");
		
		GUI.EndGroup ();
			
		}//myomo menu	
		
	}//end OnGUI
	
}//end MyomoGUI
