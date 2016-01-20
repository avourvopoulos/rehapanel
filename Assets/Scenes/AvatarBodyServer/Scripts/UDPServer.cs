using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kinect = Windows.Kinect;
using Windows.Kinect;

public class UDPServer : MonoBehaviour
{
    UdpClient sender;
    public int localPort = 1201;
    public int remotePort = 1202;
    //public string ipAddress = "192.168.10.101";

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    public GameObject userInterface;
    private UserInterface _userInterface;

    public GameObject AvatarCarl;

    public float WriteFrequency = 30f;
    private bool first = true;

	private KinectSensor _Sensor;

    //string fileName = "udp_debug.txt";
    //StreamWriter sr;
    //System.Single t1;
    //System.Single t2;

    private Dictionary<Kinect.JointType, String> AvatarJoint = new Dictionary<Kinect.JointType, String>()
    {
        { Kinect.JointType.SpineBase,       "Hips" },
        { Kinect.JointType.SpineMid,        "Hips/Spine" },
        { Kinect.JointType.Neck,            "Hips/Spine/Spine1/Spine2" },
        { Kinect.JointType.Head,            "Hips/Spine/Spine1/Spine2/Neck/Neck1" },
        { Kinect.JointType.ShoulderLeft,    "Hips/Spine/Spine1/Spine2/LeftShoulder" },
        { Kinect.JointType.ElbowLeft,       "Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm" },
        { Kinect.JointType.WristLeft,       "Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm" },
        { Kinect.JointType.HandLeft,        "Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand" },
        { Kinect.JointType.ShoulderRight,   "Hips/Spine/Spine1/Spine2/RightShoulder" },
        { Kinect.JointType.ElbowRight,      "Hips/Spine/Spine1/Spine2/RightShoulder/RightArm" },
        { Kinect.JointType.WristRight,      "Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm" },
        { Kinect.JointType.HandRight,       "Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand" },
        //{ Kinect.JointType.HipLeft,         "" },
        { Kinect.JointType.KneeLeft,        "Hips/LeftUpLeg" },
        { Kinect.JointType.AnkleLeft,       "Hips/LeftUpLeg/LeftLeg" },
        { Kinect.JointType.FootLeft,        "Hips/LeftUpLeg/LeftLeg/LeftFoot" },
        //{ Kinect.JointType.HipRight,        "" },
        { Kinect.JointType.KneeRight,       "Hips/RightUpLeg" },
        { Kinect.JointType.AnkleRight,      "Hips/RightUpLeg/RightLeg" },
        { Kinect.JointType.FootRight,       "Hips/RightUpLeg/RightLeg/RightFoot" },
        //{ Kinect.JointType.SpineShoulder,   "Spine1" },
        //{ Kinect.JointType.HandTipLeft,     "" },
        //{ Kinect.JointType.ThumbLeft,       "" },
        //{ Kinect.JointType.HandTipRight,    "" },
        //{ Kinect.JointType.ThumbRight,      "" },
    };

    private Dictionary<Kinect.JointType, String> KinectV1Joint = new Dictionary<Kinect.JointType, String>()
    {
        { Kinect.JointType.SpineBase,       "waist" },
        { Kinect.JointType.SpineMid,        "torso" },
        { Kinect.JointType.Neck,            "neck" },
        { Kinect.JointType.Head,            "head" },
        //{ Kinect.JointType.ShoulderLeft,    "" },
        { Kinect.JointType.ElbowLeft,       "leftshoulder" },
        { Kinect.JointType.WristLeft,       "leftelbow" },
        { Kinect.JointType.HandLeft,        "leftwrist" },
        //{ Kinect.JointType.ShoulderRight,   "" },
        { Kinect.JointType.ElbowRight,      "rightshoulder" },
        { Kinect.JointType.WristRight,      "rightelbow" },
        { Kinect.JointType.HandRight,       "rightwrist" },
        //{ Kinect.JointType.HipLeft,         "" },
        { Kinect.JointType.KneeLeft,        "lefthip" },
        { Kinect.JointType.AnkleLeft,       "leftknee" },
        { Kinect.JointType.FootLeft,        "leftankle" },
        //{ Kinect.JointType.HipRight,        "" },
        { Kinect.JointType.KneeRight,       "righthip" },
        { Kinect.JointType.AnkleRight,      "rightknee" },
        { Kinect.JointType.FootRight,       "rightankle" },
        //{ Kinect.JointType.SpineShoulder,   "" },
        //{ Kinect.JointType.HandTipLeft,     "" },
        //{ Kinect.JointType.ThumbLeft,       "" },
        //{ Kinect.JointType.HandTipRight,    "" },
        //{ Kinect.JointType.ThumbRight,      "" },
    };

    void Start()
    {
        //sr = File.CreateText(fileName);
;
//        sender = new UdpClient(localPort, AddressFamily.InterNetwork);
        //IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
        //IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(ipAddress), remotePort);
        //sender.Connect(groupEP);
        _userInterface = userInterface.GetComponent<UserInterface>();
        //t1 = UnityEngine.Time.time;
        InvokeRepeating("SendData", 0.1f, 1f / WriteFrequency);
    }
    
	void Update()
	{
		_Sensor = KinectSensor.GetDefault();
		
		if (_Sensor != null) {
						if (_Sensor.IsOpen)
								_userInterface.ipGo = true;
						else
								_userInterface.ipGo = false;
				}
	}

    void OnApplicationQuit()
    {
        //sender.Close();
    }

    void OnDisable()
    {
        //sender.Close();
    }

    void SendData()
    {
        //t2 = UnityEngine.Time.time;
        //print(1/(t2 - t1));
        //t1 = t2;

        if (_userInterface.ipGo)
        {
            if (first)
            {
//                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(_userInterface.ipAddress), remotePort);
//               sender.Connect(groupEP);
                first = false;
            }

            if (BodySourceManager == null)
            {
                return;
            }

            _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
            if (_BodyManager == null)
            {
                return;
            }

            Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                return;
            }

            string Message = "";
            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    //// Sending the tracked body id:
                    //Message = "[$]" + "tracking," + "[$$]" + "kinectv2," + "[$$$]" + "trackingid," + body.TrackingId.ToString() + ";";
                    //sender.Send(Encoding.ASCII.GetBytes(Message), Message.Length);
                    ////print(Message);

					Message = Message + "[$]" + "tracking," + "[$$]" + "kinect," + "[$$$]" + "index," + "number," + body.TrackingId.ToString()+";";

//					Debug.Log(body.TrackingId.ToString());

                    foreach (Kinect.JointType joint in Enum.GetValues(typeof(Kinect.JointType)))
                    {
                        // Sending the tracked body joint orientation in kinect v1 format:
                        if (AvatarJoint.ContainsKey(joint) && KinectV1Joint.ContainsKey(joint))
                        {
                            Message = Message + "[$]" + "tracking," + "[$$]" + "kinect," + "[$$$]";
                            Message = Message + KinectV1Joint[joint] + ",";
                            Message = Message + "rotation,";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.x + ",";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.y + ",";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.z + ",";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.w + ";";
                            //sender.Send(Encoding.ASCII.GetBytes(Message), Message.Length);
                            //print(Message);
                        }
                        // Sending the tracked body joint position in kinect v1 format:
                        if (AvatarJoint.ContainsKey(joint) && KinectV1Joint.ContainsKey(joint))// && KinectV1Joint[joint] == "waist")
                        {
                            //Message = "";
                            Message = Message + "[$]" + "tracking," + "[$$]" + "kinect," + "[$$$]";
                            Message = Message + KinectV1Joint[joint] + ",";
                            Message = Message + "position,";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.x + ",";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.y + ",";
                            Message = Message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.z + ";";
                            //sender.Send(Encoding.ASCII.GetBytes(Message), Message.Length);
                            //print(Message);
                        }
                    }
                }
            }

            //if (Message != "")
            //{
            //    sr.WriteLine(Message);
            //    //print(Message.Length);
            //}
			if(!DevicesLists.availableDev.Contains("KINECT2:TRACKING:JOINTS:ALL"))
			{
				DevicesLists.availableDev.Add("KINECT2:TRACKING:JOINTS:ALL");		
			}
			if(DevicesLists.selectedDev.Contains("KINECT2:TRACKING:JOINTS:ALL") && UDPData.flag==true)
			{					
				UDPData.sendString(Message);
			//	Debug.Log(Message);
			}	

            //sender.Send(Encoding.ASCII.GetBytes(Message), Message.Length);
        }


    }
}
