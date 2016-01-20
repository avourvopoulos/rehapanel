using UnityEngine;
using System.Collections;

public class HandlePrefs : MonoBehaviour {

	public static bool minWindow  = false;
	public static string bciMode = string.Empty;

	// Use this for initialization
	void Awake () {

		bciMode = PlayerPrefs.GetString ("bci");

		if (PlayerPrefs.GetString ("window") == "min"){
			minWindow = true;
		}
		else{
			minWindow = false;
		}

		//print("PlayerPrefs: "+PlayerPrefs.GetString("bci"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//#if UNITY_EDITOR
	void OnGUI()
	{
//		GUI.Label(new Rect(10, 10, 100, 20), "Window: "+PlayerPrefs.GetString("window"));
//		GUI.Label(new Rect(10, 20, 100, 20), "BCI: "+PlayerPrefs.GetString("bci"));
	}
	//#endif	

	void OnApplicationQuit() {

		PlayerPrefs.DeleteAll ();

	}

}
