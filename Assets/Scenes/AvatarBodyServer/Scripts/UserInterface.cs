using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    public GameObject Camera;
    public Vector3 avatarRoot = new Vector3(0f, 0f, 0f);
    public Vector3 stickmanRoot = new Vector3(30f, 0f, 0f);
//    public string ipAddress = "192.168.10.101";
    public bool ipGo = true;

	public Texture2D backIcon;

    // Use this for initialization
    void Start()
    {
		Camera.SetActive(false);

		Vector3 cameraPos = avatarRoot;
		cameraPos.y=0.9f;
		Camera.transform.position = cameraPos;
        //avatar = 0;
        //stickman = 30;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
		if(MainGuiControls.Kinect2Menu)
		{

			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.Kinect2Menu = false;
				MainGuiControls.hideMenus = false;
				Camera.SetActive(false);
			}	

//	        if (GUI.Button(new Rect(20, 70, 70, 40), "Avatar"))
//	        {
//	            Vector3 cameraPos = avatarRoot;
//	            cameraPos.y=0.9f;
//	            Camera.transform.position = cameraPos;
//	        }
//	        if (GUI.Button(new Rect(20, 20, 70, 40), "StickMan"))
//	        {
//	            Vector3 cameraPos = stickmanRoot;
//	            cameraPos.y = 0.9f;
//	            Camera.transform.position = cameraPos;
//	        }
//
//	        ipAddress = GUI.TextField(new Rect(500, 10, 200, 20), ipAddress, 25);
//	        if (GUI.Button(new Rect(500, 70, 70, 40), "UDP GO"))
//	        {
//	            ipGo = true;
//	        }

		}
    }
}
