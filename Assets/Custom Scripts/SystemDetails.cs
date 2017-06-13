using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class SystemDetails : MonoBehaviour {

	public GameObject kinect2;
	public static bool win8 = false;
	string cpuArch = "";

	// Use this for initialization
	void Start () 
	{
		 if(is64Bit())
			{
			cpuArch = "64 Bit";	
			}
			else
			{
			cpuArch = "32 Bit";
			}

		if (SystemInfo.operatingSystem.ToString ().Contains ("Windows 8")|| SystemInfo.operatingSystem.ToString ().Contains ("Windows 10")) {
			//kinect2.SetActive (true); 
			win8 = true; }
		else{
			//kinect2.SetActive (false);
			win8 = false;}

	//	Debug.Log("CPU: "+cpuArch);
	//	UnityEngine.Debug.Log(SystemInfo.operatingSystem);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI() 
	{
   	if(MainGuiControls.OptionsMenu)
	{     
		// System Info
			GUI.BeginGroup (new Rect (Screen.width / 2 -200-KinectGUI.gone, (Screen.height / 2 + 170), 400, 170));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,400,170), "System Info");
		GUI.color = Color.gray;
		
		GUI.Label(new Rect(20, 35, 250, 20), "- OS: " + SystemInfo.operatingSystem);
		GUI.Label(new Rect(20, 60, 250, 20), "- CPU: " + SystemInfo.processorType); 
			GUI.Label(new Rect(300, 60, 50, 20), "- " + cpuArch);
		GUI.Label(new Rect(20, 85, 250, 20), "- System Memory: " + SystemInfo.systemMemorySize + " MB");
		GUI.Label(new Rect(20, 110, 250, 20), "- Graphics: " + SystemInfo.graphicsDeviceName);
		GUI.Label(new Rect(20, 135, 250, 20), "- Graphics Memory: " + SystemInfo.graphicsMemorySize + " MB");
		
		GUI.EndGroup();
			
		}
    }
	
	//returns a boolean if the CPU Arch. is x64 or x86
	public static bool is64Bit()
    {
        string pa = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");

        return ((System.String.IsNullOrEmpty(pa) || pa.Substring(0, 3) == "x86") ? false : true);
    }
	
	
	

	
	
	
}
