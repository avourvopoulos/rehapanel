//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;
using System.Collections.Generic;

/// <summary>
/// Provider of eye position data. When the provider has been started it
/// will continuously update the Last property with the latest gaze point 
/// value received from the EyeX Engine.
/// </summary>
public class EyeXEyePositionDataStream : EyeXDataStreamBase<EyeXEyePosition>
{
    /// <summary>
    /// Creates a new instance.
    /// Note: don't create instances of this class directly. Use the <see cref="EyeXHost.GetEyePositionDataProvider"/> method instead.
    /// </summary>
    public EyeXEyePositionDataStream()
    {
        Last = EyeXEyePosition.Invalid;
    }

    public override string Id
    {
        get { return "EyeXEyePositionDataStream"; }
    }

    protected override void AssignBehavior(Interactor interactor)
    {
        var behavior = interactor.CreateBehavior(BehaviorType.EyePositionData);
        behavior.Dispose();
    }

    protected override void HandleEvent(IEnumerable<Behavior> eventBehaviors, Vector2 viewportPosition, Vector2 viewportPixelsPerDesktopPixel)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        // The data is stored in the Last property and used from the main thread.
        foreach (var behavior in eventBehaviors)
        {
            EyePositionDataEventParams eventParams;
            if (behavior.TryGetEyePositionDataEventParams(out eventParams))
            {
                var left = new EyeXSingleEyePosition(eventParams.HasLeftEyePosition != EyeXBoolean.False, (float)eventParams.LeftEyeX, (float)eventParams.LeftEyeY, (float)eventParams.LeftEyeZ);
                var leftNormalized = new EyeXSingleEyePosition(eventParams.HasLeftEyePosition != EyeXBoolean.False, (float)eventParams.LeftEyeXNormalized, (float)eventParams.LeftEyeYNormalized, (float)eventParams.LeftEyeZNormalized);
                
                var right = new EyeXSingleEyePosition(eventParams.HasRightEyePosition != EyeXBoolean.False, (float)eventParams.RightEyeX, (float)eventParams.RightEyeY, (float)eventParams.RightEyeZ);
                var rightNormalized = new EyeXSingleEyePosition(eventParams.HasRightEyePosition != EyeXBoolean.False, (float)eventParams.RightEyeXNormalized, (float)eventParams.RightEyeYNormalized, (float)eventParams.RightEyeZNormalized);

                Last = new EyeXEyePosition(left, leftNormalized, right, rightNormalized, eventParams.Timestamp);
            }
        }
    }

    protected override void OnStreamingStarted()
    {
        Last = EyeXEyePosition.Invalid;
    }
}
