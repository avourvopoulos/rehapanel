using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class TestThreads : MonoBehaviour {
	
	static bool runthread=false;
	static bool threadstatus=false;
	
	// Use this for initialization
	void Start () 
	{	                               
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void WriteY()
  	{
	//	runthread=threadstatus;
		runthread=threadstatus;
		while(runthread)
		{
			print("y");
			string test = GameObject.FindGameObjectWithTag("GhostHead").transform.position.x.ToString(); print(test);
	//		runthread=threadstatus;
			runthread=threadstatus;
		}
			
  	//  for (int i = 0; i < 1000; i++) print("y");
  	}
	
	void OnGUI() 
	{
		if (GUI.Button (new Rect (Screen.width/2,Screen.height/2-20,60,30), "Start"))
		{
			Thread t = new Thread (WriteY);
			threadstatus=true;
			t.Start();// running WriteY() 
			Debug.Log("Thread Started");
		}
		if (GUI.Button (new Rect (Screen.width/2,Screen.height/2+20,60,30), "Stop"))
		{
			threadstatus=false;
			Debug.Log("Thread Stoped"); 
		}
		
    }//onGUI
	
}
