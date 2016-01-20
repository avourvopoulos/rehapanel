using UnityEngine;
using System.Collections;

public class LeapMotionGUI : MonoBehaviour {

	public Camera LeapCamera;

	public Texture2D backIcon;

	public GameObject handGO;
	public GameObject handGO2;

	public static bool toggleView = false;

	string DeviceName = string.Empty;

	public static bool isGrabbing = false;

	// Use this for initialization
	void Awake () 
	{
		LeapCamera.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "leapmotion";
		}

		if(MainGuiControls.LeapMotionMenu)
		{
			LeapCamera.enabled = true;
		}
		else{
			LeapCamera.enabled = false;
		}

//		//Send Left handdata via UDP
		handGO = GameObject.FindWithTag ("Leaphand");
		if (handGO != null)
		{
		Transform[] allChildrenL = handGO.GetComponentsInChildren<Transform>();
			if (allChildrenL != null)
			foreach (Transform child in allChildrenL) {
				// do whatever with child transform here
				Debug.Log("L: "+child.name);

				if(child.name == "palm")
				{
				//		Debug.Log("L: "+child);
					//gazedisplay
					if(!DevicesLists.availableDev.Contains("LEAPMOTION:TRACKING:PALM:POSITION"))
					{
							DevicesLists.availableDev.Add("LEAPMOTION:TRACKING:PALM:POSITION");		
					}
					if(DevicesLists.selectedDev.Contains("LEAPMOTION:TRACKING:PALM:POSITION") && UDPData.flag==true)
					{					
						UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]palm,position,"+child.transform.position.x.ToString()+","+child.transform.position.y.ToString()+","+child.transform.position.z.ToString()+";"); 
					}

				}


//
//				//gazedisplay
//				if(!DevicesLists.availableDev.Contains("LEAPMOTION:TRACKING:FINGERS:ALL"))
//				{
//					DevicesLists.availableDev.Add("LEAPMOTION:TRACKING:FINGERS:ALL");		
//				}
//				if(DevicesLists.selectedDev.Contains("LEAPMOTION:TRACKING:FINGERS:ALL") && UDPData.flag==true)
//				{
//					if(child.name == "ring")
//					{
//						if(child.name.Contains("bone1"))
//							{
//							UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]"+"index1"+",position,"+child.transform.position.x.ToString()+","+child.transform.position.y.ToString()+","+child.transform.position.z.ToString()+";"
//							                   +"[$]tracking,[$$]"+DeviceName+",[$$$]"+"index1"+",rotation,"+child.transform.rotation.eulerAngles.x.ToString()+","+child.transform.rotation.eulerAngles.y.ToString()+","+child.transform.rotation.eulerAngles.z.ToString()+";"); 
//							}
//						if(child.name.Contains("bone2"))
//							{
//							UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]"+"index2"+",position,"+child.transform.position.x.ToString()+","+child.transform.position.y.ToString()+","+child.transform.position.z.ToString()+";"
//							                   +"[$]tracking,[$$]"+DeviceName+",[$$$]"+"index2"+",rotation,"+child.transform.rotation.eulerAngles.x.ToString()+","+child.transform.rotation.eulerAngles.y.ToString()+","+child.transform.rotation.eulerAngles.z.ToString()+";"); 
//							}
//						if(child.name.Contains("bone3"))
//							{
//							UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]"+"index3"+",position,"+child.transform.position.x.ToString()+","+child.transform.position.y.ToString()+","+child.transform.position.z.ToString()+";"
//							                   +"[$]tracking,[$$]"+DeviceName+",[$$$]"+"index3"+",rotation,"+child.transform.rotation.eulerAngles.x.ToString()+","+child.transform.rotation.eulerAngles.y.ToString()+","+child.transform.rotation.eulerAngles.z.ToString()+";"); 
//							}
//					}
//				}
//				



			}

			if(!DevicesLists.availableDev.Contains("LEAPMOTION:BUTTON:GRAB:BOOL"))
			{
				DevicesLists.availableDev.Add("LEAPMOTION:BUTTON:GRAB:BOOL");		
			}
			if(DevicesLists.selectedDev.Contains("LEAPMOTION:BUTTON:GRAB:BOOL") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]grab,bool,"+isGrabbing.ToString()+";"); 
			}
		}

//		//Send Right handdata via UDP
//		handGO2 = GameObject.FindWithTag ("LeaphandR");
//		if (handGO2 != null)
//		{
//			Transform[] allChildrenR = handGO2.GetComponentsInChildren<Transform>();
//			if (allChildrenR != null)
//			foreach (Transform child in allChildrenR) {
//				// do whatever with child transform here
//				Debug.Log("R: "+child);
//			}
//		}

	}


	void OnGUI()
	{
		if(MainGuiControls.LeapMotionMenu)
		{
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.LeapMotionMenu = false;
				MainGuiControls.hideMenus = false;
			}

		//	toggleView = GUI.Toggle(new Rect(20, 30, 100, 30), toggleView, "Reverse");

		}

	}

}
