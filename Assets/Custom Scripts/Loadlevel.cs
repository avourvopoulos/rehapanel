using UnityEngine;
using System.Collections;

public class Loadlevel : MonoBehaviour {
	
	public GameObject Args;
	public GUIStyle loadstyle;

	// Use this for initialization
	void Awake()  
	{
		Screen.SetResolution (1024, 720, false);
		
		DontDestroyOnLoad(Args);	
		
		StartCoroutine(loadcp());

	}

	
	IEnumerator loadcp() {
		print("Loading...");
        yield return new WaitForSeconds(2);
		Application.LoadLevel(1);
    }
	
	
	 void OnGUI() 
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) 
		{
            print("Quitting");
            Application.Quit();
		}
			
        GUI.Label(new Rect((Screen.width/2)-50, (Screen.height/2)+50, 100, 20), "Loading...",loadstyle);


		GUI.Label(new Rect((Screen.width)-120, (Screen.height)-30, 300, 20), "Version: " + "2017.01");
    }

	
}
