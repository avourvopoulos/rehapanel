using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class CheckUpdates : MonoBehaviour {

	public string url = "http://neurorehabilitation.m-iti.org/tools/downloads/rehabnetcp/version.txt";

	public string currentversion= "";
	public static string version;
	bool updates;
	public static bool internet = false;
	
	public static bool releaseVersion = true;
	public static bool clinicVersion = false;
	
	
	void Start()
	{
		StartCoroutine(checkVersion());//check for version

		releaseVersion = false;//dev version by default

		#if UNITY_EDITOR
//		using (StreamWriter writer = new StreamWriter(Application.dataPath+ @"/version.txt", true))
//		{
//			writer.Write(currentversion);
//			Debug.Log("current version: "+currentversion);
//		}
		#endif

		version = currentversion;

//		#if UNITY_EDITOR
//		releaseVersion = false;//dev version
//		//clinicVersion = true;
//		//releaseVersion = true;//release version
//		 #endif
//		
//		#if !UNITY_EDITOR
//		//dev or release version
//		if(InitArguments.arg=="dev")
//		{
//			releaseVersion = false;
//			clinicVersion = false;
//		}
//		else if(InitArguments.arg=="clinic")
//		{
//			releaseVersion = false;
//			clinicVersion = true;
//		}
//		else
//		{
//			releaseVersion = true;
//			clinicVersion = false;			
//		}
//		#endif
	}
	

	IEnumerator checkVersion() 
	{
        WWW www = new WWW(url);
        yield return www;
		
        string status = www.text;
		
		if(www.error != null)
	    {
	    //   print("faild to connect to internet, trying after 2 seconds.");
		  internet = false;
	      yield return new WaitForSeconds(2);// trying again after 2 sec
	      StartCoroutine(checkVersion());
	    }
		else
	    {
	    //   print("connected to internet");
			internet = true;
	       // check for version
				if(status == currentversion){ 
					updates=false;
				//	print("no updates");
				}else{
					updates=true;
				//	print("update available");
				}
	       yield return new WaitForSeconds(5);// recheck if the internet still exists after 5 sec
	       StartCoroutine(checkVersion());
	    }			
    }
	
	
	
	
	void OnGUI()
	{
		if(MainGuiControls.OptionsMenu)
		{	
		//	GUI.BeginGroup (new Rect (10, (Screen.height / 2 +100), 200, 90));
			GUI.BeginGroup (new Rect ((Screen.width/2-100)-KinectGUI.gone, (Screen.height / 2 +100), 200, 90));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,90), "Updates");
			GUI.color = Color.white;
			
			if(internet)
			{
			
				if(updates)
				{
					GUI.color = Color.green;
					GUI.Label(new Rect(45, 25, 200, 20), "Available Updates");
					GUI.color = Color.white;
						if (GUI.Button (new Rect (40,55,120,25), "Click to Download"))
						{
						Application.OpenURL("http://neurorehabilitation.m-iti.org/tools/rehabnetcp");
						}
				}
				else
				{
					GUI.color = Color.gray;
					GUI.Label(new Rect(40, 40, 200, 20), "No Updates Available");
				}
			}//if internet
			else
			{
				GUI.color = Color.red;
				GUI.Label(new Rect(65, 35, 200, 20), "No Internet Connection");
			}
			
			GUI.EndGroup ();
			
			//GUI.Label(new Rect(30, Screen.height / 2 +200, 200, 20), "Latest Build: "+DateTime.Now.ToString("dd-MM-yyyy"));
			
		//	GUI.Label(new Rect(30, Screen.height / 2 +200, 200, 20), "Build: "+currentversion);
			
		}//options menu
	
	}//gui
}
