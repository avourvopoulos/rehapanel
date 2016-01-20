using UnityEngine;
using System.Collections;

public class LinearMovement : MonoBehaviour {
	
	public Texture2D crosshairTexture;
	public Rect crosshairPosition;	
	
	public Texture2D objectTexture;
	public float displayTime, respawnTime;	
	private float timer = 0;
	public static float objectX, objectY;
	
    private bool isCircular = false;
	private float xCenter, yCenter;
	public float radius = 100.0f;
	public float velocity = 1;
    
    private bool isSinusoid = false;
	public float amplitude = 250.0f;
	public float frequency = 0.003f;
	private float phase;
	
	
	
	void OnGUI() {				
		if (GUI.Button(new Rect(10, 50, 180, 20), "Start/Stop Circular Motion"))
        {
			isCircular = !isCircular;
			isSinusoid = false;
        }
		
		if (GUI.Button(new Rect(10, 70, 180, 20), "Start/Stop Sinusoid Motion"))
        {
			isSinusoid = !isSinusoid;
			isCircular = false;
        }
		
		//GUI.DrawTexture(crosshairPosition, crosshairTexture);
		
		if(isCircular || isSinusoid) GUI.DrawTexture(new Rect(objectX, objectY, objectTexture.width, objectTexture.height), objectTexture);
    }

	// Use this for initialization
	void Start () {
		xCenter = (Screen.width - objectTexture.width)/2;
		yCenter = (Screen.height - objectTexture.height)/2;
		
		phase = 2*Mathf.PI;
		
		//crosshairPosition = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) /2, crosshairTexture.width, crosshairTexture.height);
	}
	
	// Update is called once per frame
	void Update () {
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
			if(objectX >= Screen.width) objectX = 0;
			
			//sinusoidal: y(t) = A*sin(2*Pi*f*t + p) + D
			objectX += velocity;
			objectY = amplitude * Mathf.Sin(2*Mathf.PI * frequency * objectX + phase) + yCenter;
		}
	}
}
