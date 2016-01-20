using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class faceAPIGUI : MonoBehaviour {

	public Texture2D backIcon;

	public static string faceapiURL = "";
	static bool faceapiExists = false;
	static bool faceapi = false;

	string processOutput; 
	List<string> inputData = new List<string>();
	List<string> errorMsg = new List<string>();

	Process process2 = null;//faceapi
	StreamWriter messageStream;
	
	public Vector2 scrollPosition1 = Vector2.zero;//

	// Use this for initialization
	
	void Start () 
	{
		faceapiURL = ProgramFilesx86 () + @"\NeuroRehabLab\FaceAPI client";

		//check if directory exists
		if(Directory.Exists(faceapiURL)) 
		{
			faceapiExists = true;
		}
		else{faceapiExists = false;}		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(inputData.Count>1000)
		{
			inputData.Clear();
		}
		
		//check if process is running
		if(faceapi)
		{
			if( process2 == null || process2.HasExited )
			{
				faceapi = false;
				inputData.Clear();
				errorMsg.Add("Launch failed!");
			}
		}
		
	}

	static string ProgramFilesx86()
	{
		if( 8 == IntPtr.Size 
		   || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
		{
			return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		}
		
		return Environment.GetEnvironmentVariable("ProgramFiles");
	}

	void launchFaceAPI()
	{
		try
		{
			process2 = new Process();
			process2.EnableRaisingEvents = false;
			process2.StartInfo.FileName = faceapiURL+"\\Socket.exe";
			process2.StartInfo.UseShellExecute = false;
			process2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
			process2.StartInfo.RedirectStandardOutput = true;
			process2.StartInfo.RedirectStandardInput = true;
			process2.StartInfo.RedirectStandardError = true;
			process2.OutputDataReceived += new DataReceivedEventHandler( DataReceived );
			process2.ErrorDataReceived += new DataReceivedEventHandler( ErrorReceived );
			process2.Start();
			process2.BeginOutputReadLine();
			
			messageStream = process2.StandardInput;
			faceapi = true;
			UnityEngine.Debug.Log( "Successfully launched FaceAPI" );
			inputData.Add("launching FaceAPI client...");
		}
		catch( Exception e )
		{
			UnityEngine.Debug.LogError( "Unable to launch FaceAPI: " + e.Message );
			errorMsg.Add("Unable to launch FaceAPI: " + e.Message);
			faceapi = false;
		}
		
		
	}

	
	void DataReceived( object sender, DataReceivedEventArgs eventArgs )
	{
		#if UNITY_EDITOR
		UnityEngine.Debug.Log( eventArgs.Data );
		#endif
		processOutput = eventArgs.Data;
		inputData.Add(processOutput);
	}
	
	
	void ErrorReceived( object sender, DataReceivedEventArgs eventArgs )
	{
		#if UNITY_EDITOR
		UnityEngine.Debug.LogError( eventArgs.Data );
		#endif
		errorMsg.Add(eventArgs.Data);
		processOutput = eventArgs.Data;
		faceapi = false;
	}
	

	void killFaceAPI()
	{
		process2.CloseMainWindow();
		faceapi = false;
		inputData.Add("closing FaceAPI...");
		print("closing FaceAPI...");
	}	


	void OnApplicationQuit()
	{
		if( process2 != null && !process2.HasExited )
		{
			process2.CloseMainWindow();
		}		
	}


	void OnGUI()
	{
		if(MainGuiControls.faceapiMenu)
		{
			
			//back button	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.faceapiMenu = false;
				MainGuiControls.hideMenus = false;
			}	
			
			//	GUI.Label(new Rect(Screen.width/2 - 140, Screen.height -50, 300, 20), processOutput);
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width/2 - 70, Screen.height/2 - 225, 400, 20), "Console Output:");
			GUI.color = Color.white;
			float yOffset = 0.0f;
			GUI.Box(new Rect(Screen.width/2 - 260, Screen.height/2 - 200, 470, 300), " ");
			scrollPosition1 = GUI.BeginScrollView(new Rect(Screen.width/2 - 250, Screen.height/2 - 200, 460, 290), scrollPosition1, new Rect(0, 0, 300, 300+(inputData.Count*10)));			
			foreach (string indt in inputData)//input data
			{
				GUI.Label(new Rect(5, 10+ yOffset, 450, 20), indt);
				yOffset += 25;
			}
			foreach (string err in errorMsg)//error msg's
			{
				GUI.color = Color.red;
				GUI.Label(new Rect(5, 10+ yOffset, 450, 20), err);
				GUI.color = Color.white;
				yOffset += 25;
			}			
			GUI.EndScrollView();

			
			GUI.enabled = faceapiExists;//enable gui if URL exists
			if(!faceapi)
			{
				if (GUI.Button (new Rect (Screen.width/2 - 120, Screen.height / 2+180, 200, 30), "Launch faceAPI"))
				{
					launchFaceAPI();
				}			
			}
			else
			{
				if (GUI.Button (new Rect (Screen.width/2 - 120, Screen.height / 2+180, 200, 30), "Stop faceAPI"))
				{
					killFaceAPI();
				}						
			}
			GUI.enabled = true;
				
			
		}			
	}


}
