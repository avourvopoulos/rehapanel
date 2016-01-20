using UnityEngine;
using System.Collections;

public class SampleSphere : MonoBehaviour {
	
	public Vector3 worldpoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	worldpoint = Camera.mainCamera.ScreenToWorldPoint(EyeTrackerInput.getScreenInput());
	transform.position = new Vector3 (worldpoint.x,worldpoint.y,0);
	}
}
