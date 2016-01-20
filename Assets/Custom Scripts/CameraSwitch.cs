using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {
	
	public Camera Cam1;//M1
	public Camera Cam2;//Camera Up
	public Camera Cam3;//DanaHead camera
	public Camera Cam4;//M2
	public Camera Cam5;//Carl Head Camera
	
	public static bool avatarView = false;
	
	public static string gender = "male";
	
	public Texture2D maleIcon;
	public Texture2D femaleIcon;
	
	// Use this for initialization
	void Awake () 
	{
		
		Cam1.enabled = true;
		Cam4.enabled = true;
		Cam2.enabled = false;
		Cam3.enabled = false;
		Cam5.enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Cam3.enabled == true || Cam2.enabled == true || Cam5.enabled == true)
		{
			Cam1.enabled = false;
			Cam4.enabled = false;
		}
		else
		{
			Cam1.enabled = true;
			Cam4.enabled = true;
		}
		
		if(avatarView == false)
		{
			Cam1.enabled=false;
			Cam4.enabled = false;
			Cam2.enabled=false;
			Cam3.enabled = false;
			Cam5.enabled = false;
		}
	}
	
	void OnGUI () 
		{	
		

if(MainGuiControls.KinectMenu)
{			
		
		//Cameras GUI
			GUI.BeginGroup (new Rect (Screen.width - 212+KinectGUI.gone, (Screen.height / 2 + 100), 200, 80));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,80), "Avatar View");
			GUI.color = Color.white;
			
			if (GUI.Button (new Rect (10,40,40,25), "Front"))
			{
				Cam1.enabled=true;
				Cam4.enabled = true;
				Cam2.enabled=false;
				Cam3.enabled = false;
				
				avatarView = false;
	//		MainGuiControls.toggleMirror=true;
			//	ZigSkeleton.mirror=true;
				MainGuiControls.menuindex=1;
			}
			if (GUI.Button (new Rect (60,40,40,25), "Top"))
			{
				Cam1.enabled=false;
				Cam4.enabled = false;
				Cam2.enabled=true;
				Cam3.enabled = false;
				
				avatarView = true;
	//		MainGuiControls.toggleMirror=true;
			//	ZigSkeleton.mirror=true;
				MainGuiControls.menuindex=0;
			}
			if (GUI.Button (new Rect (110,40,90,25), "First Person"))
			{
				Cam1.enabled=false;
				Cam4.enabled = true;
				Cam2.enabled=false;
				Cam3.enabled = true;
				Cam5.enabled = true;
				avatarView = true;
	//		MainGuiControls.toggleMirror=false;
			//	ZigSkeleton.mirror=false;
				MainGuiControls.menuindex=0;
			}			
			GUI.EndGroup (); //
			
			
			//avatar gender
			GUI.BeginGroup (new Rect (Screen.width - 212+KinectGUI.gone, (Screen.height / 2 + 190), 200, 60));
			GUI.color = Color.yellow;
			GUI.Box (new Rect (0,0,200,60), "Avatar Gender");
			GUI.color = Color.white;
			
			if (GUI.Button (new Rect (60,25,30,30), maleIcon))
			{
				gender = "male";
			}
			if (GUI.Button (new Rect (110,25,30,30), femaleIcon))
			{
				gender = "female";
			}
			
			GUI.EndGroup (); //
		
}//if Arg
			
			
			
		}	
		
}
