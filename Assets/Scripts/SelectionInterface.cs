using UnityEngine;
using System.Collections;

public class SelectionInterface : MonoBehaviour {
	
	public EyeTracking eyeTracker = null;
	public Calibration calibrate = null;
	public Rect mouseButton;
	public Rect eyeTrackerButton;
	public Rect calibrateButton;
	public Rect startGameButton;
	private bool showLoading = false;
	private bool showMouseButton = true;
	private bool showEyeTrackerButton = false;
	private bool showCalibrate = false;
	private bool showStartButton = false;

	// Use this for initialization
	void Start () {
		mouseButton.x += Screen.width/2.0f;
		mouseButton.y += Screen.height/2.0f;
		eyeTrackerButton.x += Screen.width/2.0f;
		eyeTrackerButton.y += Screen.height/2.0f;
		calibrateButton.x += Screen.width/2.0f;
		calibrateButton.y += Screen.height/2.0f;
		startGameButton.x += Screen.width/2.0f;
		startGameButton.y += Screen.height/2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(eyeTracker==null){
			showMouseButton = true;
		}else{
			//showMouseButton = !eyeTracker.IsConnected;
			//showEyeTrackerButton = eyeTracker.FoundTracker;
			if(calibrate!=null){
				showMouseButton &= !calibrate.IsCalibrating;
				showEyeTrackerButton &= !calibrate.IsCalibrating;
				showCalibrate = !calibrate.IsCalibrating && eyeTracker.IsConnected;
				showStartButton = !calibrate.IsCalibrating && eyeTracker.IsConnected;
			}
		}
	}
	
	void OnGUI(){
	/*	if(showMouseButton){
			if(GUI.Button(mouseButton,"Use mouse")){
				StartCoroutine(loadNextScene());
			}
		}
		if(showEyeTrackerButton){
			if(GUI.Button(eyeTrackerButton,"Free your hand")){
				if(eyeTracker.FoundTracker){
					eyeTracker.ConnectToEyeTracker();
				}
			}
		}
		if(showCalibrate){
			if(GUI.Button(calibrateButton,"Click to Calibrate")){
				if(!calibrate.IsCalibrating){
					calibrate.StartCalibrate();
				}
			}
		}
		if(showStartButton){
			if(GUI.Button(startGameButton,"Click to Start")){
				if(!calibrate.IsCalibrating){
					StartCoroutine(loadNextScene());
				}
			}
		}*/
	}
	
	IEnumerator loadNextScene(){
		showLoading = true;
		AsyncOperation async = Application.LoadLevelAsync(Application.loadedLevel+1);
		yield return async;
	}
}
