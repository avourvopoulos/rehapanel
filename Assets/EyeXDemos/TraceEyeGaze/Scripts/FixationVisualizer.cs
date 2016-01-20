//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Framework;
using UnityEngine;

/// <summary>
/// Visualizes the fixation point in the game window using a tiny GUI.Box.
/// </summary>
public class FixationVisualizer : PointVisualizerBase
{
    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private IEyeXDataProvider<EyeXFixationPoint> _fixationDataProvider;
    private int _fixationCount;

#if UNITY_EDITOR
    private FixationDataMode _oldFixationDataMode;
#endif

    /// <summary>
    /// Choice of fixation data stream to be visualized.
    /// </summary>
    public FixationDataMode fixationDataMode = FixationDataMode.Sensitive;

    /// <summary>
    /// The size of the visualizer point
    /// </summary>
    public float pointSize = 25;

    /// <summary>
    /// The color of the visualizer point
    /// </summary>
    public Color pointColor = Color.yellow;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _fixationDataProvider = _eyeXHost.GetFixationDataProvider(fixationDataMode);

#if UNITY_EDITOR
        _oldFixationDataMode = fixationDataMode;
#endif
    }

    public void OnEnable()
    {
        _fixationDataProvider.Start();
    }

    public void OnDisable()
    {
        _fixationDataProvider.Stop();
    }

    /// <summary>
    /// Draw a GUI.Box at the user's fixation point.
    /// </summary>
    public void OnGUI()
    {
#if UNITY_EDITOR
        if (_oldFixationDataMode != fixationDataMode)
        {
            _fixationDataProvider.Stop();
            _oldFixationDataMode = fixationDataMode;
            _fixationDataProvider = _eyeXHost.GetFixationDataProvider(fixationDataMode);
            _fixationDataProvider.Start();
        }
#endif

        var fixationPoint = _fixationDataProvider.Last;
        if (fixationPoint.IsValid)
        {
            if (FixationDataEventType.Begin == fixationPoint.EventType)
            {
                _fixationCount++;
            }
			if (MainGuiControls.TobiiEyeXMenu) 
			{
            DrawGUI(fixationPoint.GazePoint, pointSize, pointColor, _fixationCount.ToString());
			}
        }
    }
}
