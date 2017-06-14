using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class LaunchApps : MonoBehaviour {
	
	public Texture2D backIcon;
		
	public static string bitalinoURL = "";
	public static string faceapiURL = "";
	
	static bool bitalinoExists = false;
	static bool faceapiExists = false;
	
	static bool bitalino = false;
	static bool faceapi = false;

	public static bool bci2000 = false;
	
	string processOutput; 
	List<string> inputData = new List<string>();
	List<string> errorMsg = new List<string>();
	
	Process process1 = null;//bitalino
	Process process2 = null;//faceapi
	StreamWriter messageStream;
	
public Vector2 scrollPosition1 = Vector2.zero;//	

	// Use this for initialization
	void Awake () 
	{
		//LoadFromXml();
		faceapiURL = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\NeuroRehabLab\\FaceAPI client\\Socket.exe";
		bitalinoURL = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\BITalino client\\client.exe";
	//	print("faceapiURL: "+faceapiURL);
	}
	
	void Start () 
	{
		//check if directory exists
		if(File.Exists(bitalinoURL)) 
        {
			bitalinoExists = true;
		}
		else{bitalinoExists = false;}
		if(File.Exists(faceapiURL)) 
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
		if(bitalino)
		{
			if( process1 == null || process1.HasExited )
	        {
				bitalino = false;
				inputData.Clear();
				errorMsg.Add("Launch failed!");
			}
		}
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
	
	
	//start bitalino
	void launchBitalino()
    {
        try
        {
            process1 = new Process();
            process1.EnableRaisingEvents = false;
            process1.StartInfo.FileName = bitalinoURL;
            process1.StartInfo.UseShellExecute = false;
			process1.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process1.StartInfo.RedirectStandardOutput = true;
            process1.StartInfo.RedirectStandardInput = true;
            process1.StartInfo.RedirectStandardError = true;
            process1.OutputDataReceived += new DataReceivedEventHandler( DataReceived );
            process1.ErrorDataReceived += new DataReceivedEventHandler( ErrorReceived );
            process1.Start();
            process1.BeginOutputReadLine();

            messageStream = process1.StandardInput;
			bitalino = true;
			inputData.Add("launching bitalino client...");
			#if UNITY_EDITOR
            UnityEngine.Debug.Log( "Successfully launched bitalino" );
			#endif
        }
        catch( Exception e )
        {
			errorMsg.Add("Unable to launch bitalino: " + e.Message);
			bitalino = false;
			#if UNITY_EDITOR
            UnityEngine.Debug.LogError( "Unable to launch bitalino: " + e.Message );
			#endif
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
		bitalino = false;
		faceapi = false;
    }

 
	//kill bitalino
	void killBitalino()
	{
 		process1.CloseMainWindow();
//		process1.Kill();
		bitalino = false;
		inputData.Add("closing Bitalino...");
		print("closing Bitalino...");
	}
 

    void OnApplicationQuit()
    {
        if( process1 != null && !process1.HasExited )
        {
			process1.CloseMainWindow();
//          process1.Kill();
        }
        if( process2 != null && !process2.HasExited )
        {
			process2.CloseMainWindow();
        }		
    }
	


	void launchFaceAPI()
	{
       try
        {
            process2 = new Process();
            process2.EnableRaisingEvents = false;
            process2.StartInfo.FileName = faceapiURL;
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
		
	

	
	void killFaceAPI()
	{
 		process2.CloseMainWindow();
		faceapi = false;
		inputData.Add("closing FaceAPI...");
		print("closing FaceAPI...");
	}	
	
	
//	public void LoadFromXml()
//	{
//	  string filepath = Application.dataPath + @"/Config/LaunchApps.conf";
//	  XmlDocument xmlDoc = new XmlDocument();
//	  
//	 if(File.Exists (filepath))
//	 {
//	  	xmlDoc.Load(filepath);
//	 	   
//	  	XmlNodeList transformList = xmlDoc.GetElementsByTagName("folder");
//	  
//		  foreach (XmlNode transformInfo in transformList)
//		  {
//		 	 XmlNodeList xmlcontent = transformInfo.ChildNodes;
//		    
//			  foreach (XmlNode xmlsettings in xmlcontent)
//			  {
//			     if(xmlsettings.Name == "bitalino")
//			     {
//			     	bitalinoURL = xmlsettings.InnerText; 
//			     }
//			     if(xmlsettings.Name == "faceapi")
//			     {
//			     	faceapiURL = xmlsettings.InnerText; 
//			     }
//			  }
//		   }
//		}
//	 }//LoadFromXml
	
		
	
	void OnGUI()
	{
		if(MainGuiControls.thirdPartyMenu)
		{
			
			//back button	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.thirdPartyMenu = false;
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
			
			GUI.enabled = bitalinoExists;//enable gui if URL exists
			if(!bitalino)
			{
	         	if (GUI.Button (new Rect (Screen.width/2 - 120, Screen.height / 2+130, 200, 30), "Launch Bitalino"))
				{
					launchBitalino(); 
				}
			}
			else
			{
	         	if (GUI.Button (new Rect (Screen.width/2 - 120, Screen.height / 2+130, 200, 30), "Stop Bitalino"))
				{
					killBitalino();
				}				
			}
			GUI.enabled = true;
			
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

			if (GUI.Button (new Rect (Screen.width/2 - 120, Screen.height / 2+230, 200, 30), "BCI2000"))
			{
				//enter sub page
				bci2000 = true;
				MainGuiControls.thirdPartyMenu=false;
			}		
			
		}			
	}
	
	

	
}
