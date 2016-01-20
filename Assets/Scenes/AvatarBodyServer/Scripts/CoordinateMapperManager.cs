using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class CoordinateMapperManager : MonoBehaviour
{
    private KinectSensor m_pKinectSensor;
    private CoordinateMapper m_pCoordinateMapper;

    public CameraSpacePoint[] m_pCameraCoordinates { get; private set; }

    public GameObject MultiSourceManager;
    private MultiSourceManager _MultiManager;

    // Use this for initialization
    void Start()
    {
        m_pKinectSensor = KinectSensor.GetDefault();

        if (m_pKinectSensor != null)
        {
            if (!m_pKinectSensor.IsOpen)
            {
                m_pKinectSensor.Open();
            }

            var depthFrameDesc = m_pKinectSensor.DepthFrameSource.FrameDescription;
            m_pCameraCoordinates = new CameraSpacePoint[depthFrameDesc.Width * depthFrameDesc.Height];
            m_pCoordinateMapper = m_pKinectSensor.CoordinateMapper;
        }
        else
        {
            UnityEngine.Debug.LogError("No ready Kinect found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MultiSourceManager == null)
        {
            return;
        }

        _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
        if (_MultiManager == null)
        {
            return;
        }

        ushort[] depthData = _MultiManager.GetDepthData();
        if (depthData != null)
        {
            m_pCoordinateMapper.MapDepthFrameToCameraSpace(depthData, m_pCameraCoordinates);

        }
    
    }
}
