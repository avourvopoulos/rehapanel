using UnityEngine;
using System.Collections;

public class KeyShortcuts : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
    	{
        // CTRL + Q
		
    	}
		
	}
	
}
