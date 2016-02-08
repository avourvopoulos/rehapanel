using System;
using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class KinectCalibration : MonoBehaviour {

    public BodySourceManager BodySource;
    private Vector3 _floorNormal;
    private float KinectY;
    private float KinectPitch;
    private float KinectRoll;

	void Start () {
        if (BodySource != null)
        {
            KinectY = BodySource.Floor.W;

            _floorNormal.x = -BodySource.Floor.X;
            _floorNormal.y = BodySource.Floor.Y;
            _floorNormal.z = BodySource.Floor.Z;

            var rotFromFloortoKinect = Quaternion.FromToRotation(_floorNormal, Vector3.up);

            Vector3 ang = rotFromFloortoKinect.eulerAngles;

            KinectPitch = ang.y;
            KinectRoll = ang.z;
        }
	}
	
	void Update ()
	{
        if (BodySource != null)
        {
            KinectY = BodySource.Floor.W;

            _floorNormal.x = -BodySource.Floor.X;
            _floorNormal.y = BodySource.Floor.Y;
            _floorNormal.z = BodySource.Floor.Z;

            var rotFromFloortoKinect = Quaternion.FromToRotation(_floorNormal, Vector3.up);

            Vector3 ang = rotFromFloortoKinect.eulerAngles;

            KinectPitch = ang.x;
            KinectRoll = ang.z;
        }

        UpdateY(KinectY);
	    UpdatePitch(KinectPitch);
	    UpdateRoll(KinectRoll);
	}

    public void UpdateY(float y)
    {
        var position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public void UpdatePitch(float pitch)
    {
        var orientation = transform.rotation.eulerAngles;
        orientation.x = pitch;
        transform.rotation = Quaternion.Euler(orientation);
    }

    public void UpdateRoll(float roll)
    {
        var orientation = transform.rotation.eulerAngles;
        orientation.z = roll;
        transform.rotation = Quaternion.Euler(orientation);
    }
}
