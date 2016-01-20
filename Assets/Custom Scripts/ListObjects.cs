using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class ListObjects : MonoBehaviour {
	
	public GameObject model;
	
//	new Vector3 initRotation = new Vector3(0,180,0);
	
	List <string>bodyparts = new List<string>(); //child objects and bodyparts list with strings
	
	
	// Use this for initialization
	void Start ()
	{
	//	tPose();
//		model.transform.position =  new Vector3(3,0,0);
//		model.transform.rotation = Quaternion.Euler(initRotation);
	//	GameObject.model.transform.rotation

//		Debug.Log("List: "+bodyparts.Count); //print total number of children
	}
	
	// Update is called once per frame
	void Update () 
	{
	 createChildrenList();
	}
	
/*void OnGUI () 
	{
		if (GUI.Button(new Rect(10, 70, 50, 30), "Debug"))
			{
			createChildrenList();
            Debug.Log("createChildrenList");
			}
	}*/
	
/*	
	void tPose()
	{
		string filepath = Application.dataPath + @"/Data/modelxmldata.xml";
		XmlDocument xmlDoc = new XmlDocument(); 
		
		if(File.Exists (filepath))
		{
			xmlDoc.Load(filepath); 
		}	
		
		Transform[] childrenOnthisModel = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform child in childrenOnthisModel)
		{    
			
		string objname = child.transform.name; //switching object name to string
				
			bodyparts.Add(objname); //adding the parts to the list				
			
			
			
			XmlNodeList xnList = xmlDoc.SelectNodes("/Transformations/Type"); 
			foreach (XmlNode xn in xnList)
			{
				
			  string jointtype = xn["JointType"].InnerText; //e.g. Head
			  string jointname = xn["JointName"].InnerText; //e.g. head21

			
				if (jointname==objname)
				{
					string posx = xn["positionX"].InnerText;
			  		string posy = xn["positionY"].InnerText;
			  		string posz = xn["positionZ"].InnerText;
					
					
				Vector3 initposition  = new Vector3(float.Parse(posx),float.Parse(posy),float.Parse(posz));
			
																				
				//Apply T-Pos Data	
					
					GameObject.Find(jointname).transform.position = initposition + new Vector3(3,0,0);
					
				}
			}
		}
	}// End T-pose
*/	
	
	void createChildrenList()	
	{
		
		string filepath = Application.dataPath + @"/Data/modelxmldata.xml";
		XmlDocument xmlDoc = new XmlDocument(); 
		
		if(File.Exists (filepath))
		{
			xmlDoc.Load(filepath); 
		}	
		
		Transform[] childrenOnthisModel = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform child in childrenOnthisModel)
		{    
			
		string objname = child.transform.name; //switching object name to string
				
			bodyparts.Add(objname); //adding the parts to the list				
			
			
			
			XmlNodeList xnList = xmlDoc.SelectNodes("/Transformations/Type"); 
			foreach (XmlNode xn in xnList)
			{
				
			  string jointtype = xn["JointType"].InnerText; //e.g. Head
			  string jointname = xn["JointName"].InnerText; //e.g. head21

			
				if (jointname==objname)
				{
					string posx = xn["positionX"].InnerText;
			  		string posy = xn["positionY"].InnerText;
			  		string posz = xn["positionZ"].InnerText;
					
					string rotx = xn["rotationX"].InnerText;
			  		string roty = xn["rotationY"].InnerText;
			  		string rotz = xn["rotationZ"].InnerText;
					
					string scalex = xn["scaleX"].InnerText;
			  		string scaley = xn["scaleY"].InnerText;
			  		string scalez = xn["scaleZ"].InnerText;
					
					string gain = xn["gain"].InnerText;
					
				Vector3 biasposition  = new Vector3(float.Parse(posx),float.Parse(posy),float.Parse(posz));
				Vector3 biasrotation  = new Vector3(float.Parse(rotx),float.Parse(roty),float.Parse(rotz)); 
				Vector3 biasgain = 	new Vector3(float.Parse(gain),float.Parse(gain),float.Parse(gain));
					
					Quaternion modelrotation = Quaternion.Euler(biasrotation); // from euler angles to quaternion
			//		Quaternion udprotation = GameObject.FindGameObjectWithTag("UDP"+jointtype).transform.rotation;
					Quaternion nrotation = GameObject.FindGameObjectWithTag("Ghost"+jointtype).transform.rotation;
					
					Quaternion finalGain = Quaternion.Euler(biasgain); 
				
																				
				//Apply Position Data	
					
	//				GameObject.Find(jointname).transform.position = GameObject.FindGameObjectWithTag("UDP"+jointtype).transform.position + biasposition + new Vector3(3,0,0);
				 if (biasposition != Vector3.zero)
					{
					GameObject.Find(jointname).transform.position = biasposition + new Vector3(3,0,0);
					}
				//Apply Rotation Data				
//					GameObject.Find(jointname).transform.rotation = (udprotation*modelrotation)*finalGain; 
					 GameObject.Find(jointname).transform.rotation = (nrotation*modelrotation)*finalGain; 
					
					
				// Apply Scale	
					GameObject.Find(jointname).transform.localScale = new Vector3(float.Parse(scalex),float.Parse(scaley),float.Parse(scalez));
					
					
					
				} // end if
				
			
			}//end foreach xmlnode
							
		
	    } //end foreach	Transform child in childrenOnthisModel	
		
		
	}
		
	

} //END
