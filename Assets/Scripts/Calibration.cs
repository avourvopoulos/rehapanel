using UnityEngine;
using System.Collections;
using Tobii.Eyetracking.Sdk;
using Tobii.Eyetracking.Sdk.Exceptions;

public class Calibration : MonoBehaviour
{
	
	public int LevelToLoad;
	private bool Calibrationdone = false;
	private EyeTracking eyeTracking = null;
	private MetricTest metricTest = null;
	private GameObject smallObject;
	private SimpleAnimation simpleAnimation;
	private ArrayList frames = new ArrayList();
	private int index = 0;
	private bool isCalibrating = false;
	public Vector3[] points;
	private bool shouldPlayNext = false;
	public ArrayList Frames{
		get{return frames;}
		set{frames = value;}
	}
	
	public bool IsCalibrating{
		get{return isCalibrating;}
	}
	
	public bool IsCalibrationdone{
		get{return Calibrationdone;}
	}

	void OnGUI() {
		
	if(MainGuiControls.TobiiMenu)
		{
		
		if(eyeTracking.IsConnected && !isCalibrating){
//			if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height-90, 180, 20), "Start Calibration"))
//				{
//				startCalibrate();
//				Screen.fullScreen = !Screen.fullScreen;
//				}
		}
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		metricTest = this.GetComponent<MetricTest>();

		Calibrationdone=false;
		eyeTracking = this.GetComponent<EyeTracking>();
		if(eyeTracking!=null){
			eyeTracking.CalibratePointReceivedDelegate += new System.EventHandler(onPointReceived);
			eyeTracking.CalibrationDone += new System.EventHandler(onCalibrateDone);
			eyeTracking.TrackerConnected += new System.EventHandler(onConnected);
			eyeTracking.PlayerLeftGameDelegate += new System.EventHandler(onPlayerLeftGame);
		}

		simpleAnimation = GetComponent<SimpleAnimation>();
		Transform so = transform.FindChild("ScreenObject");
		if(so!=null){
			smallObject = so.gameObject;
			smallObject.active = false;
			setupFrames();
			simpleAnimation = smallObject.GetComponent<SimpleAnimation> ();
			simpleAnimation.finished += animation_finished;
		}
	}

	void animation_finished (object sender, System.EventArgs e)
	{
		if(frames.Count>index){
			Debug.Log("index:"+index);
			if(frames[index] == null){
				Debug.Log ("Feed next point");
				//add the correct point
				Vector3 point = points[index/4];
				eyeTracking.AddCalibrationPoint(new Point2D(point.x,1.0-point.y));
			}
			playNextFrame();
		}
	}

	void onPlayerLeftGame (object sender, System.EventArgs e)
	{
	}

	void onConnected (object sender, System.EventArgs e)
	{
		//Connected
		Debug.Log("from calibration: tracker connected");
	}

	void onCalibrateDone (object sender, System.EventArgs e)
	{
		Tobii.Eyetracking.Sdk.Calibration cali = (sender as IEyetracker).GetCalibration();
		int badpoints = 0;
		foreach(CalibrationPlotItem p in cali.Plot){
			if(p.ValidityLeft<0){
				badpoints++;
			}
			if(p.ValidityRight<0){
				badpoints++;
			}
		}
		if(badpoints<3){
			Debug.Log("Calibration Done");
			isCalibrating = false;
			shouldPlayNext = false;
			eyeTracking.showUI = true;
			metricTest.showUI = true;
			
			Screen.fullScreen = !Screen.fullScreen;//exit full screen
			//	LoadLevel(LevelToLoad);

		}else{
			index=0;
			playNextFrame();
		}
		
		Calibrationdone=true;
	}

	void LoadLevel(int level){
		Application.LoadLevel(level);
	}

	void onPointReceived (object sender, System.EventArgs e)
	{
		Debug.Log ("play next point");
		this.shouldPlayNext = true;
	}
	
	void playNextFrame(){
		simpleAnimation.PlayWithFrame(frames[index] as SimpleAnimationFrame);
		index++;
		this.shouldPlayNext = false;
		if(index>=frames.Count){
			Debug.Log ("Stop Calibration");
			if(eyeTracking!=null){
				eyeTracking.StopCalibrateEyeTracker();
				eyeTracking.SetOnScreenDisplay(true);
			}
		}
	}
	
	void OnDestroy(){
		if(eyeTracking!=null){
			eyeTracking.CalibratePointReceivedDelegate -= new System.EventHandler(onPointReceived);
			eyeTracking.CalibrationDone -= new System.EventHandler(onCalibrateDone);
			eyeTracking.PlayerLeftGameDelegate -= new System.EventHandler(onPlayerLeftGame);
			eyeTracking.TrackerConnected -= new System.EventHandler(onConnected);
		}
	}
	
	void setupFrames(){
		Vector3 position;
		for(int i=0;i<points.Length;i++){
			position = Camera.main.ViewportToWorldPoint(points[i]);
			position.z = 0.0f;
			frames.Add(new SimpleAnimationFrame(position,SimpleAnimationFrame.EmptyVector,SimpleAnimationFrame.EmptyQuaternion,1.0f));
			frames.Add(new SimpleAnimationFrame(SimpleAnimationFrame.EmptyVector,new Vector3(1.0f,1.0f,1.0f),SimpleAnimationFrame.EmptyQuaternion,1.0f));
			frames.Add(null);
			frames.Add(new SimpleAnimationFrame(SimpleAnimationFrame.EmptyVector,new Vector3(2.0f,2.0f,2.0f),SimpleAnimationFrame.EmptyQuaternion,1.0f));
		}
	}
	
	void generateFrame ()
	{
		SimpleAnimationFrame f = new SimpleAnimationFrame();
		f.position = Camera.main.ViewportToWorldPoint (new Vector3 (Random.value, Random.value, 0.0f));
		f.position.z = 0.0f;
		f.scale.x = Random.value * 10.0f + 2.0f;
		f.scale.y = f.scale.x;
		f.scale.z = f.scale.x;
		f.duration = Random.value * 5.0f + 1.0f;
		if (Random.value > 0.5){
			f.rotate = SimpleAnimationFrame.EmptyQuaternion;
		}
		simpleAnimation.currentFrame = f;
	}
	
	public void StartCalibrate(){
		
		startCalibrate();
	}
	
	void startCalibrate ()
	{
		if(smallObject!=null){
			smallObject.active=true;
		}
		if(eyeTracking==null){
			eyeTracking=EyeTrackerInput.TrackingScript;
		}
		if (eyeTracking != null) {
			if (eyeTracking.IsConnected) {
				eyeTracking.SetOnScreenDisplay(false);
				eyeTracking.StartCalibrateEyeTracker();
				index = 0;
				if(frames.Count>index){
					playNextFrame();
				}
				isCalibrating = true;
				
				eyeTracking.showUI = false;
				metricTest.showUI = false;
			}
		} else {
			Debug.Log ("Eyetracker Setting error");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(shouldPlayNext){
			playNextFrame();
		}
		
		if(!isCalibrating){
			if(smallObject!=null){
				smallObject.active=false;
			}
		}

		/*if(Input.GetKeyDown(KeyCode.S)){
			if(eyeTracking!=null&&!eyeTracking.IsConnected)
					eyeTracking.ConnectToEyeTracker();
		}

		if(Input.GetKeyDown(KeyCode.D)){
			if(eyeTracking.IsConnected){
				eyeTracking.DisconnectTracker();
			}
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			startCalibrate();
		}

		if (Input.GetKeyDown (KeyCode.L) ){
			LoadLevel(LevelToLoad);
		}*/
	}
}
