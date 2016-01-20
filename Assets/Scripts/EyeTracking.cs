using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using Tobii.Eyetracking.Sdk;
using Tobii.Eyetracking.Sdk.Exceptions;
using Tobii.Eyetracking.Sdk.Time;

public class EyeTracking : MonoBehaviour {
	
	private bool useFirstDevice = true;
	private EyetrackerBrowser browser = null;
	private string okString = "ok";
	private IEyetracker theTracker = null;
	private SyncManager syncManager = null;
	private Clock localClock;
	private EyetrackerInfo device = null;
	private Vector3 leftGazePoint;
	private Vector3 rightGazePoint;
	private Vector3 leftEyeVector;
	private Vector3 rightEyeVector;
	private Vector3 leftEyePosition;
	private Vector3 rightEyePosition;
	private bool isConnected = false;
	private bool noEyeFound = false;
	private bool foundEyeTracker = false;
	private EventThreadingOptions opt = EventThreadingOptions.BackgroundThread;
	private Transform display;
	private Transform cursor_left;
	private Transform cursor_right;
	private Transform cursor_center;
	private DataRecorder recorder = null;
	//public EyesBlinkRecognizer blinkRecognizer;
	//private EyesMovementRecognizer movementRecognizer;
	
	public string productId = "TX300-010101144563";
	public string ipAddress = "169.254.96.168";
	public int serverPort = 4455;
	public int syncPort = 4457;
	public Int64 userLeftTimeOut = 20000;
	public event EventHandler CalibrationDone = null;
	public event EventHandler CalibratePointReceivedDelegate = null;
	public event EventHandler TrackerConnected = null;
	public event EventHandler PlayerLeftGameDelegate = null;
	public bool showOnScreenDisplay = true;
	public bool shouldRecord = true;
	public bool showUI = true;
	
	MainGuiControls maingui;
	
	public bool FoundTracker{
		get{return foundEyeTracker;}
	}
	
	public bool NoEyeFound{
		get{return noEyeFound;}
	}
	
	public bool IsConnected{
		get{return isConnected;}
	}
	
	public EyetrackerInfo ConnectedDevice{
		get{return device;}
	}
	
	public Vector3 CenterEyePosition{
		get{return Vector3.Lerp(leftGazePoint,rightGazePoint,0.5f);}
	}
	
	public Vector3 LeftEyePosition{
		get{return leftEyePosition;}
	}
	
	public Vector3 RightEyePosition{
		get{return rightEyePosition;}
	}
	
	public Vector3 RightEyeVector{
		get{return rightEyeVector;}
	}
	
	public Vector3 LeftEyeVector{
		get{return leftEyeVector;}
	}
	
	public Vector2 LeftGazePoint{
		get{return leftGazePoint;}
	}
	
	public Vector2 RightGazePoint{
		get{return rightGazePoint;}
	}
	
	public Vector2 CenterGazePoint{
		get{return Vector2.Lerp(leftGazePoint,rightGazePoint,0.5f);}
	}
	
	public bool Blinked{
		get{
			//return blinkRecognizer.GetEyeBlinked;
			return false;
		}
	}
	
	public void DeactivateChildren(){
		for(int i=0;i<gameObject.transform.childCount;i++){
			gameObject.transform.GetChild(i).gameObject.active=false;
		}
	}

	void OnGUI() {
		
	if(MainGuiControls.TobiiMenu)
	{	
		if(showUI){
			if(IsConnected){
				GUI.color = Color.red;	
				if (GUI.Button(new Rect(Screen.width/2 - 45, Screen.height-120, 80, 20), "Disconnect"))
					{
						DisconnectTracker();
					}
				GUI.color = Color.white;	
			}
			else{
				GUI.color = Color.green;	
				if (GUI.Button(new Rect(Screen.width/2 - 45, Screen.height-120, 80, 20), "Connect"))
					{
						ConnectToEyeTracker();
					
						KinectGUI.sendULData();
					}
				GUI.color = Color.white;
			}
		}
		
		}//if tobii menu

	}

	// Use this for initialization
	void Start () {
		//cursor_left = transform.FindChild("cursor_left");
		//cursor_right = transform.FindChild("cursor_right");
		cursor_center = transform.FindChild("cursor_center");
		display = transform.FindChild("display");
		recorder = new DataRecorder();
		//blinkRecognizer = new EyesBlinkRecognizer();
		//movementRecognizer = new EyesMovementRecognizer();
		try{
			Tobii.Eyetracking.Sdk.Library.Init();
		}catch(DllNotFoundException e){
			Debug.LogError(e.Message);
			return;
		}
		//if(Application.isEditor)opt = EventThreadingOptions.BackgroundThread;
		//opt = EventThreadingOptions.BackgroundThread;
		localClock = new Clock();
		browser = new EyetrackerBrowser(opt);
		browser.EyetrackerFound += new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerFound);
		browser.EyetrackerRemoved += new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerRemoved);
		browser.EyetrackerUpdated += new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerUpdated);
		if(!browser.IsStarted)browser.Start();
		//ConnectToEyeTracker();
		Debug.Log("Waiting for trackers");
	}
	
	void Awake(){
		DontDestroyOnLoad(this.gameObject);
		
		maingui = GetComponent<MainGuiControls>();
	}

	void eyeTrackerRemoved (object sender, EyetrackerInfoEventArgs e)
	{
		Debug.Log("remove:"+e.EyetrackerInfo.ProductId);
		if(isMatch(e.EyetrackerInfo)){
			device = e.EyetrackerInfo;
			isConnected = false;
			Debug.Log("Tracker removed");
		}
	}

	void eyeTrackerFound (object sender, EyetrackerInfoEventArgs e)
	{
		Debug.Log("add:"+e.EyetrackerInfo.ProductId);
		if(isMatch(e.EyetrackerInfo)){
			Debug.Log("preparing to connect to:"+e.EyetrackerInfo.ProductId);
			device = e.EyetrackerInfo;
			Debug.Log("eyetracker state:"+device.Status);
			if(device.Status != okString)return;
			foundEyeTracker = true;
//			try{
//				theTracker = EyetrackerFactory.CreateEyetracker(device,opt);
//				syncManager = new SyncManager(localClock,device,opt);
//				syncManager.SyncManagerError += new EventHandler(syncMangerError);
//				syncManager.SyncStateChanged += new EventHandler<SyncStateChangedEventArgs>(syncStateChanged);
//				theTracker.ConnectionError += new EventHandler<ConnectionErrorEventArgs>(trackerConnectionError);
//				theTracker.CalibrationStarted += new EventHandler<CalibrationStartedEventArgs>(calibrationStarted);
//				theTracker.CalibrationStopped += new EventHandler<CalibrationStoppedEventArgs>(calibrationStopped);
//				theTracker.GazeDataReceived += new EventHandler<GazeDataEventArgs>(internalGazeDataReceived);
//				theTracker.FramerateChanged += new EventHandler<FramerateChangedEventArgs>(frameRateChanged);
//				theTracker.SetFramerate(300.0f);
//				theTracker.StartTracking();
//				Debug.Log(theTracker.GetXConfiguration().LowerLeft+";"+theTracker.GetXConfiguration().UpperLeft);
//				isConnected = true;
//				//Close the browser?
//				//CloseBrowser();
//				if(TrackerConnected!=null){
//					TrackerConnected(this.theTracker,EventArgs.Empty);
//				}
//				Debug.Log("tracker connected");
//			}catch(EyetrackerException ex){
//				Debug.LogError(ex.Message);
//				DisconnectTracker();
//				CloseBrowser();
//				Application.Quit();
//			}catch(Exception es){
//				Debug.LogError(es.Message);
//				DisconnectTracker();
//				CloseBrowser();
//				Application.Quit();
//			}
		}
	}
	
	public void ConnectToEyeTracker(){
		try{
			Debug.Log("Connecting");
			theTracker = EyetrackerFactory.CreateEyetracker(device,opt);
			syncManager = new SyncManager(localClock,device,opt);
			syncManager.SyncManagerError += new EventHandler(syncMangerError);
			syncManager.SyncStateChanged += new EventHandler<SyncStateChangedEventArgs>(syncStateChanged);
			theTracker.ConnectionError += new EventHandler<ConnectionErrorEventArgs>(trackerConnectionError);
			theTracker.CalibrationStarted += new EventHandler<CalibrationStartedEventArgs>(calibrationStarted);
			theTracker.CalibrationStopped += new EventHandler<CalibrationStoppedEventArgs>(calibrationStopped);
			theTracker.GazeDataReceived += new EventHandler<GazeDataEventArgs>(internalGazeDataReceived);
			theTracker.FramerateChanged += new EventHandler<FramerateChangedEventArgs>(frameRateChanged);
			theTracker.SetFramerate(60.0f);
			theTracker.StartTracking();
			Debug.Log(theTracker.GetXConfiguration().LowerLeft+";"+theTracker.GetXConfiguration().UpperLeft);
			isConnected = true;
			if(TrackerConnected!=null){
				TrackerConnected(this.theTracker,EventArgs.Empty);
			}
			Debug.Log("tracker connected");
		}catch(EyetrackerException ex){
			Debug.LogError(ex.Message);
			DisconnectTracker();
			CloseBrowser();
		}catch(Exception es){
			Debug.LogError(es.Message);
			DisconnectTracker();
			CloseBrowser();
		}
	}

	void syncStateChanged (object sender, SyncStateChangedEventArgs e)
	{
		Debug.Log(e.SyncState.StateFlag);
	}

	void syncMangerError (object sender, EventArgs e)
	{
		Debug.LogError("error happened");
		DisconnectTracker();
	}
	
	void eyeTrackerUpdated (object sender, EyetrackerInfoEventArgs e)
	{
		Debug.Log("update:"+e.EyetrackerInfo.ProductId);
		if(isMatch(e.EyetrackerInfo)){
			device = e.EyetrackerInfo;
			Debug.Log("status:"+e.EyetrackerInfo.Status);
		}
	}

	void frameRateChanged (object sender, FramerateChangedEventArgs e)
	{
		Debug.Log("Frame rate change to"+e.Framerate);
	}
	
	public void StartCalibrateEyeTracker(){
		if(theTracker != null){
			theTracker.StartCalibration();
		}else{
			Debug.Log("No tracker is connected.");
		}
	}
	
	public void AddCalibrationPoint(Point2D p){
		if(theTracker != null){
			theTracker.AddCalibrationPointAsync(p,OnPointReceived);
			Debug.Log("point added");
		}
	}
	
	void OnPointReceived(object sender,AsyncCompletedEventArgs<Empty> e){
		//tell the client it's time to feed next point
		if(CalibratePointReceivedDelegate!=null){
			CalibratePointReceivedDelegate(this,EventArgs.Empty);
			//CalibratePointReceivedDelegate.Invoke(this,EventArgs.Empty);
		}
	}
	
	public void StopCalibrateEyeTracker(){
		if(theTracker != null){
			Debug.Log("before stop calibrating");
			theTracker.ComputeCalibrationAsync(OnCompleteCalculateCalibration);
			theTracker.StopCalibration();
			
			SetOnScreenDisplay(true);
		}
	}
	
	void OnCompleteCalculateCalibration(object sender, AsyncCompletedEventArgs<Empty> e){
		if(CalibrationDone!=null){
			Debug.Log("Finish Calculating");
			CalibrationDone(theTracker,EventArgs.Empty);
		}
	}

	void internalGazeDataReceived (object sender, GazeDataEventArgs e)
	{
		if(recorder!=null && shouldRecord){
			DataItem item = new DataItem(e.GazeDataItem, MetricTest.objectX, MetricTest.objectY, MetricTest.showObject);
			recorder.feed(item);
		}
		if(e.GazeDataItem.LeftValidity<2 && e.GazeDataItem.RightValidity<2){
			leftGazePoint = Point2DToVector2(e.GazeDataItem.LeftGazePoint2D);
			rightGazePoint = Point2DToVector2(e.GazeDataItem.RightGazePoint2D);
			leftEyeVector = Point3DToVector3(e.GazeDataItem.LeftEyePosition3DRelative);
			rightEyeVector = Point3DToVector3(e.GazeDataItem.RightEyePosition3DRelative);
			leftEyePosition = Point3DToVector3(e.GazeDataItem.LeftEyePosition3D);
			rightEyePosition = Point3DToVector3(e.GazeDataItem.RightEyePosition3D);
		}
	//	GazeData gd = new GazeData(e.GazeDataItem);
		//movementRecognizer.feed(gd);
		//blinkRecognizer.feed(gd);
		if(e.GazeDataItem.LeftValidity>=3 && e.GazeDataItem.RightValidity>=3){
			noEyeFound=true;
		}else{
			noEyeFound=false;
		}
	}
	
	void calibrationStopped (object sender, CalibrationStoppedEventArgs e)
	{
	}

	void calibrationStarted (object sender, CalibrationStartedEventArgs e)
	{
	}

	void trackerConnectionError (object sender, ConnectionErrorEventArgs e)
	{
		Debug.Log(e.ToString());
		DisconnectTracker();
	}
	
	private bool isMatch(EyetrackerInfo info){
		if(useFirstDevice)return true;
		if(productId==null)return false;
		if(productId=="")return false;
		string id = info.ProductId;
		if(productId==id){
			return true;
		}
		return false;
	}
	
	public void DisconnectTracker(){
		isConnected = false;
		//foundEyeTracker = false;
		if(theTracker!=null){
			Debug.Log("disconnect from the tracker");
			theTracker.StopTracking();
			theTracker.GazeDataReceived -= new EventHandler<GazeDataEventArgs>(internalGazeDataReceived);
			theTracker.FramerateChanged -= new EventHandler<FramerateChangedEventArgs>(frameRateChanged);
			theTracker.CalibrationStarted -= new EventHandler<CalibrationStartedEventArgs>(calibrationStarted);
			theTracker.CalibrationStopped -= new EventHandler<CalibrationStoppedEventArgs>(calibrationStopped);
			theTracker.ConnectionError -= new EventHandler<ConnectionErrorEventArgs>(trackerConnectionError);
			theTracker.Dispose();
			theTracker=null;
		}
		if(syncManager!=null){
			syncManager.SyncManagerError -= new EventHandler(syncMangerError);
			syncManager.SyncStateChanged -= new EventHandler<SyncStateChangedEventArgs>(syncStateChanged);
			syncManager.Dispose();
			syncManager=null;
		}
		CloseBrowser();
	}
	
	Vector3 Point3DToVector3(Point3D point){
		return new Vector3((float)point.X,(float)point.Y,(float)point.Z);	
	}
	
	Vector2 Point2DToVector2(Point2D point){
		return new Vector2((float)point.X,(float)(1.0-point.Y));
	}

	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyDown(KeyCode.T)){
//			showOnScreenDisplay=!showOnScreenDisplay;
//			SetOnScreenDisplay(showOnScreenDisplay);
//		}
//		if(Input.GetKeyDown(KeyCode.R)){
//			shouldRecord=!shouldRecord;
//		}
		if(showOnScreenDisplay){
			if(theTracker!=null){
				if(this.isConnected && MainGuiControls.TobiiMenu){
					//cursor_left.position = this.leftGazePoint;
					//cursor_right.position = this.rightGazePoint;
					cursor_center.position = this.CenterGazePoint;
					display.guiText.text = leftGazePoint+";"+rightGazePoint+";"+CenterEyePosition.z + (shouldRecord?";R":"");
				}else{
					display.guiText.text = "not connected";
					//cursor_left.position=Vector3.zero;
					//cursor_right.position=Vector3.zero;
					cursor_center.position = Vector3.zero;
				}
			}
		}
	}
	
	public void SetOnScreenDisplay(bool v){
		showOnScreenDisplay=v;
		//cursor_left.gameObject.active = v;
		//cursor_right.gameObject.active = v;
		cursor_center.gameObject.SetActive(v);
		display.gameObject.active = v;
	}
	
	void CloseBrowser(){
		Debug.Log("Close tracker");
		isConnected = false;
		if(browser==null)return;
		browser.EyetrackerUpdated -= new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerUpdated);
		browser.EyetrackerFound -= new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerFound);
		browser.EyetrackerRemoved -= new EventHandler<EyetrackerInfoEventArgs>(eyeTrackerRemoved);
		while(browser.IsStarted){
			Debug.Log("wait");
			browser.Stop();
		}
		Debug.Log ("Browser closed");
		browser=null;
	}
	
	void OnDestroy(){
		Debug.Log("Quit");
		DisconnectTracker();
		CloseBrowser();
		recorder.WriteData();
		if(!Application.isEditor){
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}
