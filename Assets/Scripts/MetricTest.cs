using UnityEngine;
using System.Collections;

public class MetricTest : MonoBehaviour {
	
	public Texture2D crosshairTexture;
	public Rect crosshairPosition;
	private EyeTracking eyeTracking = null;
	public bool showUI = true;
	
	public Texture2D objectTexture;
	public static float objectX, objectY;
	public float displayTime, respawnTime;
	private float timer = 0;

	//Random test variables
	private bool isRandom = false;
	private bool spawning = true;
	public static bool showObject = false;

	//Circular test variables
	private bool isCircular = false;
	private float xCenter, yCenter;
	public float radius = 100.0f;
	public float velocity = 1;

	//Sinusoid test variables
	private bool isSinusoid = false;
	public float amplitude = 250.0f;
	public float frequency = 0.003f;
	public bool bounce = true;
	private float phase;
	
	void OnGUI() {

		/*if (GUI.Button(new Rect(10, 10, 80, 20), "Connect"))
			eyeTracking.ConnectToEyeTracker();  

		if (GUI.Button(new Rect(10, 30, 80, 20), "Disconnect"))
			eyeTracking.DisconnectTracker();*/

		if(eyeTracking.IsConnected && showUI){
			if (GUI.Button(new Rect(10, 70, 180, 20), "Start/Stop Metric Test"))
	        {
				isRandom = !isRandom;
				spawning = !spawning;
				isSinusoid = false;
				isCircular = false;
			}
			
			if (GUI.Button(new Rect(10, 90, 180, 20), "Start/Stop Circular Motion"))
			{
				isCircular = !isCircular;
				isSinusoid = false;
				isRandom = false;
			}
			
			if (GUI.Button(new Rect(10, 110, 180, 20), "Start/Stop Sinusoid Motion"))
			{
				isSinusoid = !isSinusoid;
				isCircular = false;
				isRandom = false;
			}
			
			if(isRandom) GUI.DrawTexture(crosshairPosition, crosshairTexture);
			
			if(showObject || isCircular || isSinusoid) {
				GUI.DrawTexture(new Rect(objectX, objectY, objectTexture.width, objectTexture.height), objectTexture);
			}
		}
    }

	// Use this for initialization
	void Start () {
		eyeTracking = this.GetComponent<EyeTracking>();

		crosshairPosition = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) /2, crosshairTexture.width, crosshairTexture.height);

		xCenter = (Screen.width - objectTexture.width)/2;
		yCenter = (Screen.height - objectTexture.height)/2;
		
		phase = 2*Mathf.PI;
	}
	
	// Update is called once per frame
	void Update () {

		eyeTracking.SetOnScreenDisplay(!(isRandom || isCircular || isSinusoid));

        //Object Test
		if(isRandom){
			if(!spawning){
				timer += Time.deltaTime;
			}
			
			//when timer reaches y seconds, hide the object
			if(showObject && timer >= displayTime){
				showObject = false;
				//Debug.Log("Hidden!");
			}
				
			//when timer reaches x seconds, call Spawn function
			if(timer >= respawnTime){
				Spawn();
			}
		}
		else {
			showObject = false;
		}

		if(isCircular){
			timer += Time.deltaTime;
			
			//
			float angle = (timer * velocity) % (2*Mathf.PI);
			
			//circle: (x - a)^2 + (y - b)^2 = r^2; x = a + r * cos(t); y = b + r * sin(t)
			objectX = xCenter + radius * Mathf.Cos(angle);
			objectY = yCenter + radius * Mathf.Sin(angle);
		}
		
		if(isSinusoid) {
			//timer += Time.deltaTime;
			if(objectX > (Screen.width-objectTexture.width)){
				if(bounce)
					velocity = -velocity;
				else
					objectX = 0;
			}

			if(objectX < 0){
				if(bounce)
					velocity = -velocity;
				else
					objectX = Screen.width;
			}
			
			//sinusoidal: y(t) = A*sin(2*Pi*f*t + p) + D
			objectX += velocity;
			objectY = amplitude * Mathf.Sin(2*Mathf.PI * frequency * objectX + phase) + yCenter;
		}
	}
	
	//Spawn test object
	void Spawn(){
		//set spawning to true, to stop timer counting in the Update function
		spawning = true;
		//reset the timer to 0 so process can start over
		timer = 0;
		
		//select a random number, inside a maths function absolute command to ensure it is a whole number
		objectX = Mathf.Abs(Random.Range(100, Screen.width - objectTexture.width)); 
		objectY = Mathf.Abs(Random.Range(100, Screen.height - objectTexture.height));
		
		//set spawning back to false so timer may start again
		showObject = true;
		spawning = false;
		
		//Debug.Log("Spawned: " + System.DateTime.Now.ToString());
	}
}
