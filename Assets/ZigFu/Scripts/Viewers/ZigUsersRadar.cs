using UnityEngine;
using System.Collections;

public class ZigUsersRadar : MonoBehaviour {
	public Vector2 RadarRealWorldDimensions = new Vector2(6000, 6000);
	public int PixelsPerMeter = 35;
    public Color boxColor = Color.white;
    GUIStyle style;
    Texture2D texture;
	
	public static Vector2 radarPosition;
	
	void Start()
	{
        style = new GUIStyle();
        texture = new Texture2D(1, 1);
        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {

                Color color = Color.white;
                texture.SetPixel(x, y, color);
            }                      
        }
        texture.Apply();
        style.normal.background = texture;
		
	
		
	}


	void Update () 
	{
//		foreach (ZigTrackedUser currentUser in ZigInput.Instance.TrackedUsers.Values) {
//						Debug.Log ("Tracked: " + currentUser.SkeletonTracked);
//				}
	}

	
	void OnGUI () 
	{
		if(MainGuiControls.KinectMenu)
		{		
			if (!ZigInput.Instance.ReaderInited) return; 
			
			int width = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.x / 1000.0f));
			int height = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.y / 1000.0f));
			
			//for seated and near mode
			int nrwidth = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.x / 500.0f));
			int nrheight = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.y / 500.0f));
	        
			GUI.BeginGroup (new Rect (Screen.width - width - 565, ((Screen.height/2) + 95+KinectGUI.resize+MainGuiControls.hideviewers)*MainGuiControls.hideMenu, width, height)); // move position
	        Color oldColor = GUI.color;
	        GUI.color = boxColor;
	//		GUI.Box(new Rect(0, 0, width, height), "Users Radar", style);
			GUI.Box(new Rect(0, 0, width, height), " ", style);
	        GUI.color = oldColor;
			foreach (ZigTrackedUser currentUser in ZigInput.Instance.TrackedUsers.Values)
			{
				// normalize the center of mass to radar dimensions
				Vector3 com = currentUser.Position;
				radarPosition = new Vector2(com.x / RadarRealWorldDimensions.x, -com.z / RadarRealWorldDimensions.y);
				
				// X axis: 0 in real world is actually 0.5 in radar units (middle of field of view)
				radarPosition.x += 0.5f;

				// clamp
				radarPosition.x = Mathf.Clamp(radarPosition.x, 0.0f, 1.0f);
				radarPosition.y = Mathf.Clamp(radarPosition.y, 0.0f, 1.0f);
	
				// draw
	            Color orig = GUI.color;
	            GUI.color = (currentUser.SkeletonTracked) ? Color.blue : Color.red;
		//		GUI.Box(new Rect(radarPosition.x * width - 10, radarPosition.y * height - 20, 20, 20), currentUser.Id.ToString());
				
				if(KinectGUI.SeatedMode==true)
				{
					GUI.Box(new Rect(radarPosition.x * nrwidth-150 , radarPosition.y * nrheight-20, 60, 60), " ");//on seated mode
				}
				else
				{
				GUI.Box(new Rect(radarPosition.x * width - 10, radarPosition.y * height - 20, 20, 20), " ");
				}
				
	            GUI.color = orig;

			}
			GUI.EndGroup();
		}//if kinect menu
	}
}
