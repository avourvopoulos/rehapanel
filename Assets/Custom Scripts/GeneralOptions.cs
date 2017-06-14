using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;

public class GeneralOptions : MonoBehaviour {
	
	public float capFps = 50.0f;
	int nFPS = 1;
	
	public static Process myProcess;
	
	private bool capFpsOn=true;
	
	private bool rehablogin=false;
	public string password = "MyPassword";
	private string username = "admin";
	bool logged=false;
	
	PerformanceCounter cpuCounter;
	PerformanceCounter ramCounter;
	
	string nCPU = string.Empty;
	string nRAM = string.Empty;
	bool tick = true;
	
	// Use this for initialization
	void Start () 
	{
	//	policyServer(); 

	}
	//get CPU and RAM load
	IEnumerator SystemStatus() {
		cpuCounter = new PerformanceCounter();
		cpuCounter.CategoryName = "Processor";
		cpuCounter.CounterName = "% Processor Time";
		cpuCounter.InstanceName = "_Total";
		ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		ramCounter.NextValue();//ram
        cpuCounter.NextValue(); //print("TICK");
		yield return new WaitForSeconds(1);
		ramCounter.NextValue();
		cpuCounter.NextValue();
		nRAM = ramCounter.NextValue().ToString();
		nCPU = cpuCounter.NextValue().ToString(); //print(cpuCounter.NextValue());
		tick = true;// print("TACK");
    }
	
	public string getCurrentCpuUsage(){
	        return nCPU+"%";
	}

	public string getAvailableRAM(){
            return nRAM+"MB";
	} 
	
	// Update is called once per frame
	void FixedUpdate () 
	{
//		if(tick)
//		{	
//		 StartCoroutine(SystemStatus());
//		 tick = false;
//		}
		
		if(!capFpsOn)
		{
			System.Threading.Thread.Sleep(1000/(int)capFps);//reduce framerate and CPU power
			
//			if(HUDFPS.fps < capFps)
//			{
//				nFPS = nFPS++;
//				System.Threading.Thread.Sleep(1000/nFPS);//reduce framerate and CPU power
//			}
//			else
//			{
//				nFPS = nFPS--;
//				System.Threading.Thread.Sleep(1000/nFPS);//reduce framerate and CPU power
//			}
		}
	
	}
	
	void OnGUI()
	{
		
//		GUI.Label(new Rect(Screen.width-250, 30, 80, 20), "CPU: "+getCurrentCpuUsage());
//		GUI.Label(new Rect(Screen.width-350, 30, 80, 20), "RAM: "+getAvailableRAM());
		
if(MainGuiControls.OptionsMenu)
{		
			
		//Events
//		GUI.Label(new Rect(Screen.width/2, Screen.height/2, 200, 20), "Mouse Pos: "+Event.current.mousePosition);
//		GUI.Label(new Rect(Screen.width/2, Screen.height/2+15, 200, 120), "Click Count: " + Event.current.clickCount);
//		GUI.Label(new Rect(Screen.width/2, Screen.height/2+30, 200, 120), "Key Pressed: " + Event.current.keyCode);
		
		// Group1
//		GUI.BeginGroup (new Rect (10, (UnityEngine.Screen.height/2 - 280), 200, 150));
//		GUI.color = Color.yellow;
//		GUI.Box (new Rect (0,0,200,150), "Options");
//		GUI.color = Color.white;
//		
//		capFpsOn = GUI.Toggle(new Rect(130, 25, 100, 20), capFpsOn, "Off");
//		
//	GUI.enabled = !capFpsOn;
//		GUI.Label(new Rect(20, 50, 120, 20), "Reduce Framerate");
//		capFps = GUI.HorizontalSlider(new Rect (20, 80, 150, 30), capFps, 60.0f, 10.0f);
//		GUI.color = Color.gray;
//		GUI.Label(new Rect(80, 95, 100, 20), capFps.ToString("0"));
//		GUI.color = Color.white;
//		GUI.Label(new Rect(20, 90, 200, 20), "Max");
//		GUI.Label(new Rect(150, 90, 200, 20), "Min");
//		GUI.color = Color.red;
//		GUI.Label(new Rect(20, 120, 200, 20), "WARNING! Unstable");
//		GUI.color = Color.white;
//	GUI.enabled = true;
//		
//		GUI.EndGroup();
		
		
		
		
		// Connect to RehabNet Social Network
//		GUI.BeginGroup (new Rect ((Screen.width/2-110)-KinectGUI.gone, (Screen.height/2 - 240), 220, 160));
//		GUI.color = Color.gray;
//		GUI.Box (new Rect (0,0,220,160), "RehabNet Social Network");
//		GUI.color = Color.white;
//		
//	GUI.enabled = logged;//rehablogin;
//		
//		GUI.Label(new Rect(10, 40, 200, 20), "Username");
//		username = GUI.TextField (new Rect (80, 40, 50, 20), username);
//		
//		GUI.Label(new Rect(10, 70, 200, 20), "Password");
//		password = GUI.PasswordField(new Rect(80, 70, 120, 20), password, "*"[0], 25);
//	
////	GUI.enabled = true;
//		
//		if(!logged)
//		{
//			if (GUI.Button (new Rect (60, 110, 90, 30), "Login")) 
//			{
//				//loged in
//				logged=true;
//			}
//		}
//		else{
//			if (GUI.Button (new Rect (60, 110, 90, 30), "Logout")) 
//			{
//				//loged out
//				logged=false;
//			}
//		}
//		
//	GUI.enabled = true;
//		
//		GUI.EndGroup();
		
		}//options menu				
	}
	
	
//	public string LocalIPAddress()
//	 {
//		if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
//    	{
//        return null;
//    	}
//		
//	   IPHostEntry host;
//	   string localIP = "";
//	   host = Dns.GetHostEntry(Dns.GetHostName());
//	   foreach (IPAddress ip in host.AddressList)
//	   {
//	     if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
//	     {
//	       localIP = ip.ToString();
//	     }
//	   }
//	   return localIP;
//	 }
	
	
	public static void policyServer()
	{	
		myProcess = new Process();
		myProcess.StartInfo.FileName = "socpl.exe";
//		myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.CreateNoWindow = true;		
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		myProcess.Start();

		print("Launching Policy Server");	    
	}
		
	
	public static void killPolicyServer()
	{
 		myProcess.CloseMainWindow();
		print("Killing Policy Server");
	}
	
	
	void OnApplicationQuit() 
		{
			if(UDPData.flag==true)
			{
	         killPolicyServer();
			}
		}
	
	
	
}
