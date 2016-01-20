using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class ModelVisualisation : MonoBehaviour {
	
	
//	new Vector3 initRotation = new Vector3(0,180,0);
	
//	List <string>bodyparts = new List<string>(); //child objects and bodyparts list with strings
	
//	public Camera FrontCam;
	
//	public static bool Head; 
//    public static bool Neck;
//    public static bool Torso;
//    public static bool Waist;
//    
//    public static bool LeftShoulder;
//    public static bool LeftElbow;
//    public static bool LeftWrist;
//
//    public static bool RightShoulder;
//    public static bool RightElbow;
//    public static bool RightWrist;
//
//    public static bool LeftHip;
//    public static bool LeftKnee;
//    public static bool LeftAnkle;
//
//    public static bool RightHip;
//    public static bool RightKnee;
//    public static bool RightAnkle;
	
	string filepath;
	XmlDocument xmlDoc = new XmlDocument();
	
	string objname = string.Empty;
	string jointtype = string.Empty;
	string jointname = string.Empty;
	
	string posx, posy, posz, rotx, roty, rotz, scalex, scaley, scalez, gain = string.Empty;
	
	public static Vector3 biasposition, biasrotation,biasgain = new Vector3(0,0,0); 
	public static Quaternion nrotation, modelrotation,finalGain;
									
	
	// Use this for initialization
	void Start ()
	{	
		filepath = Application.dataPath + @"/Data/modelxmldata.xml";
		
		if(File.Exists (filepath))
		{
			xmlDoc.Load(filepath); 
		}	
		
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		createChildrenList();
	}
	

	
	void createChildrenList()	
	{
		

		Transform[] childrenOnthisModel = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform child in childrenOnthisModel)
		{    
			
			objname = child.transform.name; //switching object name to string
				
//			bodyparts.Add(objname); //adding the parts to the list				
					
			XmlNodeList xnList = xmlDoc.SelectNodes("/Transformations/Type"); 
			foreach (XmlNode xn in xnList)
			{
				
			  jointtype = xn["JointType"].InnerText; //e.g. Head
			  jointname = xn["JointName"].InnerText; //e.g. head21

			
				if (jointname==objname)
				{
					posx = xn["positionX"].InnerText;
			  		posy = xn["positionY"].InnerText;
			  		posz = xn["positionZ"].InnerText;
					
					rotx = xn["rotationX"].InnerText;
			  		roty = xn["rotationY"].InnerText;
			  		rotz = xn["rotationZ"].InnerText;
					
					scalex = xn["scaleX"].InnerText;
			  		scaley = xn["scaleY"].InnerText;
			  		scalez = xn["scaleZ"].InnerText;
					
					gain = xn["gain"].InnerText;
					
				biasposition  = new Vector3(float.Parse(posx),float.Parse(posy),float.Parse(posz));
				biasrotation  = new Vector3(float.Parse(rotx),float.Parse(roty),float.Parse(rotz)); 
				biasgain = 	new Vector3(float.Parse(gain),float.Parse(gain),float.Parse(gain));
					
					modelrotation = Quaternion.Euler(biasrotation); // from euler angles to quaternion
			//		Quaternion udprotation = GameObject.FindGameObjectWithTag("UDP"+jointtype).transform.rotation;
					nrotation = GameObject.FindGameObjectWithTag("Ghost"+jointtype).transform.rotation;
					
					finalGain = Quaternion.Euler(biasgain); 
																							
//				//Apply Position Data	
//					
//	//				GameObject.Find(jointname).transform.position = GameObject.FindGameObjectWithTag("UDP"+jointtype).transform.position + biasposition + new Vector3(3,0,0);
////				 if (biasposition != Vector3.zero)
////					{
//				GameObject.Find(jointname).transform.position = GameObject.Find(jointname).transform.position + biasposition;
////					}
//				//Apply Rotation Data				
////					GameObject.Find(jointname).transform.rotation = (nrotation*modelrotation)*finalGain;			
//					
//				// Apply Scale	
//					GameObject.Find(jointname).transform.localScale = new Vector3(float.Parse(scalex),float.Parse(scaley),float.Parse(scalez));
//					
//
//				//		GameObject.FindGameObjectWithTag("UDPBody").transform.rotation = (GameObject.FindGameObjectWithTag("GhostBody").transform.rotation*modelrotation)*finalGain;
//						GameObject.Find("Dana").transform.rotation = (GameObject.FindGameObjectWithTag("GhostBody").transform.rotation*modelrotation)*finalGain;					
//		
//							//Head
//								if (ZigSkeleton.toggle1 == true)
//								{
//				//		GameObject.FindGameObjectWithTag("UDPHead").transform.rotation = (GameObject.FindGameObjectWithTag("GhostHead").transform.rotation*modelrotation)*finalGain;			
//									GameObject.Find("Head").transform.rotation = (GameObject.FindGameObjectWithTag("GhostHead").transform.rotation*modelrotation)*finalGain;
//								}
//								
//							// Neck
//								if (ZigSkeleton.toggle2 == true)
//								{
//									GameObject.Find("Neck").transform.rotation = (GameObject.FindGameObjectWithTag("GhostNeck").transform.rotation*modelrotation)*finalGain;
//								}
//								
//							// Torso
//								if (ZigSkeleton.toggle3 == true)
//								{
//									GameObject.Find("Spine1").transform.rotation = (GameObject.FindGameObjectWithTag("GhostTorso").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							//Waist
//								if (ZigSkeleton.toggle4 == true)
//								{
//									GameObject.Find("Spine").transform.rotation = (GameObject.FindGameObjectWithTag("GhostWaist").transform.rotation*modelrotation)*finalGain;
//								}	
//							
//							// LeftShoulder
//								if (ZigSkeleton.toggle5 == true)
//								{
//									GameObject.Find("LeftArm").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftShoulder").transform.rotation*modelrotation)*finalGain;
//								//		GameObject.Find("LeftArm").transform.rotation = new Quaternion(lowPass(lsin,lsout)[0],lowPass(lsin,lsout)[1],lowPass(lsin,lsout)[2],lowPass(lsin,lsout)[3]);
//								}
//										
//							// LeftElbow
//								if (ZigSkeleton.toggle6 == true)
//								{
//
//									GameObject.Find("LeftForeArm").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftElbow").transform.rotation*modelrotation)*finalGain;
//								//	GameObject.Find("LeftForeArm").transform.rotation = new Quaternion(lowPass(lein,leout)[0],lowPass(lein,leout)[1],lowPass(lein,leout)[2],lowPass(lein,leout)[3]);
//								}
//										
//							// LeftWrist
//								if (ZigSkeleton.toggle7 == true)
//								{	
//									GameObject.Find("LeftHand").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftWrist").transform.rotation*modelrotation)*finalGain;
//								//	GameObject.Find("LeftHand").transform.rotation = new Quaternion(lowPass(lwin,lwout)[0],lowPass(lwin,lwout)[1],lowPass(lwin,lwout)[2],lowPass(lwin,lwout)[3]);
//								}
//									
//							// RightShoulder
//								if (ZigSkeleton.toggle8 == true)
//								{	
//									GameObject.Find("RightArm").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightShoulder").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// RightElbow
//								if (ZigSkeleton.toggle9 == true)
//								{	
//									GameObject.Find("RightForeArm").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightElbow").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// RightWrist
//								if (ZigSkeleton.toggle10 == true)
//								{	
//									GameObject.Find("RightHand").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightWrist").transform.rotation*modelrotation)*finalGain;
//								}
//							
//							// LeftHip
//								if (ZigSkeleton.toggle11 == true)
//								{	
//									GameObject.Find("LeftUpLeg").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftHip").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// LeftKnee
//								if (ZigSkeleton.toggle12 == true)
//								{	
//									GameObject.Find("LeftLeg").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftKnee").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// LeftAnkle
//								if (ZigSkeleton.toggle13 == true)
//								{	
//									GameObject.Find("LeftFoot").transform.rotation = (GameObject.FindGameObjectWithTag("GhostLeftAnkle").transform.rotation*modelrotation)*finalGain;
//								}	
//							
//							// RightHip
//								if (ZigSkeleton.toggle14 == true)
//								{	
//									GameObject.Find("RightUpLeg").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightHip").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// RightKnee
//								if (ZigSkeleton.toggle15 == true)
//								{	
//									GameObject.Find("RightLeg").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightKnee").transform.rotation*modelrotation)*finalGain;
//								}
//									
//							// RightAnkle
//								if (ZigSkeleton.toggle16 == true)
//								{	
//									GameObject.Find("RightFoot").transform.rotation = (GameObject.FindGameObjectWithTag("GhostRightAnkle").transform.rotation*modelrotation)*finalGain;
//								}
//			
//					
				} // end if
				
			
			}//end foreach xmlnode
							
		
	    } //end foreach	Transform child in childrenOnthisModel	
		
		
	}
		
				


} //END
