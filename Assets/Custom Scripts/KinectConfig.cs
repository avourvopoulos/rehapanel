using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class KinectConfig : MonoBehaviour {
	
	private string timestamp = "";
	string savetime = "";
	string XMLlongWord;

//	bool XMLSeatedMode;
//	bool XMLTrackSkeletonInNearMode;
//	bool XMLNearMode;
//	bool XMLMirror;
//		
//	float XMLSmoothing; 
//	float XMLCorrection; 
//	float XMLPrediction; 
//	float XMLJitterRadius; 
//	float XMLMaxDeviationRadius; 
	

	// Use this for initialization
	void Start () 
	{
		LoadFromXml();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timestamp = DateTime.Now.ToString("dd-MM-yyyy HH.mm.ss"); // get time	
	}
	
	void OnGUI()
    {
		
	if(MainGuiControls.OptionsMenu)
	{	
		
			GUI.BeginGroup (new Rect (10-KinectGUI.gone, (Screen.height / 2 - 120), 200, 100));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,100), "Current Configuration");
		GUI.color = Color.white;
		
		if (GUI.Button(new Rect(40, 30, 50, 20), "Save"))
        {
			WriteToXml();
			Debug.Log("Saved...");
		}
		
		if (GUI.Button(new Rect(110, 30, 50, 20), "Load"))
        {
			LoadFromXml();
			Debug.Log("Loaded...");
		}
		GUI.Label(new Rect(10, 60, 400, 20), "Last Save: "+savetime);
		
		GUI.EndGroup ();
			
		}//options menu	
	}
	
	public void WriteToXml()
 	{
		string path = Application.dataPath+ @"/Config/";
		string filepath = path +"/Config.conf";//save
				
  	  savetime=timestamp;
		
	  XmlDocument xmlDoc = new XmlDocument();
	  
	  if(File.Exists (filepath))
	  {
	   xmlDoc.Load(filepath);
	    
	   XmlElement elmRoot = xmlDoc.DocumentElement;
	    
	    elmRoot.RemoveAll(); // remove all inside the transforms node.
	    
	    XmlElement elm1 = xmlDoc.CreateElement("settings"); // create the node.
	    
	     XmlElement TimeXML = xmlDoc.CreateElement("Time"); 
	     TimeXML.InnerText = timestamp; //time 
			
		 XmlElement longWordXML = xmlDoc.CreateElement("longWord"); 
			longWordXML.InnerText = KinectGUI.longWord.ToString(); //int 
	    
	     XmlElement SeatedModeXML = xmlDoc.CreateElement("SeatedMode");
			SeatedModeXML.InnerText = KinectGUI.SeatedMode.ToString(); //bool
	    
	     XmlElement TrackSkeletonInNearModeXML = xmlDoc.CreateElement("TrackSkeletonInNearMode"); 
			TrackSkeletonInNearModeXML.InnerText = KinectGUI.TrackSkeletonInNearMode.ToString(); // bool
			
		 XmlElement NearModeXML = xmlDoc.CreateElement("NearMode");
			NearModeXML.InnerText = KinectGUI.NearMode.ToString(); //bool
			
		 XmlElement MirrorXML = xmlDoc.CreateElement("Mirror");
			MirrorXML.InnerText = KinectGUI.toggleMirror.ToString();//bool
			
		 XmlElement SmoothingXML = xmlDoc.CreateElement("Smoothing");
			SmoothingXML.InnerText = KinectGUI.Smoothing.ToString();//float
			
		 XmlElement CorrectionXML = xmlDoc.CreateElement("Correction");
			CorrectionXML.InnerText = KinectGUI.Correction.ToString();//float
			
		 XmlElement PredictionXML = xmlDoc.CreateElement("Prediction");
			PredictionXML.InnerText = KinectGUI.Prediction.ToString();//float
			
		 XmlElement JitterRadiusXML = xmlDoc.CreateElement("JitterRadius");
			JitterRadiusXML.InnerText = KinectGUI.JitterRadius.ToString();//float
			
		 XmlElement MaxDeviationRadiusXML = xmlDoc.CreateElement("MaxDeviationRadius");
			MaxDeviationRadiusXML.InnerText = KinectGUI.MaxDeviationRadius.ToString();//float	
	   
			
		XmlElement toggle1XML = xmlDoc.CreateElement("Head");
	    toggle1XML.InnerText = ZigSkeleton.toggle1.ToString();//bool
			
		XmlElement toggle2XML = xmlDoc.CreateElement("Neck");
	    toggle2XML.InnerText = ZigSkeleton.toggle2.ToString();//bool	
				
		XmlElement toggle3XML = xmlDoc.CreateElement("Torso");
	    toggle3XML.InnerText = ZigSkeleton.toggle3.ToString();//bool	
			
		XmlElement toggle4XML = xmlDoc.CreateElement("Waist");
	    toggle4XML.InnerText = ZigSkeleton.toggle4.ToString();//bool	
			
		XmlElement toggle5XML = xmlDoc.CreateElement("LeftShoulder");
	    toggle5XML.InnerText = ZigSkeleton.toggle5.ToString();//bool	
			
		XmlElement toggle6XML = xmlDoc.CreateElement("LeftElbow");
	    toggle6XML.InnerText = ZigSkeleton.toggle6.ToString();//bool	
			
		XmlElement toggle7XML = xmlDoc.CreateElement("LeftWrist");
	    toggle7XML.InnerText = ZigSkeleton.toggle7.ToString();//bool	
			
		XmlElement toggle8XML = xmlDoc.CreateElement("RightShoulder");
	    toggle8XML.InnerText = ZigSkeleton.toggle8.ToString();//bool	
			
		XmlElement toggle9XML = xmlDoc.CreateElement("RightElbow");
	    toggle9XML.InnerText = ZigSkeleton.toggle9.ToString();//bool	
			
		XmlElement toggle10XML = xmlDoc.CreateElement("RightWrist");
	    toggle10XML.InnerText = ZigSkeleton.toggle10.ToString();//bool	
			
			
		XmlElement IPXML = xmlDoc.CreateElement("IPAddress");
	    IPXML.InnerText = UDPData.ipField.ToString();//string	
			
		XmlElement PortXML = xmlDoc.CreateElement("Port");
	    PortXML.InnerText = UDPData.portField.ToString();//string			
			
			
	   elm1.AppendChild(TimeXML);	
	   elm1.AppendChild(longWordXML); 
	   elm1.AppendChild(SeatedModeXML); 
	   elm1.AppendChild(TrackSkeletonInNearModeXML);
	   elm1.AppendChild(NearModeXML);
	   elm1.AppendChild(MirrorXML);
	   elm1.AppendChild(SmoothingXML);
	   elm1.AppendChild(CorrectionXML);
	   elm1.AppendChild(PredictionXML);
	   elm1.AppendChild(JitterRadiusXML);
	   elm1.AppendChild(MaxDeviationRadiusXML);
			
		elm1.AppendChild(toggle1XML);
		elm1.AppendChild(toggle2XML);
			elm1.AppendChild(toggle3XML);
			elm1.AppendChild(toggle4XML);
			elm1.AppendChild(toggle5XML);
			elm1.AppendChild(toggle6XML);
			elm1.AppendChild(toggle7XML);
			elm1.AppendChild(toggle8XML);
			elm1.AppendChild(toggle9XML);
			elm1.AppendChild(toggle10XML);
			
		elm1.AppendChild(IPXML);
		elm1.AppendChild(PortXML);
				
	   elmRoot.AppendChild(elm1); // make the transform node the parent.			
	    
	   xmlDoc.Save(filepath); // save file.
	  }
	 }
	
  
	 public void LoadFromXml()
	 {
	  string filepath = Application.dataPath + @"/Config/Config.conf";
	  XmlDocument xmlDoc = new XmlDocument();
	  
	  if(File.Exists (filepath))
	  {
	   xmlDoc.Load(filepath);
	    
	   XmlNodeList transformList = xmlDoc.GetElementsByTagName("settings");
	  
	   foreach (XmlNode transformInfo in transformList)
	   {
	    XmlNodeList xmlcontent = transformInfo.ChildNodes;
	    
	    foreach (XmlNode xmlsettings in xmlcontent)
	    {
	     if(xmlsettings.Name == "Time")
	     {
	      savetime = xmlsettings.InnerText; 
	     }
	     if(xmlsettings.Name == "longWord")
	     {
						KinectGUI.longWord = xmlsettings.InnerText; 
	     }
	     if(xmlsettings.Name == "SeatedMode")
	     {
						KinectGUI.SeatedMode = bool.Parse(xmlsettings.InnerText); 
	     }
	     if(xmlsettings.Name == "TrackSkeletonInNearMode")
	     {
						KinectGUI.TrackSkeletonInNearMode = bool.Parse(xmlsettings.InnerText);
	     }
		if(xmlsettings.Name == "NearMode")
	     {
						KinectGUI.NearMode = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "Mirror")
	     {
						KinectGUI.toggleMirror = bool.Parse(xmlsettings.InnerText);					
		 }
		if(xmlsettings.Name == "Smoothing")
	     {
						KinectGUI.Smoothing = float.Parse(xmlsettings.InnerText);
		}
		if(xmlsettings.Name == "Correction")
	     {
						KinectGUI.Correction = float.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "Prediction")
	     {
						KinectGUI.Prediction = float.Parse(xmlsettings.InnerText);
		}
		if(xmlsettings.Name == "JitterRadius")
	     {
						KinectGUI.JitterRadius = float.Parse(xmlsettings.InnerText);
		}
		if(xmlsettings.Name == "MaxDeviationRadius")
	     {
						KinectGUI.MaxDeviationRadius = float.Parse(xmlsettings.InnerText);
		 }	
	      
		if(xmlsettings.Name == "Head")
	     {
	     ZigSkeleton.toggle1 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "Neck")
	     {
	     ZigSkeleton.toggle2 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "Torso")
	     {
	     ZigSkeleton.toggle3 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "Waist")
	     {
	     ZigSkeleton.toggle4 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "LeftShoulder")
	     {
	     ZigSkeleton.toggle5 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "LeftElbow")
	     {
	     ZigSkeleton.toggle6 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "LeftWrist")
	     {
	     ZigSkeleton.toggle7 = bool.Parse(xmlsettings.InnerText);
		 }		
		if(xmlsettings.Name == "RightShoulder")
	     {
	     ZigSkeleton.toggle8 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "RightElbow")
	     {
	     ZigSkeleton.toggle9 = bool.Parse(xmlsettings.InnerText);
		 }
		if(xmlsettings.Name == "RightWrist")
	     {
	     ZigSkeleton.toggle10 = bool.Parse(xmlsettings.InnerText);
		 }
					
					
		if(xmlsettings.Name == "IPAddress")
	     {
	     UDPData.ipField = xmlsettings.InnerText;
		 }
		if(xmlsettings.Name == "Port")
	     {
	     UDPData.portField = xmlsettings.InnerText;
		 }
					
		
	    }
	   }
	  }
	  //CubeObject.transform.eulerAngles =new Vector3(X,Y,Z); // Apply the values to the cube object.
	  
	 }
	
}
