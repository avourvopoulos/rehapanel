using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class AvatarKinectPositionControl : MonoBehaviour
{

    public Kinect.JointType jointType;
    public GameObject BodySourceManager;
    public GameObject UserInterfaceManager;

    private BodySourceManager _BodyManager;
    private UserInterface _InterfaceManager;

    // Update is called once per frame
    void Update()
    {

        if (BodySourceManager == null || UserInterfaceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        _InterfaceManager = UserInterfaceManager.GetComponent<UserInterface>();
        if (_BodyManager == null || _InterfaceManager == null)
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
                Vector3 floorNormal;
                floorNormal.x = _BodyManager.floor.X;
                floorNormal.y = _BodyManager.floor.Y;
                floorNormal.z = _BodyManager.floor.Z;

                var rotFromFloortoKinect = Quaternion.FromToRotation(floorNormal, Vector3.up);
                Vector3 floorPos = new Vector3(_BodyManager.floor.X * _BodyManager.floor.W, _BodyManager.floor.Y * _BodyManager.floor.W, _BodyManager.floor.Z * _BodyManager.floor.W);
                transform.localPosition = GetVector3FromJoint(body.Joints[jointType]) + floorPos;
                transform.localPosition = rotFromFloortoKinect * transform.localPosition;
                transform.localPosition = transform.localPosition + _InterfaceManager.avatarRoot + new Vector3(0, 0.15f, 0);
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

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(-joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
}