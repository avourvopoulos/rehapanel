using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO.Ports;
//using System.Runtime.InteropServices;
//using System.Management;

public class SerialPortList : MonoBehaviour {
	
//	 [DllImport("Management.dll")]
	static List<string> comports = new List<string>();

	// Use this for initialization
	void Start () 
	{
		
		    string[] ports = SerialPort.GetPortNames();

            // add each port name to a list. 
            foreach(string port in ports)
            {
                //print(port);
				comports.Add(port);
            }
		
		
//		ManagementObjectCollection ManObjReturn;
//        ManagementObjectSearcher ManObjSearch;
//        ManObjSearch = new ManagementObjectSearcher("Select * from Win32_SerialPort");
//        ManObjReturn = ManObjSearch.Get();
//
//        foreach (ManagementObject ManObj in ManObjReturn)
//        {
//            //int s = ManObj.Properties.Count;
//            //foreach (PropertyData d in ManObj.Properties)
//            //{
//            //    MessageBox.Show(d.Name);
//            //}
//           print(ManObj["DeviceID"].ToString());
//            print(ManObj["PNPDeviceID"].ToString());
//              print(ManObj["Name"].ToString());
//               print(ManObj["Caption"].ToString());
//               print(ManObj["Description"].ToString());
//               print(ManObj["ProviderType"].ToString());
//               print(ManObj["Status"].ToString());
//
//        }

         
	}
	
	void OnGUI()
	{ 
		GUI.Label (new Rect (30-KinectGUI.gone, (Screen.height/2 + 50)*MainGuiControls.myomomenu,400,380), "Available Ports: ");
			int offset=0;
            // Display each port name. 
            foreach(string com in comports)
            {
			GUI.Label (new Rect (50-KinectGUI.gone, (Screen.height/2 + 70+offset)*MainGuiControls.myomomenu,400,380), com);
				offset=offset+20;
            }
	}
	
	
}
