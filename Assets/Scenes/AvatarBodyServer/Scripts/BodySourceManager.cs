using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour
{
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    public Windows.Kinect.Vector4 Floor { get; private set; }

    public Body[] GetData()
    {
        return _Data;
    }


    //TODO: implement an on/off button for the sensor

    void Start()
    {
        if (SystemInfo.operatingSystem.ToString ().Contains ("Windows 8"))
            _Sensor = KinectSensor.GetDefault();

        //if (_Sensor != null)
        //{
        //    _Reader = _Sensor.BodyFrameSource.OpenReader();

        //    if (!_Sensor.IsOpen)
        //    {
        //        _Sensor.Open();
        //    }
        //}
    }

    void Update()
    {
        //Start the Kinect2 if the toggle is on and the device is off
        if (UserInterface.Kinect2On)
        {
            if (_Sensor != null)
            {
                if (_Reader == null)
                    _Reader = _Sensor.BodyFrameSource.OpenReader();

                if (!_Sensor.IsOpen)
                {
                    _Sensor.Open();
                }
            }
        }
        //Turns the Kinect2 off if the toggle is set to off and the device is on
        else if (!UserInterface.Kinect2On)
        {
            if (_Reader != null)
            {
                _Reader.Dispose();
                _Reader = null;
            }

            if (_Sensor != null)
            {
                if (_Sensor.IsOpen)
                {
                    _Sensor.Close();
                }
            }
        }


        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(_Data);
                Floor = frame.FloorClipPlane;

                frame.Dispose();
                frame = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
