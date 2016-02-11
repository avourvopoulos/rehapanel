using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class AvatarKinectHandControl : MonoBehaviour
{
    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    public GameObject AvatarCarl;

    [Range(0, 5)]
    public int BodyIndex;

    // Use this for initialization
    void Start()
    {

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

        var body = data[BodyIndex];
        if (body != null)
        {
            if (body.IsTracked)
            {
                var handState = body.HandLeftState;
                var hand = "Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand";
                switch (handState)
                {
                    case Kinect.HandState.Unknown:
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2/LeftHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2/LeftHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2/LeftHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2/LeftHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.NotTracked:
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2/LeftHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2/LeftHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2/LeftHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2/LeftHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.Open:
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2/LeftHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2/LeftHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2/LeftHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2/LeftHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.Closed:
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2/LeftHandMiddle3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2/LeftHandPinky3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandRing1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2/LeftHandRing3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1").localEulerAngles = new Vector3(20, -5, -10);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2").localEulerAngles = new Vector3(0, -20, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2/LeftHandThumb3").localEulerAngles = new Vector3(0, -50, 0);
                        break;
                    case Kinect.HandState.Lasso:
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandMiddle1/LeftHandMiddle2/LeftHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandPinky1/LeftHandPinky2/LeftHandPinky3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandRing1").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2").localEulerAngles = new Vector3(0, 0, 90);
                        AvatarCarl.transform.Find(hand + "/LeftHandRing1/LeftHandRing2/LeftHandRing3").localEulerAngles = new Vector3(0, 0, 90);

                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1").localEulerAngles = new Vector3(20, -5, -10);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2").localEulerAngles = new Vector3(0, -20, 0);
                        AvatarCarl.transform.Find(hand + "/LeftHandThumb1/LeftHandThumb2/LeftHandThumb3").localEulerAngles = new Vector3(0, -50, 0);
                        break;
                }

                handState = body.HandRightState;
                hand = "Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand";
                switch (handState)
                {
                    case Kinect.HandState.Unknown:
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2/RightHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2/RightHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2/RightHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2/RightHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2/RightHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.NotTracked:
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2/RightHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2/RightHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2/RightHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2/RightHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        //AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2/RightHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.Open:
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2/RightHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2/RightHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandPinky1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2/RightHandPinky3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandRing1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2/RightHandRing3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandThumb1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2/RightHandThumb3").localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case Kinect.HandState.Closed:
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2/RightHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2/RightHandMiddle3").localEulerAngles = new Vector3(0, 0, -90);

                        AvatarCarl.transform.Find(hand + "/RightHandPinky1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2/RightHandPinky3").localEulerAngles = new Vector3(0, 0, -90);

                        AvatarCarl.transform.Find(hand + "/RightHandRing1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2/RightHandRing3").localEulerAngles = new Vector3(0, 0, -90);

                        AvatarCarl.transform.Find(hand + "/RightHandThumb1").localEulerAngles = new Vector3(20, 5, 10);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2").localEulerAngles = new Vector3(0, 20, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2/RightHandThumb3").localEulerAngles = new Vector3(0, 50, 0);
                        break;
                    case Kinect.HandState.Lasso:
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandIndex1/RightHandIndex2/RightHandIndex3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2").localEulerAngles = new Vector3(0, 0, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandMiddle1/RightHandMiddle2/RightHandMiddle3").localEulerAngles = new Vector3(0, 0, 0);

                        AvatarCarl.transform.Find(hand + "/RightHandPinky1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandPinky1/RightHandPinky2/RightHandPinky3").localEulerAngles = new Vector3(0, 0, -90);

                        AvatarCarl.transform.Find(hand + "/RightHandRing1").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2").localEulerAngles = new Vector3(0, 0, -90);
                        AvatarCarl.transform.Find(hand + "/RightHandRing1/RightHandRing2/RightHandRing3").localEulerAngles = new Vector3(0, 0, -90);

                        AvatarCarl.transform.Find(hand + "/RightHandThumb1").localEulerAngles = new Vector3(20, 5, 10);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2").localEulerAngles = new Vector3(0, 20, 0);
                        AvatarCarl.transform.Find(hand + "/RightHandThumb1/RightHandThumb2/RightHandThumb3").localEulerAngles = new Vector3(0, 50, 0);
                        break;
                }
            }
        }
    }
}
