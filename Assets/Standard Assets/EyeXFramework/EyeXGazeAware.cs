//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Used for assigning the GazeAware behavior to an interactor, so that it can sense when it has the user's gaze.
/// See <see cref="EyeXInteractor.EyeXBehaviors"/>.
/// </summary>
public class EyeXGazeAware : IEyeXBehavior
{
    /// <summary>
    /// Gets or sets the time (in milliseconds) that the user has to look at the interactor before a gaze aware event is triggered.
    /// </summary>
    public int DelayTime { get; set; }

    /// <summary>
    /// Gets a value indicating whether the user's gaze is within the bounds of the interactor.
    /// </summary>
    public bool HasGaze { get; private set; }

    #region IEyeXBehavior interface

    public void AssignBehavior(Interactor interactor)
    {
        using (var behavior = interactor.CreateBehavior(BehaviorType.GazeAware))
        {
            if (DelayTime > 0)
            {
                var gazeAwareParams = new GazeAwareParams() { GazeAwareMode = GazeAwareMode.Delayed, DelayTime = DelayTime };
                behavior.SetGazeAwareParams(ref gazeAwareParams);
            }
        }
    }

    public void HandleEvent(string interactorId, IEnumerable<Behavior> behaviors)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior.BehaviorType != BehaviorType.GazeAware) { continue; }

            GazeAwareEventParams eventData;
            if (behavior.TryGetGazeAwareEventParams(out eventData))
            {
                HasGaze = eventData.HasGaze != EyeXBoolean.False;
            }
        }
    }

    #endregion
}

public static class EyeXGazeAwareInteractorExtensions
{
    /// <summary>
    /// Gets a value indicating whether the specified interactor has received the user's gaze.
    /// Note that only interactors which have been assigned the GazeAware behavior will receive gaze events.
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    /// <returns>True if the user's gaze is within the bounds of the interactor.</returns>
    public static bool HasGaze(this EyeXInteractor interactor)
    {
        var behavior = GetGazeAwareBehavior(interactor);
        if (behavior == null) { return false; }
        return behavior.HasGaze;
    }

    /// <summary>
    /// Gets the EyeXGazeAware behavior assigned to the interactor.
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    /// <returns>The behavior. Null if no matching EyeX behavior has been assigned to the interactor.</returns>
    public static EyeXGazeAware GetGazeAwareBehavior(EyeXInteractor interactor)
    {
        foreach (var behavior in interactor.EyeXBehaviors)
        {
            var gazeAwareBehavior = behavior as EyeXGazeAware;
            if (gazeAwareBehavior != null)
            {
                return gazeAwareBehavior;
            }
        }

        return null;
    }
}