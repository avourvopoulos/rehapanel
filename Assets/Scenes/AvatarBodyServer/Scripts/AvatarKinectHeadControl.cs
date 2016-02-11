using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class AvatarKinectHeadControl : MonoBehaviour
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

    public GameObject FaceSourceManager;
    private FaceSourceManager _FaceManager;

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

        var face = dataFace[BodyIndex];
        if (face != null)
        {
            Quaternion quat = new Quaternion(face.FaceRotationQuaternion.X, face.FaceRotationQuaternion.Y, face.FaceRotationQuaternion.Z, face.FaceRotationQuaternion.W);

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

            transform.eulerAngles = new Vector3(euler1, euler2, euler3);
            transform.Rotate(Vector3.up * 180, Space.World);

            Vector3 floorNormal;
            floorNormal.x = _BodyManager.Floor.X;
            floorNormal.y = _BodyManager.Floor.Y;
            floorNormal.z = _BodyManager.Floor.Z;

            var rotFromKinectoFloor = Quaternion.FromToRotation(Vector3.up, floorNormal);
            transform.rotation = transform.rotation * rotFromKinectoFloor;
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
}
