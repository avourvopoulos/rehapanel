using UnityEngine;
using System.Collections;
using Tobii.EyeX.Framework;

public class FollowGazePoint : MonoBehaviour
{
    // Scale: 1 mm maps to 0.001 units in world space
    private const float Scale = 1 / 1000.0f;

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private IEyeXDataProvider<EyeXGazePoint> _gazePointProvider;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _gazePointProvider = _eyeXHost.GetGazePointDataProvider(GazePointDataMode.LightlyFiltered);
    }

    void Start()
    {
        _gazePointProvider.Start();
    }

    void Update()
    {
        var displaySize = _eyeXHost.DisplaySize;
        var screenBounds = _eyeXHost.ScreenBounds;
        var gazePoint = _gazePointProvider.Last;

        var gazePointOnDisplayX = gazePoint.Display.x;
        var gazePointOnDisplayY = gazePoint.Display.y;

        if (displaySize.IsValid &&
            screenBounds.IsValid && screenBounds.Value.Width > 0 && screenBounds.Value.Height > 0 &&
            gazePoint.IsValid)
        {
            var normalizedGazePoint = new Vector2(
                (float)((gazePointOnDisplayX - screenBounds.Value.X) / screenBounds.Value.Width),
                (float)((gazePointOnDisplayY - screenBounds.Value.Y) / screenBounds.Value.Height));

            var gazePointOnDisplayPlaneMm = new Vector2(
                (float)((0.5 - normalizedGazePoint.x) * displaySize.Value.Width),
                (float)((0.5 - normalizedGazePoint.y) * displaySize.Value.Height));

            renderer.transform.position = new Vector3(
                gazePointOnDisplayPlaneMm.x * Scale,
                gazePointOnDisplayPlaneMm.y * Scale,
                0);

            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }
}
