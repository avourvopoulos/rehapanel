using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class About : MonoBehaviour {
	
	public static bool activeWin = false;
	
	string aboutText = string.Empty;
		
	Rect windowRect = new Rect(300, 150, 500, 430);//posx, posy, width, height
	
	private Vector2 scrollViewVector = Vector2.zero;
	
	public Texture logos;
	public GUIStyle style;
	
	void Awake()
	{
		//load text file
		//path = Application.dataPath + "/StreamingAssets";
		using (TextReader reader = File.OpenText(Application.dataPath + "/StreamingAssets" + "/About.txt"))
		{
		    aboutText = reader.ReadToEnd();
			Debug.Log("About.txt loaded");
		}
	}
	
    void OnGUI()
	{
    	if (activeWin)
		{
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "About");
			
		}
    }
		
    void DoMyWindow(int windowID)
	{
		//close button
        if (GUI.Button(new Rect((windowRect.width/2)-50, windowRect.height-30, 100, 20), "Close"))
		{
			activeWin = false;
            print("closing about...");
		}
		
		//author
		GUI.Label (new Rect (30, windowRect.height-145, 450, 80), "Development: Athanasios Vourvopoulos");
		GUI.Label (new Rect (30, windowRect.height-125, 450, 80), "email: athanasios.vourvopoulos@m-iti.org");
		
		//logo
		if (GUI.Button(new Rect(25, windowRect.height-100, 450, 80), logos,style))
            Application.OpenURL("http://neurorehabilitation.m-iti.org/");
		
		//drag window
         GUI.DragWindow(new Rect(0, 0, 10000, 20));
		
		// Begin the ScrollView
		scrollViewVector = GUI.BeginScrollView (new Rect (30, 30, 450, 250), scrollViewVector, new Rect (0, 0, 420, 700));
		// Put something inside the ScrollView
		aboutText = GUI.TextArea (new Rect (0, 0, 420, 700), aboutText);
		// End the ScrollView
		GUI.EndScrollView();
    }
	

}
