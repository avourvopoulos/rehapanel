using UnityEngine;
using System;
using System.Collections;

public class InitArguments : MonoBehaviour {
	
	public static string arg = "";


	// Use this for initialization
	void Awake () 
	{

#if UNITY_STANDALONE

		string[] arguments = Environment.GetCommandLineArgs();
		
//	       foreach(string arg in arguments)
//	       {
//	         cmdInfo += arg.ToString() + "@"+"\n ";
//	       }

//		arg = arguments[1];

		foreach(string arg in arguments){
			if(arg=="min")
			{
				PlayerPrefs.SetString("window", "min");
			}
			if(arg=="max")
			{
				PlayerPrefs.SetString("window", "max");
			}

			if(arg=="bci")
			{
				PlayerPrefs.SetString("bci", "on");
			}

		}

#endif		
	}
	

//	void OnGUI()
//	{
//		 GUI.Label(new Rect((Screen.width/2)-50, (Screen.height/2)-30, 100, 500), "args: "+ cmdInfo);
//	}
	
	
}