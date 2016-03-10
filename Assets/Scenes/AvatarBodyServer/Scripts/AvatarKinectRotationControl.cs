using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
//using KinectFace = Microsoft.Kinect.Face;

public class AvatarKinectRotationControl : MonoBehaviour
{
    public enum axis : int
    {
        kinectJointAxisX = 0,
        kinectJointAxisY = 1,
        kinectJointAxisZ = 2
    }

    private float euler1;
    private float euler2;
    private float euler3;

    public float euler1Alpha;
    public float euler2Alpha;
    public float euler3Alpha;

    public float euler1OffSet;
    public float euler2OffSet;
    public float euler3OffSet;

    public axis unityAxisX;
    public axis unityAxisY;
    public axis unityAxisZ;

    public Kinect.JointType jointType;
    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    [Range(0, 5)]
    public int BodyIndex;

    // Use this for initialization
    void Start()
    {
        euler1 = 0f;
        euler2 = 0f;
        euler3 = 0f;
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

        var body = data[BodyIndex];
        //List<ulong> trackedIds = new List<ulong>();
        //foreach (var body in data)
        //{
        //    if (body == null)
        //    {
        //        continue;
        //    }

        //    if (body.IsTracked)
        //    {
        //        trackedIds.Add(body.TrackingId);
        //    }
        //}

        if (body != null)
        {
            if (body.IsTracked)
            {
                Kinect.JointOrientation orientation = body.JointOrientations[jointType];
                Vector4 axisAngle = GetVector4FromJoint(orientation);
                Quaternion quat = new Quaternion(axisAngle.x, axisAngle.y, axisAngle.z, axisAngle.w);

                switch (unityAxisX)
                {
                    case axis.kinectJointAxisX:
                        euler1 = quat.eulerAngles.x;
                        break;
                    case axis.kinectJointAxisY:
                        euler1 = quat.eulerAngles.y;
                        break;
                    case axis.kinectJointAxisZ:
                        euler1 = quat.eulerAngles.z;
                        break;
                }
                switch (unityAxisY)
                {
                    case axis.kinectJointAxisX:
                        euler2 = quat.eulerAngles.x;
                        break;
                    case axis.kinectJointAxisY:
                        euler2 = quat.eulerAngles.y;
                        break;
                    case axis.kinectJointAxisZ:
                        euler2 = quat.eulerAngles.z;
                        break;
                }

                switch (unityAxisZ)
                {
                    case axis.kinectJointAxisX:
                        euler3 = quat.eulerAngles.x;
                        break;
                    case axis.kinectJointAxisY:
                        euler3 = quat.eulerAngles.y;
                        break;
                    case axis.kinectJointAxisZ:
                        euler3 = quat.eulerAngles.z;
                        break;
                }

                euler1 = LimitAngleDomain(euler1Alpha * (euler1 + euler1OffSet));
                euler2 = LimitAngleDomain(euler2Alpha * (euler2 + euler2OffSet));
                euler3 = LimitAngleDomain(euler3Alpha * (euler3 + euler3OffSet));

                Quaternion rot1 = Quaternion.Euler(euler1, euler2, euler3);
                Vector3 floorNormal;
                floorNormal.x = _BodyManager.Floor.X;
                floorNormal.y = _BodyManager.Floor.Y;
                floorNormal.z = _BodyManager.Floor.Z;

                var rotFromKinectoFloor = Quaternion.FromToRotation(Vector3.up, floorNormal);

                Quaternion rot = rotFromKinectoFloor * rot1;

                transform.rotation = rot;
                transform.Rotate(Vector3.up * 180, Space.World);
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
