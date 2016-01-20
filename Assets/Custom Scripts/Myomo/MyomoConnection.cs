using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;

public class MyomoConnection : MonoBehaviour {	
	
	//Setup parameters to connect to Myomo NeuroRobotic Elbow
	public static string comport = "COM3"; // 
	public static SerialPort sp = new SerialPort(comport, 115200, Parity.None, 8, StopBits.One); //COM 3,6
	
    //Function connecting to Myomo
    public static void OpenConnection() 
    {	
       if (sp != null) 
       {
         if (sp.IsOpen) 
         {
          sp.Close();
				Debug.Log ("Closing port, because it was already open!");
				MyomoGUI.innerText=MyomoGUI.timestamp+" Closing port, because it was already open!";
         }
         else 
         {
		  sp = new SerialPort(comport, 115200, Parity.None, 8, StopBits.One); //COM 3,6		
          sp.Open();  // opens the connection
          sp.ReadTimeout = 500;  // sets the timeout value before reporting error
				Debug.Log ("Port Opened!");
				Debug.Log (sp.ReadLine());
				MyomoGUI.innerText=MyomoGUI.timestamp+" Port Opened!\r\n";
				
				//----------------------------------------
				//set all assistance levels to 0
				MyomoFunctions.SetTricepAssistLevel(0);
				MyomoFunctions.SetBicepAssistLevel(0);
				//----------------------------------------
         }
       }
       else 
       {
         if (sp.IsOpen)
         {
          Debug.Log("Port is already open");
		  MyomoGUI.innerText=MyomoGUI.timestamp+" Port is already open";
         }
         else 
         {
          Debug.Log("Port == null");
		  MyomoGUI.innerText=MyomoGUI.timestamp+" Port == null";
         }
       }
    }

	
	public static void CloseConnection() 
	{
	sp.Close();
	
		Debug.Log ("Disconnected");
		MyomoGUI.innerText=MyomoGUI.timestamp+" Disconnected\t\n";
		
	}

    void OnApplicationQuit() 
    {
       sp.Close();
    }
    
    
}
