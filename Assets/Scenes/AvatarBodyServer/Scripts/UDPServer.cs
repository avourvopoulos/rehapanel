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
    //UdpClient sender;
    //public int localPort = 1201;
    //public int remotePort = 1202;
    //public string ipAddress = "192.168.10.101";

    public GameObject BodySourceManager;
    private BodySourceManager _bodyManager;

    public GameObject UserInterface;
    private UserInterface _userInterface;

    public GameObject AvatarCarl;

    public float WriteFrequency = 30f;
    private bool _first = true;

	private KinectSensor _sensor;

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
        _userInterface = UserInterface.GetComponent<UserInterface>();
        //t1 = UnityEngine.Time.time;
        InvokeRepeating("SendData", 0.1f, 1f / WriteFrequency);
    }
    
	void Update()
	{
		_sensor = KinectSensor.GetDefault();
		
		if (_sensor != null)
            _userInterface.ipGo = _sensor.IsOpen;

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
            if (_first)
            {
//                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(_userInterface.ipAddress), remotePort);
//               sender.Connect(groupEP);
                _first = false;
            }

            if (BodySourceManager == null)
            {
                return;
            }

            _bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
            if (_bodyManager == null)
            {
                return;
            }

            Kinect.Body[] data = _bodyManager.GetData();
            if (data == null)
            {
                return;
            }

            string message = "";

            //Finds which body is closer to the Kinect
            int closestBodyIndex = 0;
            Single distance = 10000; //10m or 10 000 m? doesn't matter just has to be big
            int bodyindex = 0;
            foreach (var body in data)
            {
                bodyindex = bodyindex + 1;
                if (body == null)
                    continue;

                if (body.IsTracked)
                {
                    if (distance > AvatarCarl.transform.Find(AvatarJoint[Kinect.JointType.SpineBase]).rotation.z)
                    {
                        closestBodyIndex = bodyindex - 1;
                    }
                    distance = AvatarCarl.transform.Find(AvatarJoint[Kinect.JointType.SpineBase]).rotation.z;
                }
            }

            bodyindex = 0;
            foreach (var body in data)
            {
                bodyindex = bodyindex + 1;
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    //// Sending the tracked body id:
                    //message = "[$]" + "tracking," + "[$$]" + "kinectv2," + "[$$$]" + "trackingid," + body.TrackingId.ToString() + ";";
                    //sender.Send(Encoding.ASCII.GetBytes(message), message.Length);
                    ////print(message);

					message = message + "[$]" + "tracking," + "[$$]" + "kinect," + "[$$$]" + "index," + "number," + body.TrackingId.ToString()+";";

//					Debug.Log(body.TrackingId.ToString());

                    foreach (Kinect.JointType joint in Enum.GetValues(typeof(Kinect.JointType)))
                    {
                        message = JointMensage(joint, message, "kinect,");
                        //todo: the false must be changed to a variable that is true when the body in qustion is the target player
                        if (closestBodyIndex == bodyindex - 1)
                            message = JointMensage(joint, message, "kinect,detected,");
                    }
                }
            }

            //if (message != "")
            //{
            //    sr.WriteLine(message);
            //    //print(message.Length);
            //}
			if(!DevicesLists.availableDev.Contains("KINECT2:TRACKING:JOINTS:ALL"))
			{
				DevicesLists.availableDev.Add("KINECT2:TRACKING:JOINTS:ALL");		
			}
			if(DevicesLists.selectedDev.Contains("KINECT2:TRACKING:JOINTS:ALL") && UDPData.flag==true)
			{					
				UDPData.sendString(message);
			//	Debug.Log(message);
			}	

            //sender.Send(Encoding.ASCII.GetBytes(message), message.Length);
        }


    }

    private string JointMensage(JointType joint, string message, string device)
    {
        // Sending the tracked body joint orientation in kinect v1 format:
        if (AvatarJoint.ContainsKey(joint) && KinectV1Joint.ContainsKey(joint))
        {
            message = message + "[$]" + "tracking," + "[$$]" + device + "[$$$]";
            message = message + KinectV1Joint[joint] + ",";
            message = message + "rotation,";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.x + ",";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.y + ",";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.z + ",";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).rotation.w + ";";
            //sender.Send(Encoding.ASCII.GetBytes(message), message.Length);
            //print(message);
        }
        // Sending the tracked body joint position in kinect v1 format:
        if (AvatarJoint.ContainsKey(joint) && KinectV1Joint.ContainsKey(joint)) // && KinectV1Joint[joint] == "waist")
        {
            //message = "";
            message = message + "[$]" + "tracking," + "[$$]" + device + "[$$$]";
            message = message + KinectV1Joint[joint] + ",";
            message = message + "position,";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.x + ",";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.y + ",";
            message = message + AvatarCarl.transform.Find(AvatarJoint[joint]).position.z + ";";
            //sender.Send(Encoding.ASCII.GetBytes(message), message.Length);
            //print(message);
        }
        return message;
    }
}
