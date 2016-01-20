//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Framework;
using UnityEngine;

/// <summary>
/// Visualizes the gaze point in the game window using a tiny GUI.Box.
/// </summary>
public class GazePointVisualizer : PointVisualizerBase
{
    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private IEyeXDataProvider<EyeXGazePoint> _gazePointProvider;

#if UNITY_EDITOR
    private GazePointDataMode _oldGazePointMode;
#endif

    /// <summary>
    /// Choice of gaze point data stream to be visualized.
    /// </summary>
    public GazePointDataMode gazePointMode = GazePointDataMode.LightlyFiltered;

    /// <summary>
    /// The size of the visualizer point
    /// </summary>
    public float pointSize = 5;

    /// <summary>
    /// The color of the visualizer point
    /// </summary>
    public Color pointColor = Color.yellow;

	public static Vector2 gazeDisplay;
	public static Vector2 gazeGUI;
	public static Vector2 gazeScreen;
	public static Vector2 gazeViewport;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _gazePointProvider = _eyeXHost.GetGazePointDataProvider(gazePointMode);

#if UNITY_EDITOR
        _oldGazePointMode = gazePointMode;
#endif
    }

    public void OnEnable()
    {
        _gazePointProvider.Start();
    }

    public void OnDisable()
    {
        _gazePointProvider.Stop();
    }

    /// <summary>
    /// Draw a GUI.Box at the user's gaze point.
    /// </summary>
    public void OnGUI()
    {
#if UNITY_EDITOR
        if (_oldGazePointMode != gazePointMode)
        {
            _gazePointProvider.Stop();
            _oldGazePointMode = gazePointMode;
            _gazePointProvider = _eyeXHost.GetGazePointDataProvider(gazePointMode);
            _gazePointProvider.Start();
        }
#endif

        var gazePoint = _gazePointProvider.Last;

		if (MainGuiControls.TobiiEyeXMenu) 
		{
			DrawGUI (gazePoint, pointSize, pointColor, string.Empty);
		}

	//	Debug.Log ("gazePoint: "+gazePoint);

		gazeDisplay = gazePoint.Display;
		gazeGUI = gazePoint.GUI;
		gazeScreen = gazePoint.Screen;
		gazeViewport = gazePoint.Viewport;

    }

}
