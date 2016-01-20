using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.Face;

public class FaceSourceManager : MonoBehaviour
{
    private KinectSensor kinectSensor;
    private int bodyCount;
    private Body[] bodies;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private FaceFrameResult[] result;

    public GameObject bodyManager;

    public FaceFrameResult[] GetData()
    {
        return result;
    }

    void Start()
    {
        // one sensor is currently supported
        kinectSensor = KinectSensor.GetDefault();

        // set the maximum number of bodies that would be tracked by Kinect
        bodyCount = kinectSensor.BodyFrameSource.BodyCount;

        // allocate storage to store body objects
        bodies = new Body[bodyCount];

        // specify the required face frame results
        FaceFrameFeatures faceFrameFeatures =
            FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.BoundingBoxInInfraredSpace
                | FaceFrameFeatures.PointsInInfraredSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.MouthOpen;

        // create a face frame source + reader to track each face in the FOV
        faceFrameSources = new FaceFrameSource[bodyCount];
        faceFrameReaders = new FaceFrameReader[bodyCount];
        result = new FaceFrameResult[bodyCount];
        for (int i = 0; i < bodyCount; i++)
        {
            // create the face frame source with the required face frame features and an initial tracking Id of 0
            faceFrameSources[i] = FaceFrameSource.Create(kinectSensor, 0, faceFrameFeatures);

            // open the corresponding reader
            faceFrameReaders[i] = faceFrameSources[i].OpenReader();
        }
    }

    void Update()
    {
        if (bodyManager == null)
        {
            return;
        }

        // get bodies either from BodySourceManager or object get them from a BodyReader
        var bodySourceManager = bodyManager.GetComponent<BodySourceManager>();
        if (bodySourceManager == null)
        {
            return;
        }
        bodies = bodySourceManager.GetData();
        if (bodies == null)
        {
            return;
        }

        for (int i = 0; i < bodyCount; i++)
        {
            if (bodies[i].IsTracked)
            {
                if (bodies[i].TrackingId != faceFrameSources[i].TrackingId)
                {
                    faceFrameSources[i].TrackingId = bodies[i].TrackingId;
                }

                using (FaceFrame frame = faceFrameReaders[i].AcquireLatestFrame())
                {
                    if (frame != null)
                    {
                        result[i] = frame.FaceFrameResult;
                    }
                }

            }
            else
            {
                faceFrameSources[i].TrackingId = 0;
                result[i] = null;
            }
        }

        //// iterate through each body and update face source
        //for (int i = 0; i < bodyCount; i++)
        //{
        //    // check if a valid face is tracked in this face source				
        //    if (faceFrameSources[i].IsTrackingIdValid)
        //    {
        //        using (FaceFrame frame = faceFrameReaders[i].AcquireLatestFrame())
        //        {
        //            if (frame != null)
        //            {
        //                if (frame.TrackingId == 0)
        //                {
        //                    continue;
        //                }

        //                // do something with result
        //                result[i] = frame.FaceFrameResult;
        //                //print(result.FaceRotationQuaternion.X);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // check if the corresponding body is tracked 
        //        if (bodies[i].IsTracked)
        //        {
        //            // update the face frame source to track this body
        //            faceFrameSources[i].TrackingId = bodies[i].TrackingId;
        //        }
        //    }
        //}
    }
}
