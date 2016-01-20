//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;
using System.Collections.Generic;

/// <summary>
/// Provider of gaze point data. When the provider has been started it
/// will continuously update the Last property with the latest gaze point 
/// value received from the EyeX Engine.
/// </summary>
public class EyeXGazePointDataStream : EyeXDataStreamBase<EyeXGazePoint>
{
    private readonly GazePointDataMode _mode;

    /// <summary>
    /// Creates a new instance.
    /// Note: don't create instances of this class directly. Use the <see cref="EyeXHost.GetGazePointDataProvider"/> method instead.
    /// </summary>
    /// <param name="mode">Data mode.</param>
    public EyeXGazePointDataStream(GazePointDataMode mode)
    {
        _mode = mode;
        Last = EyeXGazePoint.Invalid;
    }

    public override string Id
    {
        get { return string.Format("EyeXGazePointDataStream/{0}", _mode); }
    }

    protected override void AssignBehavior(Interactor interactor)
    {
        var behaviorParams = new GazePointDataParams() { GazePointDataMode = _mode };
        interactor.CreateGazePointDataBehavior(ref behaviorParams);
    }

    protected override void HandleEvent(IEnumerable<Behavior> eventBehaviors, Vector2 viewportPosition, Vector2 viewportPixelsPerDesktopPixel)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        // The data is stored in the Last property and used from the main thread.
        foreach (var behavior in eventBehaviors)
        {
            GazePointDataEventParams eventParams;
            if (behavior.TryGetGazePointDataEventParams(out eventParams))
            {
                Last = new EyeXGazePoint(
                    new Vector2((float)eventParams.X, (float)eventParams.Y), 
                    eventParams.Timestamp,
                    viewportPosition,
                    viewportPixelsPerDesktopPixel);
            }
        }
    }

    protected override void OnStreamingStarted()
    {
        Last = EyeXGazePoint.Invalid;
    }
}
