using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Microsoft.Kinect;
using KinectFace = Microsoft.Kinect.Face;

public class AvatarKinectRotationControl2 : MonoBehaviour
{
    //public enum axis : int
    //{
    //    kinectJointAxisX = 0,
    //    kinectJointAxisY = 1,
    //    kinectJointAxisZ = 2
    //}

    //private float euler1;
    //private float euler2;
    //private float euler3;

    //public float euler1Alpha;
    //public float euler2Alpha;
    //public float euler3Alpha;

    //public float euler1OffSet;
    //public float euler2OffSet;
    //public float euler3OffSet;

    //public axis unityAxisX;
    //public axis unityAxisY;
    //public axis unityAxisZ;

    public Kinect.JointType jointType;
    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    // Use this for initialization
    void Start()
    {
        //euler1 = 0f;
        //euler2 = 0f;
        //euler3 = 0f;
    }

    // Update is called once per frame
    void Update()
    {
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

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                Kinect.JointOrientation orientation = body.JointOrientations[jointType];
                Vector4 axisAngle = GetVector4FromJoint(orientation);
                Quaternion quat = new Quaternion(-axisAngle.x, axisAngle.y, axisAngle.z, -axisAngle.w);

                float euler1 = LimitAngleDomain(quat.eulerAngles.x);
                float euler2 = LimitAngleDomain(quat.eulerAngles.y);
                float euler3 = LimitAngleDomain(quat.eulerAngles.z);
                transform.eulerAngles = new Vector3(euler1, euler2, euler3);

                //// Reset the bone rotation
                //transform.rotation = Quaternion.identity;

                //// Rotate the bone in unity to match the bone in kinect
                //transform.Rotate(new Vector3(0, 90, -90), Space.World);

                //// Rotate the bone in Kinect space
                //Kinect.JointOrientation orientation = body.JointOrientations[jointType];
                //Vector4 axisAngle = GetVector4FromJoint(orientation);
                //Quaternion KinectQuat = new Quaternion(axisAngle.x, axisAngle.y, axisAngle.z, axisAngle.w);
                //euler1 = 0;//KinectQuat.eulerAngles.z;
                //euler2 = 0;//KinectQuat.eulerAngles.x;
                //euler3 = KinectQuat.eulerAngles.y;
                //transform.Rotate(new Vector3(euler1, euler2, euler3), Space.World);

                //// Rotate the reference frame back from kinect to unity
                //transform.Rotate(new Vector3(90, 0, 90), Space.World);

                //// mirror rotation? due to left hand vs right hand
            }
        }
    }

    private float LimitAngleDomain(float angle)
    {
        if (angle > 360)
            angle -= 360;

        if (angle < 0)
            angle += 360;

        if (angle > 360 || angle < 0)
            LimitAngleDomain(angle);

        return angle;
    }

    private static Vector4 GetVector4FromJoint(Kinect.JointOrientation joint)
    {
        return new Vector4(joint.Orientation.X, joint.Orientation.Y, joint.Orientation.Z, joint.Orientation.W);
    }
}

