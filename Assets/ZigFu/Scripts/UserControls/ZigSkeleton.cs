using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LSL;

public class ZigSkeleton : MonoBehaviour{

	//	LSL- create stream info and outlet
	static liblsl.StreamInfo info;
	static liblsl.StreamOutlet outlet;
	static float[] data;


	public Texture2D upperbody;
	
	public static GameObject nBody,nHead,nNeck,nTorso,nWaist,nLeftShoulder,nLeftElbow,nLeftWrist,nRightShoulder,nRightElbow,nRightWrist,nLeftHip,nLeftKnee,nLeftAnkle,nRightHip,nRightKnee,nRightAnkle;
	public static Vector3 pBody,pHead,pNeck,pTorso,pWaist,pLeftShoulder,pLeftElbow,pLeftWrist,pRightShoulder,pRightElbow,pRightWrist;
	public static Quaternion rBody,rHead,rNeck,rTorso,rWaist,rLeftShoulder,rLeftElbow,rLeftWrist,rRightShoulder,rRightElbow,rRightWrist;

    public Transform Body;
	
	public Transform Head;
    public Transform Neck;
    public Transform Torso;
    public Transform Waist;

    public Transform LeftCollar;
    public Transform LeftShoulder;
    public Transform LeftElbow;
    public Transform LeftWrist;
    public Transform LeftHand;
    public Transform LeftFingertip;

    public Transform RightCollar;
    public Transform RightShoulder;
    public Transform RightElbow;
    public Transform RightWrist;
    public Transform RightHand;
	public Transform RightFingertip;

    public Transform LeftHip;
    public Transform LeftKnee;
    public Transform LeftAnkle;
    public Transform LeftFoot;

    public Transform RightHip;
    public Transform RightKnee;
    public Transform RightAnkle;
    public Transform RightFoot;
	
    public static bool mirror = false;
    public bool UpdateJointPositions = false;
    public bool UpdateRootPosition = false;
    public bool UpdateOrientation = true;
    public bool RotateToPsiPose = false;
    public float RotationDamping = 30.0f;
    public float Damping = 30.0f;
    public Vector3 Scale = new Vector3(0.001f, 0.001f, 0.001f);

    public Vector3 PositionBias = Vector3.zero;

    public static Transform[] transforms;
    private Quaternion[] initialRotations;
    private Vector3 rootPosition;
	
	public GameObject Dana;
	public GameObject Carl;

	string DeviceName = string.Empty;
	

    ZigJointId mirrorJoint(ZigJointId joint)
    {
        switch (joint)
        {
            case ZigJointId.LeftCollar: 
                return ZigJointId.RightCollar;
            case ZigJointId.LeftShoulder:
                return ZigJointId.RightShoulder;
            case ZigJointId.LeftElbow:
                return ZigJointId.RightElbow;
            case ZigJointId.LeftWrist:
                return ZigJointId.RightWrist;
            case ZigJointId.LeftHand:
                return ZigJointId.RightHand;
            case ZigJointId.LeftFingertip:
                return ZigJointId.RightFingertip;
            case ZigJointId.LeftHip:
                return ZigJointId.RightHip;
            case ZigJointId.LeftKnee:
                return ZigJointId.RightKnee;
            case ZigJointId.LeftAnkle:
                return ZigJointId.RightAnkle;
            case ZigJointId.LeftFoot:
                return ZigJointId.RightFoot;

            case ZigJointId.RightCollar:
                return ZigJointId.LeftCollar;
            case ZigJointId.RightShoulder:
                return ZigJointId.LeftShoulder;
            case ZigJointId.RightElbow:
                return ZigJointId.LeftElbow;
            case ZigJointId.RightWrist:
                return ZigJointId.LeftWrist;
            case ZigJointId.RightHand:
                return ZigJointId.LeftHand;
            case ZigJointId.RightFingertip:
                return ZigJointId.LeftFingertip;
            case ZigJointId.RightHip:
                return ZigJointId.LeftHip;
            case ZigJointId.RightKnee:
                return ZigJointId.LeftKnee;
            case ZigJointId.RightAnkle:
                return ZigJointId.LeftAnkle;
            case ZigJointId.RightFoot:
                return ZigJointId.LeftFoot;


            default:
                return joint;
        }
    }
	
	
	public static bool toggle1 = false; //Head
	public static bool toggle2 = false; //Neck
	public static bool toggle3 = false; //Torso
	public static bool toggle4 = false; //Waist
	
	public static bool toggle5 = false; //Left Shoulder 	
	public static bool toggle6 = false; //Left Elbow
	public static bool toggle7 = false; //Left Wrist
	
	public static bool toggle8 = false; //Right Shoulder
	public static bool toggle9 = false; //Right Elbow
	public static bool toggle10 = false; //Right Wrist
	
	public static bool toggle11 = false; //Left Hip
	public static bool toggle12 = false; //Left Knee
	public static bool toggle13 = false; //Left Ankle
	
	public static bool toggle14 = false; //RightHip
	public static bool toggle15 = false; //RightKnee
	public static bool toggle16 = false; //RightAnkle
	
	bool alljoints = false;
	
	bool addDataToList = true;
	
	//lowpass filter
//	public float ALPHA = 0.15f;
//	public float[] n_input = new float[3];//xy in/out
//	public float[] n_output = new float[3];//xy in/out
//	Vector3 FilteredPosition;
//	float delta;
//	bool deltaon = false;

	void Start()
	{
		
		// LSL - create stream info and outlet
		info = new liblsl.StreamInfo("kinect1", "kinematics", 3, 60, liblsl.channel_format_t.cf_float32, "rehabnet");
		outlet = new liblsl.StreamOutlet(info);
		data = new float[8];
		
		
		Dana.SetActive(false);
		Carl.SetActive(true);
		
		//		nBody.tag="Body";
		//		nHead.tag="Head";
		//		nNeck.tag="Neck";
		//		nTorso.tag="Torso";
		//		nWaist.tag="Waist";
		
		// start out in calibration pose
		if (RotateToPsiPose)
		{
			RotateToCalibrationPose();
		}
	}

	void Update()
	{
		// LSL - send
		//for (int k = 0; k < data.Length; k++) {
				data [0] = LeftWrist.transform.position.x;
				data [1] = LeftWrist.transform.position.y;
				data [2] = LeftWrist.transform.position.z;
				outlet.push_sample (data);
//		}
	}

	void FixedUpdate()
    {
		
		//network model
		if(CameraSwitch.gender=="female")
		{
			Dana.SetActive(true);
			Carl.SetActive(false);
		}
		else
		{
			Dana.SetActive(false);
			Carl.SetActive(true);
		}
		
		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "kinect";
		}
	
if(KinectGUI.startzigfu)
	{
		sendTracking();
			
			
		pBody = new Vector3(Body.transform.position.x, Body.transform.position.y, Body.transform.position.z); 
		rBody = new Quaternion(Body.transform.rotation.x, Body.transform.rotation.y, Body.transform.rotation.z, Body.transform.rotation.w);
//		nBody.transform.position = pBody; 
//		nBody.transform.rotation = rBody;	
			
		
		pHead = new Vector3(Head.transform.position.x, Head.transform.position.y, Head.transform.position.z);
		rHead = new Quaternion(Head.transform.rotation.x, Head.transform.rotation.y, Head.transform.rotation.z, Head.transform.rotation.w);
//		nHead.transform.position =  pHead;
//		nHead.transform.rotation = 	rHead;
			
			
		pNeck = new Vector3(Neck.transform.position.x, Neck.transform.position.y, Neck.transform.position.z);
		rNeck = new Quaternion(Neck.transform.rotation.x, Neck.transform.rotation.y, Neck.transform.rotation.z, Neck.transform.rotation.w);
//		nNeck.transform.position =  pNeck;
//		nNeck.transform.rotation = 	rNeck;	
			

		pTorso = new Vector3(Torso.transform.position.x, Torso.transform.position.y, Torso.transform.position.z);
		rTorso = new Quaternion(Torso.transform.rotation.x, Torso.transform.rotation.y, Torso.transform.rotation.z, Torso.transform.rotation.w);
//		nTorso.transform.position = pTorso;
//		nTorso.transform.rotation = rTorso;	
			
			
		pWaist = new Vector3(Waist.transform.position.x, Waist.transform.position.y, Waist.transform.position.z);
		rWaist = new Quaternion(Waist.transform.rotation.x, Waist.transform.rotation.y, Waist.transform.rotation.z, Waist.transform.rotation.w);
//		nWaist.transform.position = pWaist;
//		nWaist.transform.rotation = rWaist;		
			
		
		pLeftShoulder = new Vector3(LeftShoulder.transform.position.x, LeftShoulder.transform.position.y, LeftShoulder.transform.position.z);
		rLeftShoulder = new Quaternion(LeftShoulder.transform.rotation.x, LeftShoulder.transform.rotation.y, LeftShoulder.transform.rotation.z, LeftShoulder.transform.rotation.w);	
			
		
		pLeftElbow = new Vector3(LeftElbow.transform.position.x, LeftElbow.transform.position.y, LeftElbow.transform.position.z);
		rLeftElbow = new Quaternion(LeftElbow.transform.rotation.x, LeftElbow.transform.rotation.y, LeftElbow.transform.rotation.z, LeftElbow.transform.rotation.w);
		
				
		pLeftWrist = new Vector3(LeftWrist.transform.position.x, LeftWrist.transform.position.y, LeftWrist.transform.position.z);
		rLeftWrist = new Quaternion(LeftWrist.transform.rotation.x, LeftWrist.transform.rotation.y, LeftWrist.transform.rotation.z, LeftWrist.transform.rotation.w);	
			
			
		pRightShoulder = new Vector3(RightShoulder.transform.position.x, RightShoulder.transform.position.y, RightShoulder.transform.position.z);
		rRightShoulder = new Quaternion(RightShoulder.transform.rotation.x, RightShoulder.transform.rotation.y, RightShoulder.transform.rotation.z, RightShoulder.transform.rotation.w);

		
		pRightElbow = new Vector3(RightElbow.transform.position.x, RightElbow.transform.position.y, RightElbow.transform.position.z);
		rRightElbow = new Quaternion(RightElbow.transform.rotation.x, RightElbow.transform.rotation.y, RightElbow.transform.rotation.z, RightElbow.transform.rotation.w);
			
			
		pRightWrist = new Vector3(RightWrist.transform.position.x, RightWrist.transform.position.y, RightWrist.transform.position.z);
		rRightWrist = new Quaternion(RightWrist.transform.rotation.x, RightWrist.transform.rotation.y, RightWrist.transform.rotation.z, RightWrist.transform.rotation.w);
			
	//		print("RightKnee: "+RightKnee.transform.position);
	}
	
//	    nLeftShoulder=LeftShoulder;
//	    nLeftElbow=LeftElbow;
//	    nLeftWrist=LeftWrist;
//	
//	    nRightShoulder=RightShoulder;
//	    nRightElbow=RightElbow;
//	    nRightWrist=RightWrist;
	
		
//	    nLeftHip=LeftHip;
//	    nLeftKnee=LeftKnee;
//	    nLeftAnkle=LeftAnkle;
//	
//	    nRightHip=RightHip;
//	    nRightKnee=RightKnee;
//	    nRightAnkle=RightAnkle;
		
		
	}
	

    public void Awake()
    {

		
        int jointCount = Enum.GetNames(typeof(ZigJointId)).Length;

        transforms = new Transform[jointCount];
        initialRotations = new Quaternion[jointCount];
		
        transforms[(int)ZigJointId.Head] = Head;
        transforms[(int)ZigJointId.Neck] = Neck;
        transforms[(int)ZigJointId.Torso] = Torso;
        transforms[(int)ZigJointId.Waist] = Waist;
 //       transforms[(int)ZigJointId.LeftCollar] = LeftCollar;
        transforms[(int)ZigJointId.LeftShoulder] = LeftShoulder;
        transforms[(int)ZigJointId.LeftElbow] = LeftElbow;
        transforms[(int)ZigJointId.LeftWrist] = LeftWrist;
//        transforms[(int)ZigJointId.LeftHand] = LeftHand;
//        transforms[(int)ZigJointId.LeftFingertip] = LeftFingertip;
//        transforms[(int)ZigJointId.RightCollar] = RightCollar;
        transforms[(int)ZigJointId.RightShoulder] = RightShoulder;
        transforms[(int)ZigJointId.RightElbow] = RightElbow;
        transforms[(int)ZigJointId.RightWrist] = RightWrist;
//        transforms[(int)ZigJointId.RightHand] = RightHand;
//        transforms[(int)ZigJointId.RightFingertip] = RightFingertip;
        transforms[(int)ZigJointId.LeftHip] = LeftHip;
        transforms[(int)ZigJointId.LeftKnee] = LeftKnee;
        transforms[(int)ZigJointId.LeftAnkle] = LeftAnkle;
//        transforms[(int)ZigJointId.LeftFoot] = LeftFoot;
        transforms[(int)ZigJointId.RightHip] = RightHip;
        transforms[(int)ZigJointId.RightKnee] = RightKnee;
        transforms[(int)ZigJointId.RightAnkle] = RightAnkle;
//        transforms[(int)ZigJointId.RightFoot] = RightFoot;



        // save all initial rotations
        // NOTE: Assumes skeleton model is in "T" pose since all rotations are relative to that pose
        foreach (ZigJointId j in Enum.GetValues(typeof(ZigJointId)))
        {
            if (transforms[(int)j])
            {
                // we will store the relative rotation of each joint from the gameobject rotation
                // we need this since we will be setting the joint's rotation (not localRotation) but we 
                // still want the rotations to be relative to our game object
                initialRotations[(int)j] = Quaternion.Inverse(transform.rotation) * transforms[(int)j].rotation;
            }
        }
    }



    void UpdateRoot(Vector3 skelRoot)
    {
        // +Z is backwards in OpenNI coordinates, so reverse it
        rootPosition = Vector3.Scale(new Vector3(skelRoot.x, skelRoot.y, skelRoot.z), doMirror(Scale)) + PositionBias;
        if (UpdateRootPosition)
        {
            transform.localPosition = (transform.rotation * rootPosition);
        }
    }

    void UpdateRotation(ZigJointId joint, Quaternion orientation)
    {
        joint = mirror ? mirrorJoint(joint) : joint;
        // make sure something is hooked up to this joint
        if (!transforms[(int)joint])
        {
            return;
        }

        if (UpdateOrientation)
        {
            Quaternion newRotation = transform.rotation * orientation * initialRotations[(int)joint];
            if (mirror)
            {
                newRotation.y = -newRotation.y;
                newRotation.z = -newRotation.z;
            }
            transforms[(int)joint].rotation = Quaternion.Slerp(transforms[(int)joint].rotation, newRotation, Time.deltaTime * RotationDamping);
        }
    }
    Vector3 doMirror(Vector3 vec)
    {
        return new Vector3(mirror ? -vec.x : vec.x, vec.y, vec.z);
    }
    void UpdatePosition(ZigJointId joint, Vector3 position)
    {
        joint = mirror ? mirrorJoint(joint) : joint;
        // make sure something is hooked up to this joint
        if (!transforms[(int)joint])
        {
            return;
        }

        if (UpdateJointPositions)
        {
            Vector3 dest = Vector3.Scale(position, doMirror(Scale)) - rootPosition;
            transforms[(int)joint].localPosition = Vector3.Lerp(transforms[(int)joint].localPosition, dest, Time.deltaTime * Damping);
        }
    }

    public void RotateToCalibrationPose()
    {
        foreach (ZigJointId j in Enum.GetValues(typeof(ZigJointId)))
        {
            if (null != transforms[(int)j])
            {
                transforms[(int)j].rotation = transform.rotation * initialRotations[(int)j];
            }
        }

        // calibration pose is skeleton base pose ("T") with both elbows bent in 90 degrees
        if (null != RightElbow)
        {
            RightElbow.rotation = transform.rotation * Quaternion.Euler(0, -90, 90) * initialRotations[(int)ZigJointId.RightElbow];
        }
        if (null != LeftElbow)
        {
            LeftElbow.rotation = transform.rotation * Quaternion.Euler(0, 90, -90) * initialRotations[(int)ZigJointId.LeftElbow];
        }
    }

    public void SetRootPositionBias()
    {
        this.PositionBias = -rootPosition;
    }

    public void SetRootPositionBias(Vector3 bias)
    {
        this.PositionBias = bias;
    }

    void Zig_UpdateUser(ZigTrackedUser user)
    {
        UpdateRoot(user.Position);
        if (user.SkeletonTracked)
        {
            foreach (ZigInputJoint joint in user.Skeleton)
            {
                if (joint.GoodPosition) UpdatePosition(joint.Id, joint.Position);
                if (joint.GoodRotation) UpdateRotation(joint.Id, joint.Rotation);
            }
        }
    }
	
	
	void sendTracking()
	{
//		if(UDPData.flag==true)
//		{
		if(DevicesLists.selectedDev.Contains("KINECT:BUTTON:BODY:STATUS") && UDPData.flag==true)
		{
			UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]body,status,"+Zig.trackeduser+";");
		}
		if(!DevicesLists.availableDev.Contains("KINECT:BUTTON:BODY:STATUS"))
		{
			DevicesLists.availableDev.Add("KINECT:BUTTON:BODY:STATUS");
		}

			//Body
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:BODY:ROTATION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]body,rotation,"+Body.transform.rotation.x.ToString()+","+Body.transform.rotation.y.ToString()+","+Body.transform.rotation.z.ToString()+","+Body.transform.rotation.w.ToString()+";"); // quaternions
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:BODY:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]body,position,"+Body.transform.position.x.ToString()+","+Body.transform.position.y.ToString()+","+Body.transform.position.z.ToString()+";");
			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:BODY:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:BODY:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:BODY:ROTATION");
			}
//			
//			//network model
			if(CameraSwitch.gender=="female")
			{
				Dana.SetActive(true);
				Carl.SetActive(false);
			GameObject.FindGameObjectWithTag("K1Dana").transform.rotation = Body.transform.rotation;// * ModelVisualisation.modelrotation;//plus xml bias rotation
			}
			else
			{
				Dana.SetActive(false);
				Carl.SetActive(true);
				GameObject.FindGameObjectWithTag("K1Carl").transform.rotation = Body.transform.rotation;
			}

		//Head
		if (toggle1 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:HEAD:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]head,position,"+Head.transform.position.x.ToString()+","+Head.transform.position.y.ToString()+","+Head.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:HEAD:ROTATION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]head,rotation,"+Head.transform.rotation.x.ToString()+","+Head.transform.rotation.y.ToString()+","+Head.transform.rotation.z.ToString()+","+Head.transform.rotation.w.ToString()+";"); // quaternions
			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:HEAD:POSITION"))
			{		
				DevicesLists.availableDev.Add("KINECT:TRACKING:HEAD:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:HEAD:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1Head").transform.rotation = Head.transform.rotation;
		}
			
		// Neck
		if (toggle2 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:NECK:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]neck,position,"+Neck.transform.position.x.ToString()+","+Neck.transform.position.y.ToString()+","+Neck.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:NECK:ROTATION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]neck,rotation,"+Neck.transform.rotation.x.ToString()+","+Neck.transform.rotation.y.ToString()+","+Neck.transform.rotation.z.ToString()+","+Neck.transform.rotation.w.ToString()+";");
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:NECK:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:NECK:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:NECK:ROTATION");	
			}
			//network model
			GameObject.FindGameObjectWithTag("K1Neck").transform.rotation=Neck.transform.rotation;
		}
			
		// Torso
		if (toggle3 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:TORSO:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]torso,position,"+Torso.transform.position.x.ToString()+","+Torso.transform.position.y.ToString()+","+Torso.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:TORSO:ROTATION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]torso,rotation,"+Torso.transform.rotation.x.ToString()+","+Torso.transform.rotation.y.ToString()+","+Torso.transform.rotation.z.ToString()+","+Torso.transform.rotation.w.ToString()+";");
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:TORSO:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:TORSO:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:TORSO:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1Spine1").transform.rotation = Torso.transform.rotation;	
		}
				
		//Waist
		if (toggle4 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:WAIST:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]waist,position,"+Waist.transform.position.x.ToString()+","+Waist.transform.position.y.ToString()+","+Waist.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:WAIST:ROTATION") && UDPData.flag==true)
			{			
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]waist,rotation,"+Waist.transform.rotation.x.ToString()+","+Waist.transform.rotation.y.ToString()+","+Waist.transform.rotation.z.ToString()+","+Waist.transform.rotation.w.ToString()+";");
			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:WAIST:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:WAIST:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:WAIST:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1Spine").transform.rotation = Waist.transform.rotation;
		}	
		
		// LeftShoulder
		//	if (toggle8 == true)//mirror
		if (toggle5 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTSHOULDER:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftshoulder,position,"+LeftShoulder.transform.position.x.ToString()+","+LeftShoulder.transform.position.y.ToString()+","+LeftShoulder.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTSHOULDER:ROTATION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftshoulder,rotation,"+LeftShoulder.transform.rotation.x.ToString()+","+LeftShoulder.transform.rotation.y.ToString()+","+LeftShoulder.transform.rotation.z.ToString()+","+LeftShoulder.transform.rotation.w.ToString()+";");
			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTSHOULDER:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTSHOULDER:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTSHOULDER:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftArm").transform.rotation = LeftShoulder.transform.rotation;
		}
					
		// LeftElbow
		//	if (toggle9 == true)
		if (toggle6 == true)
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTELBOW:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftelbow,position,"+LeftElbow.transform.position.x.ToString()+","+LeftElbow.transform.position.y.ToString()+","+LeftElbow.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTELBOW:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftelbow,rotation,"+LeftElbow.transform.rotation.x.ToString()+","+LeftElbow.transform.rotation.y.ToString()+","+LeftElbow.transform.rotation.z.ToString()+","+LeftElbow.transform.rotation.w.ToString()+";");
			//	print("leftelbow "+LeftElbow.transform.rotation.eulerAngles);

			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTELBOW:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTELBOW:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTELBOW:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftForeArm").transform.rotation = LeftElbow.transform.rotation;	
		}
					
		// LeftWrist
		//	if (toggle10 == true)
		if (toggle7 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTWRIST:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftwrist,position,"+LeftWrist.transform.position.x.ToString()+","+LeftWrist.transform.position.y.ToString()+","+LeftWrist.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTWRIST:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftwrist,rotation,"+LeftWrist.transform.rotation.x.ToString()+","+LeftWrist.transform.rotation.y.ToString()+","+LeftWrist.transform.rotation.z.ToString()+","+LeftWrist.transform.rotation.w.ToString()+";");	
			//	print("[$]tracking,[$$]"+DeviceName+",[$$$]leftwrist,rotation,"+LeftWrist.transform.rotation.x.ToString()+","+LeftWrist.transform.rotation.y.ToString()+","+LeftWrist.transform.rotation.z.ToString()+","+LeftWrist.transform.rotation.w.ToString()+";");		

			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTWRIST:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTWRIST:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTWRIST:ROTATION");	
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftHand").transform.rotation = LeftWrist.transform.rotation;



		}
				
		// RightShoulder
		//	if (toggle5 == true)//mirror
		if (toggle8 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTSHOULDER:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightshoulder,position,"+RightShoulder.transform.position.x.ToString()+","+RightShoulder.transform.position.y.ToString()+","+RightShoulder.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTSHOULDER:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightshoulder,rotation,"+RightShoulder.transform.rotation.x.ToString()+","+RightShoulder.transform.rotation.y.ToString()+","+RightShoulder.transform.rotation.z.ToString()+","+RightShoulder.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTSHOULDER:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTSHOULDER:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTSHOULDER:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightArm").transform.rotation = RightShoulder.transform.rotation;
		}
				
		// RightElbow
		//	if (toggle6 == true)
		if (toggle9 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTELBOW:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightelbow,position,"+RightElbow.transform.position.x.ToString()+","+RightElbow.transform.position.y.ToString()+","+RightElbow.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTELBOW:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightelbow,rotation,"+RightElbow.transform.rotation.x.ToString()+","+RightElbow.transform.rotation.y.ToString()+","+RightElbow.transform.rotation.z.ToString()+","+RightElbow.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTELBOW:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTELBOW:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTELBOW:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightForeArm").transform.rotation = RightElbow.transform.rotation;	
		}
				
		// RightWrist
		//	if (toggle7 == true)
		if (toggle10 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTWRIST:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightwrist,position,"+RightWrist.transform.position.x.ToString()+","+RightWrist.transform.position.y.ToString()+","+RightWrist.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTWRIST:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightwrist,rotation,"+RightWrist.transform.rotation.x.ToString()+","+RightWrist.transform.rotation.y.ToString()+","+RightWrist.transform.rotation.z.ToString()+","+RightWrist.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTWRIST:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTWRIST:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTWRIST:ROTATION");	
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightHand").transform.rotation = RightWrist.transform.rotation;	
		}
		
		// LeftHip
		//	if (toggle14 == true)
		if (toggle11 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTHIP:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]lefthip,position,"+LeftHip.transform.position.x.ToString()+","+LeftHip.transform.position.y.ToString()+","+LeftHip.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTHIP:ROTATION") && UDPData.flag==true)
			{			
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]lefthip,rotation,"+LeftHip.transform.rotation.x.ToString()+","+LeftHip.transform.rotation.y.ToString()+","+LeftHip.transform.rotation.z.ToString()+","+LeftHip.transform.rotation.w.ToString()+";");		
			}
			
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTHIP:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTHIP:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTHIP:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftUpLeg").transform.rotation = LeftHip.transform.rotation;
		}
				
		// LeftKnee
		//	if (toggle15 == true)
		if (toggle12 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTKNEE:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftknee,position,"+LeftKnee.transform.position.x.ToString()+","+LeftKnee.transform.position.y.ToString()+","+LeftKnee.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTKNEE:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftknee,rotation,"+LeftKnee.transform.rotation.x.ToString()+","+LeftKnee.transform.rotation.y.ToString()+","+LeftKnee.transform.rotation.z.ToString()+","+LeftKnee.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTKNEE:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTKNEE:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTKNEE:ROTATION");	
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftLeg").transform.rotation = LeftKnee.transform.rotation;	
		}
				
		// LeftAnkle
		//	if (toggle16 == true)
		if (toggle13 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTANKLE:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftankle,position,"+LeftAnkle.transform.position.x.ToString()+","+LeftAnkle.transform.position.y.ToString()+","+LeftAnkle.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:LEFTANKLE:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]leftankle,rotation,"+LeftAnkle.transform.rotation.x.ToString()+","+LeftAnkle.transform.rotation.y.ToString()+","+LeftAnkle.transform.rotation.z.ToString()+","+LeftAnkle.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:LEFTANKLE:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTANKLE:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:LEFTANKLE:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1LeftFoot").transform.rotation = LeftAnkle.transform.rotation;	
		}	
		
		// RightHip
		//	if (toggle11 == true)
		if (toggle14 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTHIP:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]righthip,position,"+RightHip.transform.position.x.ToString()+","+RightHip.transform.position.y.ToString()+","+RightHip.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTHIP:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]righthip,rotation,"+RightHip.transform.rotation.x.ToString()+","+RightHip.transform.rotation.y.ToString()+","+RightHip.transform.rotation.z.ToString()+","+RightHip.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTHIP:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTHIP:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTHIP:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightUpLeg").transform.rotation = RightHip.transform.rotation;
		}
				
		// RightKnee
		//	if (toggle12 == true)
		if (toggle15 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTKNEE:POSITION") && UDPData.flag==true)
			{
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightknee,position,"+RightKnee.transform.position.x.ToString()+","+RightKnee.transform.position.y.ToString()+","+RightKnee.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTKNEE:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightknee,rotation,"+RightKnee.transform.rotation.x.ToString()+","+RightKnee.transform.rotation.y.ToString()+","+RightKnee.transform.rotation.z.ToString()+","+RightKnee.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTKNEE:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTKNEE:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTKNEE:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightLeg").transform.rotation = RightKnee.transform.rotation;
		}
				
		// RightAnkle
		//	if (toggle13 == true)
		if (toggle16 == true)	
		{
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTANKLE:POSITION") && UDPData.flag==true)
			{			
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightankle,position,"+RightAnkle.transform.position.x.ToString()+","+RightAnkle.transform.position.y.ToString()+","+RightAnkle.transform.position.z.ToString()+";");
			}
			if(DevicesLists.selectedDev.Contains("KINECT:TRACKING:RIGHTANKLE:ROTATION") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]rightankle,rotation,"+RightAnkle.transform.rotation.x.ToString()+","+RightAnkle.transform.rotation.y.ToString()+","+RightAnkle.transform.rotation.z.ToString()+","+RightAnkle.transform.rotation.w.ToString()+";");		
			}
				
			if(!DevicesLists.availableDev.Contains("KINECT:TRACKING:RIGHTANKLE:POSITION"))
			{
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTANKLE:POSITION");
				DevicesLists.availableDev.Add("KINECT:TRACKING:RIGHTANKLE:ROTATION");
			}
			//network model
			GameObject.FindGameObjectWithTag("K1RightFoot").transform.rotation = RightAnkle.transform.rotation;
		}
		
		//add wave detection to list
		if(!DevicesLists.availableDev.Contains("KINECT:BUTTON:LEFTWAVE:BOOL") || !DevicesLists.availableDev.Contains("KINECT:BUTTON:RIGHTWAVE:BOOL"))
		{
			DevicesLists.availableDev.Add("KINECT:BUTTON:LEFTWAVE:BOOL");
			DevicesLists.availableDev.Add("KINECT:BUTTON:RIGHTWAVE:BOOL");
		}
		
		//send wave detection via udp		
		if(UDPData.flag==true)
		{
			if(ZigHandSessionDetector.hand=="left")
			{
				if(DevicesLists.selectedDev.Contains("KINECT:BUTTON:LEFTWAVE:BOOL"))
				{			
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftwave,bool,"+"1"+";");
				}
				else
				{
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]leftwave,bool,"+"0"+";");
				}
			}
			else if (ZigHandSessionDetector.hand=="right")
			{
				if(DevicesLists.selectedDev.Contains("KINECT:BUTTON:RIGHTWAVE:BOOL"))
				{			
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightwave,bool,"+"1"+";");
				}
				else
				{
					UDPData.sendString("[$]button,[$$]"+DeviceName+",[$$$]rightwave,bool,"+"0"+";");
				}
			}
			
		if(addDataToList)
			{
				//add data to list
				foreach(string adev in DevicesLists.availableDev)
				{
					if(adev.Contains("KINECT") && !DevicesLists.selectedDev.Contains(adev))
					{
						DevicesLists.selectedDev.Add(adev);
					}
				}
			addDataToList=false;	
			}
			
		}

//		}//end flag if	
	}//end of send tracking
	
	
	void OnGUI () 
	{
				
//		ALPHA = GUI.HorizontalSlider(new Rect((Screen.width/2), (Screen.height/2), 100, 30), ALPHA, 0.0F, 1.0F);
//		GUI.Label(new Rect((Screen.width/2)+120, (Screen.height/2), 200, 100), ": "+ALPHA.ToString("0.00"));
//		deltaon = GUI.Toggle(new Rect((Screen.width/2), (Screen.height/2)+20, 100, 30), deltaon, "Delta");
		
//GUI.enabled = UDPData.flag;	
if(MainGuiControls.KinectMenu)
	{			
		//Skeleton Group
		GUI.BeginGroup (new Rect (Screen.width - 212, (Screen.height/2 - 200)*MainGuiControls.hideMenu, 200, 290));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,290), "Send Joint Data");
		GUI.color = Color.white;
		
		toggle1 = GUI.Toggle (new Rect (95, 25, 15, 20), toggle1, new GUIContent(" ", "Head")); //Head	
		toggle2 = GUI.Toggle (new Rect (95, 50, 15, 20), toggle2, new GUIContent(" ", "Neck")); //Neck	
		toggle3 = GUI.Toggle (new Rect (95, 90, 15, 20), toggle3, new GUIContent(" ", "Torso")); //Torso		
		toggle4 = GUI.Toggle (new Rect (95, 120, 15, 20), toggle4, new GUIContent(" ", "Waist")); //Waist
		
		toggle5 = GUI.Toggle (new Rect (80, 65, 15, 20), toggle5, new GUIContent(" ", "Left Shoulder")); //L Shoulder		
		toggle6 = GUI.Toggle (new Rect (55, 65, 15, 20), toggle6, new GUIContent(" ", "Left Elbow")); //L Elbow
		toggle7 = GUI.Toggle (new Rect (30, 65, 15, 20), toggle7, new GUIContent(" ", "Left Wrist")); //L Wrist
		
		toggle8 = GUI.Toggle (new Rect (110, 65, 15, 20), toggle8, new GUIContent(" ", "Right Shoulder")); //R Shoulder	
		toggle9 = GUI.Toggle (new Rect (135, 65, 15, 20), toggle9, new GUIContent(" ", "Right Elbow"));	//R Elbow			
		toggle10 = GUI.Toggle (new Rect (160, 65, 15, 20), toggle10, new GUIContent(" ", "Right Wrist")); //R Wrist
		
		toggle11 = GUI.Toggle (new Rect (80, 130, 15, 20), toggle11, new GUIContent(" ", "Left Hip")); //L Hip		
		toggle12 = GUI.Toggle (new Rect (80, 155, 15, 20), toggle12, new GUIContent(" ", "Left Knee")); //L Knee
		toggle13 = GUI.Toggle (new Rect (80, 180, 15, 20), toggle13, new GUIContent(" ", "Left Ankle")); //L Ankle	
		
		toggle14 = GUI.Toggle (new Rect (110, 130, 15, 20), toggle14, new GUIContent(" ", "Right Hip")); //R Hip
		toggle15 = GUI.Toggle (new Rect (110, 155, 15, 20), toggle15, new GUIContent(" ", "Right Knee")); //R Knee
		toggle16 = GUI.Toggle (new Rect (110, 180, 15, 20), toggle16, new GUIContent(" ", "Right Ankle")); //R Ankle
		
		GUI.color = Color.white;//hover text color
//		GUI.Label(new Rect(75, 220, 100, 40), GUI.tooltip);
		
		GUI.color = Color.white;	
		if(GUI.Button(new Rect(125,250,65,20), "Clear All"))
		{
			alljoints = false;
			
			toggle1 = false; //Head
			toggle2 = false;
			toggle3 = false;			
			toggle4 = false;
			
			toggle5 = false;		
			toggle6 = false;
			toggle7 = false;
			
			toggle8 = false;
			toggle9 = false;
			toggle10 = false;
			
			toggle11 = false;			
			toggle12 = false; 
			toggle13 = false;
			
			toggle14 = false;
			toggle15 = false;
			toggle16 = false;
		}
		
		GUI.color = Color.white;	
		if(GUI.Button(new Rect(10,250,65,20), "Send All"))
		{
			alljoints = true;
			
			toggle1 = true; //Head
			toggle2 = true;
			toggle3 = true;			
			toggle4 = true;
			
			toggle5 = true;		
			toggle6 = true;
			toggle7 = true;
			
			toggle8 = true;
			toggle9 = true;
			toggle10 = true;
			
			toggle11 = true;			
			toggle12 = true; 
			toggle13 = true;
			
			toggle14 = true;
			toggle15 = true;
			toggle16 = true;
		}
		
		if (GUI.Button (new Rect (85,250, 30, 30), new GUIContent(upperbody, "Upper Body"))) 
		{
			toggle1 = true; //Head
			toggle2 = true;
			toggle3 = true;	
				
			toggle4 = false;
				
			toggle5 = true;		
			toggle6 = true;
			toggle7 = true;
			
			toggle8 = true;
			toggle9 = true;
			toggle10 = true;
				
			toggle11 = false;			
			toggle12 = false; 
			toggle13 = false;
			
			toggle14 = false;
			toggle15 = false;
			toggle16 = false;				
		}
		
		GUI.Label(new Rect(75, 220, 100, 40), GUI.tooltip);
		
		GUI.EndGroup (); // end of skeleton group
		
//GUI.enabled = true;
			
		}//if kinect enabled				
		
	}

}
