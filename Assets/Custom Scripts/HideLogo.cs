using UnityEngine;
using System.Collections;

public class HideLogo : MonoBehaviour {

	public Texture2D logo;
	public Texture2D logo2;
	public GUIStyle logostyle;
	
	//Rect LogoRect = new Rect(10, Screen.height-110, 160, 100);
	
    void OnGUI() 
	{
		Rect LogoRect = new Rect(10, Screen.height-110, 160, 100);
        LogoRect = GUI.Window(1, LogoRect, LogoWindow, " ");

//		Rect LogoRect = new Rect(10, Screen.height-110, 1000, 100);
//		LogoRect = GUI.Window(1, LogoRect, LogoWindow, " ");
    }
	
    void LogoWindow(int windowID) 
	{
       if (GUI.Button(new Rect(0, 0, 300, 300), logo, logostyle))
           Application.OpenURL("http://www.m-iti.org/");
 
//		if (GUI.Button (new Rect (0, 0, 1000, 1000), logo2, logostyle)) {
//			About.activeWin=true;
//				
    }
	
}
