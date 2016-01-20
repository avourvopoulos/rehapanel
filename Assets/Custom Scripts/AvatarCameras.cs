using UnityEngine;
using System.Collections;

public class AvatarCameras : MonoBehaviour {
	
	public Camera Cam1;
	public Camera Cam2;

	// Use this for initialization
	void Start () 
	{
		Cam1.enabled = true;
		Cam2.enabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
