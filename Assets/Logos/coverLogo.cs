using UnityEngine;
using System.Collections;

public class coverLogo : MonoBehaviour {
	
	public Texture umamiti;
	bool showlogo = false;

	// Use this for initialization	
	void Awake() 
	{
	  StartCoroutine("Delay");	
	}
	
	IEnumerator Delay()
	{
        yield return new WaitForSeconds(2);
		showlogo = true;
		Debug.Log("after 2 secs");
    }


    void OnGUI() 
	{
	 if(showlogo == true)
		{
       GUI.DrawTexture(new Rect(10, 660, 180, 100), umamiti, ScaleMode.StretchToFill, false);
		}
		
	}
	
	 
    
}
