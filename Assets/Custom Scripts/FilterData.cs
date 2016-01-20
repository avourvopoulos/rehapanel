using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FilterData : MonoBehaviour {
	
	List<string> jointlst = new List<string>();
	
	public float ALPHA = 0.15f;
	public float[] n_input = new float[3];//xy in/out
	public float[] n_output = new float[3];//xy in/out
	
	public float[] r_input = new float[4];//rot xyz in/out
	public float[] r_output = new float[4];//rotxyz in/out
	
	Vector3 FilteredPosition;
	Vector3 FilteredRotation;
	
	float delta;
	bool deltaon = false;
	bool filteronoff = false;
	
	
	//low pass filter x,y
	float[] lowPass( float[] input, float[] output ) 
	{
     for(int i=0; i<input.Length; i++ ) 
		{
			 if( output == null || output.Length == 0 || float.IsNaN(output[0]) || float.IsNaN(output[1]) || float.IsInfinity(output[0]) || float.IsInfinity(output[1]))
			 {
			//	return input;
				output[i] = input[i];
			 }
			 else
			{
				output[i] = output[i] + ALPHA * (input[i] - output[i]) * delta;
			//	output[i] = (1-ALPHA)* input[i] + (ALPHA*output[i]); //ToDo: include a deltatime relative to ALPHA
				
			}
    	}
    		return output;
	}


	// Use this for initialization
	void Awake() 
	{
		 jointlst.Add("Body");
		  jointlst.Add("Head");
		  jointlst.Add("Neck");
	      jointlst.Add("Torso");
	      jointlst.Add("Waist");			
	      jointlst.Add("LeftShoulder");
	      jointlst.Add("LeftElbow");
	      jointlst.Add("LeftWrist");
	      jointlst.Add("RightShoulder");
	      jointlst.Add("RightElbow");
	      jointlst.Add("RightWrist");
	      jointlst.Add("LeftHip");
	      jointlst.Add("LeftKnee");
	      jointlst.Add("LeftAnkle");
	      jointlst.Add("RightHip");
	      jointlst.Add("RightKnee");
	      jointlst.Add("RightAnkle");
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if(filteronoff)
		{
			foreach (string joint in jointlst)
			{
				
				//put last x,y coordinates to an array in order to feed the tracking to the low pass filter
				n_input[0]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.position.x;//x
				n_input[1]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.position.y;//y
				n_input[2]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.position.z;//z
				
				n_output[0] = lowPass(n_input, n_output)[0];
				n_output[1] = lowPass(n_input, n_output)[1];
				n_output[2] = lowPass(n_input, n_output)[2];
				
		//		lowPass(n_input, n_output); //[0]for x, [1]for y
				FilteredPosition = new Vector3(lowPass(n_input, n_output)[0], lowPass(n_input, n_output)[1], lowPass(n_input, n_output)[2]);
				
				//position
				GameObject.FindGameObjectWithTag("Ghost"+joint).transform.position = FilteredPosition;
				
	
				//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				
				r_input[0]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.rotation.eulerAngles.x;//x
				r_input[1]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.rotation.eulerAngles.y;//y
				r_input[2]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.rotation.eulerAngles.z;//z
		//		r_input[3]=GameObject.FindGameObjectWithTag("Ghost"+joint).transform.rotation.w;//w
				
				r_output[0] = lowPass(r_input, r_output)[0];
				r_output[1] = lowPass(r_input, r_output)[1];
				r_output[2] = lowPass(r_input, r_output)[2];
		//		r_output[3] = lowPass(r_input, r_output)[3];
				
		//		lowPass(n_input, n_output); //[0]for x, [1]for y
				FilteredRotation = new Vector3(lowPass(r_input, r_output)[0], lowPass(r_input, r_output)[1], lowPass(r_input, r_output)[2]);//, lowPass(r_input, r_output)[3]);
				
				//rotation
				GameObject.FindGameObjectWithTag("Ghost"+joint).transform.rotation = Quaternion.Euler(FilteredRotation);
	
			}//foreach
		}//if on
	}
	
	
	void OnGUI () 
	{
		filteronoff = GUI.Toggle(new Rect((Screen.width/2)+100, (Screen.height/2)+20, 100, 30), filteronoff, "On/Off");
		
		ALPHA = GUI.HorizontalSlider(new Rect((Screen.width/2), (Screen.height/2), 100, 30), ALPHA, 0.0F, 1.0F);
		GUI.Label(new Rect((Screen.width/2)+120, (Screen.height/2), 200, 100), ": "+ALPHA.ToString("0.00"));
		deltaon = GUI.Toggle(new Rect((Screen.width/2), (Screen.height/2)+20, 100, 30), deltaon, "Delta");
	}
	

}
