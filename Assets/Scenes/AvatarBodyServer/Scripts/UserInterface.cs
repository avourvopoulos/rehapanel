using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public GameObject Camera;
    public Vector3 avatarRoot = new Vector3(0f, 0f, 0f);
    public Vector3 stickmanRoot = new Vector3(30f, 0f, 0f);
    public bool ipGo = true;
    public static bool Kinect2On = false;

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

            //todo: put on/off button here
            Kinect2On = GUI.Toggle(new Rect(10, 50, 100, 50), Kinect2On, "On/Off");
		}
    }
}
