using UnityEngine;
using System.Collections;

public class DataMenu : MonoBehaviour {
	
	public Texture2D netIcon;
	public Texture2D logIcon;
	public Texture2D emuIcon;
	public Texture2D visIcon;
	public Texture2D bciapeIcon;
	public Texture2D redIcon;
	
	public GUIStyle dataStyle;

	// Use this for initialization
	void Start () {
	
	}
	
//	void FixedUpdate ()
//    {
//        Debug.Log("FixedUpdate time :" + Time.deltaTime);
//    }
//    
//    
//    void Update ()
//    {
//        Debug.Log("Update time :" + Time.deltaTime);
//    }
	
	void OnGUI()
    {
		if(MainGuiControls.DataMenu && !MainGuiControls.hideMenus)
		{
			GUI.Label(new Rect(90 , Screen.height/2-150, 120, 25), "Networking", dataStyle);	
			if (GUI.Button (new Rect (60 , Screen.height/2-100 ,160,160), netIcon))
			{
				MainGuiControls.NetMenu = true;
				MainGuiControls.hideMenus = true;
			}
			
	GUI.enabled = !CheckUpdates.releaseVersion;		
			
			GUI.Label(new Rect(Screen.width/2-150 , Screen.height/2-150, 120, 25), "Logging", dataStyle);
			if (GUI.Button (new Rect (Screen.width/2-200 , Screen.height/2-100 ,160,160), logIcon))
			{
				MainGuiControls.LogMenu = true;
				MainGuiControls.hideMenus = true;
			}
			GUI.Label(new Rect(Screen.width/2+80 , Screen.height/2-150, 120, 25), "Emulation", dataStyle);
			if (GUI.Button (new Rect (Screen.width/2+40, Screen.height/2-100 ,160,160), emuIcon))
			{
				MainGuiControls.EmuMenu = true;
				MainGuiControls.hideMenus = true;
			}


//			GUI.Label(new Rect(Screen.width-250 , Screen.height/2-150, 120, 25), "Adaptive Performance\n          Engine", dataStyle);
//			if (GUI.Button (new Rect (Screen.width-230, Screen.height/2-100 ,160,160), bciapeIcon))
//			{
//				MainGuiControls.BCIAPE = true;
//				MainGuiControls.hideMenus = true;
//			}

			
	GUI.enabled = true;	

	GUI.enabled = false;	
			GUI.Label(new Rect(Screen.width-210 , Screen.height/2-150, 120, 25), "Visualisation", dataStyle);
			if (GUI.Button (new Rect (Screen.width-230, Screen.height/2-100 ,160,160), visIcon))
			{
				//	MainGuiControls.VisMenu = true;
				//	MainGuiControls.hideMenus = true;
			}

	GUI.enabled = false;
			
			//green-red icons
			if(CheckUpdates.releaseVersion && !MainGuiControls.hideMenus)
			{
				GUI.Label(new Rect(Screen.width/2-200 , Screen.height/2-100, 20, 20), redIcon);//Logging
				GUI.Label(new Rect(Screen.width/2+50, Screen.height/2-100, 20, 20), redIcon);//Emulation
				GUI.Label(new Rect(Screen.width-230, Screen.height/2-100, 20, 20), redIcon);//Visualisation
			}
			
			
		}
	}
}
