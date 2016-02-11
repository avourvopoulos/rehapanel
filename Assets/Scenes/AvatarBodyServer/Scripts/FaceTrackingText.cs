using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;
using Microsoft.Kinect;

public class FaceTrackingText : MonoBehaviour
{

    public GameObject FaceSourceManager;
    private FaceSourceManager _FaceManager;

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FaceSourceManager == null || BodySourceManager == null)
        {
            return;
        }

        _FaceManager = FaceSourceManager.GetComponent<FaceSourceManager>();
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_FaceManager == null || _BodyManager == null)
        {
            return;
        }

        Kinect.Body[] dataBody = _BodyManager.GetData();
        var dataFace = _FaceManager.GetData();
        if (dataFace == null || dataBody == null)
        {
            return;
        }

        //Todo: make it specific for individual faces
        foreach (var face in dataFace)
        {
            if (face == null)
            {
                continue;
            }
            string faceText = string.Empty;
            if (face.FaceProperties != null)
            {
                foreach (var item in face.FaceProperties)
                {
                    faceText += item.Key.ToString() + " : ";

					string Message = "";
					Message = Message + "[$]" + "button," + "[$$]" + "kinect," + "[$$$]";
					Message = Message + item.Key.ToString() + ",";
					Message = Message + "value,";
					Message = Message + item.Value + ";";
					Message = Message.ToLower();

					if(!DevicesLists.availableDev.Contains("KINECT2:BUTTON:FACE:ALL"))
					{
						DevicesLists.availableDev.Add("KINECT2:BUTTON:FACE:ALL");		
					}
					if(DevicesLists.selectedDev.Contains("KINECT2:BUTTON:FACE:ALL") && UDPData.flag==true)
					{					
						UDPData.sendString(Message);
						Debug.Log(Message);
					}	

                    // consider a "maybe" as a "no" to restrict 
                    // the detection result refresh rate
                    if (item.Value == Kinect.DetectionResult.Maybe)
                    {
                        faceText += Kinect.DetectionResult.No + "\n";
                    }
                    else
                    {
                        faceText += item.Value.ToString() + "\n";
                    }
                }
            }

            guiText.text = faceText;

			//Debug.Log(faceText);
            //dataFace.ToString();
        }
    }
}
