using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class MultiSourceManager : MonoBehaviour {
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }
    public int IRWidth { get; private set; }
    public int IRHeight { get; private set; }
    
    private KinectSensor _Sensor;
    private MultiSourceFrameReader _Reader;
    private Texture2D _ColorTexture;
    private byte[] _ColorData;
    private ushort[] _DepthData;
    private ushort[] _IRData;
    private byte[] _IRRawData;
    private Texture2D _IRTexture;

    public Texture2D GetColorTexture()
    {
        return _ColorTexture;
    }

    public byte[] GetColorData()
    {
        return _ColorData;
    }
    
    public ushort[] GetDepthData()
    {
        return _DepthData;
    }

    public Texture2D GetInfraredTexture()
    {
        return _IRTexture;
    }

    public ushort[] GetInfraredData()
    {
        return _IRData;
    }

    public byte[] GetInfraredRawData()
    {
        return _IRRawData;
    }
    
    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared);
            
            var colorFrameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = colorFrameDesc.Width;
            ColorHeight = colorFrameDesc.Height;
            
            _ColorTexture = new Texture2D(colorFrameDesc.Width, colorFrameDesc.Height, TextureFormat.RGBA32, false);
            _ColorData = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];
            
            var depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
            _DepthData = new ushort[depthFrameDesc.LengthInPixels];

            var irFrameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            IRWidth = irFrameDesc.Width;
            IRHeight = irFrameDesc.Height;

            _IRData = new ushort[irFrameDesc.LengthInPixels];
            _IRRawData = new byte[irFrameDesc.LengthInPixels * 4];
            _IRTexture = new Texture2D(irFrameDesc.Width, irFrameDesc.Height, TextureFormat.BGRA32, false);
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }
    
    void Update () 
    {
        if (_Reader != null) 
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                var colorFrame = frame.ColorFrameReference.AcquireFrame();
                if (colorFrame != null)
                {
                    var depthFrame = frame.DepthFrameReference.AcquireFrame();
                    if (depthFrame != null)
                    {
                        var irFrame = frame.InfraredFrameReference.AcquireFrame();
                        if (irFrame != null)
                        {
                            colorFrame.CopyConvertedFrameDataToArray(_ColorData, ColorImageFormat.Rgba);
                            _ColorTexture.LoadRawTextureData(_ColorData);
                            _ColorTexture.Apply();

                            depthFrame.CopyFrameDataToArray(_DepthData);

                            irFrame.CopyFrameDataToArray(_IRData);

                            int index = 0;
                            foreach (var ir in _IRData)
                            {
                                byte intensity = (byte)(ir >> 8);
                                _IRRawData[index++] = intensity;
                                _IRRawData[index++] = intensity;
                                _IRRawData[index++] = intensity;
                                _IRRawData[index++] = 255; // Alpha
                            }

                            _IRTexture.LoadRawTextureData(_IRRawData);
                            _IRTexture.Apply();

                            irFrame.Dispose();
                            irFrame = null;
                        }

                        depthFrame.Dispose();
                        depthFrame = null;
                    }
                
                    colorFrame.Dispose();
                    colorFrame = null;
                }
                
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
