using UnityEngine;
using System.Collections;

public class EyeTrackerInput : MonoBehaviour {
	
	static public EyeTracking TrackingScript = null;
	static public bool firstRun = true;
	private GameObject trackerObject;

	// Use this for initialization
	void Start () {
		trackerObject = GameObject.Find("EyeTrackingObject");
		if(trackerObject!=null){
			TrackingScript = trackerObject.GetComponent<EyeTracking>();
			TrackingScript.DeactivateChildren();
		}
	}
	
	static public bool IsEyesOpen(){
		if(TrackingScript==null || !TrackingScript.IsConnected){
			return !Input.GetMouseButton(0);
		}else{
			return !TrackingScript.NoEyeFound;
		}
	}
	
	/*static public bool IsEyesBlinked(){
		if(TrackingScript==null || !TrackingScript.IsConnected){
			return Input.GetMouseButtonUp(0);
		}else{
			return TrackingScript.Blinked;
		}
	}*/
	
	static public Vector3 getViewportInput(){
		if(TrackingScript==null || !TrackingScript.IsConnected){
			return Camera.mainCamera.ScreenToViewportPoint(Input.mousePosition);
		}else{
			Vector2 pos = TrackingScript.CenterGazePoint;
			return new Vector3(pos.x,pos.y,0.0f);
		}
	}
	
	static public Vector3 getScreenInput(){
		if(TrackingScript==null || !TrackingScript.IsConnected){
			return Input.mousePosition;
		}else{
			Vector2 pos = TrackingScript.CenterGazePoint;
			return Camera.mainCamera.ViewportToScreenPoint(new Vector3(pos.x,pos.y,0.0f));
		}
	}

	static public Vector3 getEyePos(){
		if(TrackingScript==null || !TrackingScript.IsConnected){
			return new Vector3(0.0f,0.0f,600.0f);
		}else{
			return TrackingScript.CenterEyePosition;
		}
	}
	
	static public bool IsOutOfRange(){
		if(TrackingScript == null || !TrackingScript.IsConnected){
			return false;
		}else{
			Vector3 eye_pos = TrackingScript.CenterEyePosition;
			return eye_pos.z>700.0f || eye_pos.z<540.0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
