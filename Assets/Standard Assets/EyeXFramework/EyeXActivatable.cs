//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Used for assigning the Activatable behavior to an interactor, making it respond to activation events, "gaze clicking".
/// See <see cref="EyeXInteractor.EyeXBehaviors"/>.
/// </summary>
public class EyeXActivatable : IEyeXBehavior
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="activationHub">Activation hub used for synchronizing activation events across interactors and frames. See <see cref="EyeXHost.ActivationHub>."/></param>
    public EyeXActivatable(IEyeXActivationHub activationHub)
    {
        ActivationHub = activationHub;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the interactor wants to receive tentative activation focus events.
    /// </summary>
    public bool IsTentativeFocusEnabled { get; set; }

    /// <summary>
    /// Gets the activation hub.
    /// </summary>
    public IEyeXActivationHub ActivationHub { get; private set; }

    #region IEyeXBehavior interface

    public void AssignBehavior(Interactor interactor)
    {
        var behaviorParams = new ActivatableParams { EnableTentativeFocus = IsTentativeFocusEnabled ? EyeXBoolean.True : EyeXBoolean.False };
        interactor.CreateActivatableBehavior(ref behaviorParams);
    }

    public void HandleEvent(string interactorId, IEnumerable<Behavior> behaviors)
    {
        ActivationHub.HandleEvent(interactorId, behaviors);
    }

    #endregion
}

public static class EyeXActivatableInteractorExtensions
{
    /// <summary>
    /// Gets a value indicating whether the specified interactor has been activated.
    /// Note that only interactors which have been assigned the Activatable behavior can be activated.
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    /// <returns>True if the interactor has been activated.</returns>
    public static bool IsActivated(this EyeXInteractor interactor)
    {
        var behavior = GetActivatableBehavior(interactor);
        if (behavior == null) { return false; }
        return behavior.ActivationHub.GetIsActivated(interactor.Id);
    }

    /// <summary>
    /// Gets a value indicating the activation focus state of the specified interactor.
    /// Note that only interactors which have been assigned the Activatable behavior have an activation focus state.
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    /// <returns>The activation focus state.</returns>
    public static ActivationFocusState GetActivationFocusState(this EyeXInteractor interactor)
    {
        var behavior = GetActivatableBehavior(interactor);
        if (behavior == null) { return ActivationFocusState.None; }
        return behavior.ActivationHub.GetActivationFocusState(interactor.Id);
    }

    /// <summary>
    /// Gets the EyeXActivatable behavior assigned to the interactor.
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    /// <returns>The behavior. Null if no matching EyeX behavior has been assigned to the interactor.</returns>
    public static EyeXActivatable GetActivatableBehavior(EyeXInteractor interactor)
    {
        foreach (var behavior in interactor.EyeXBehaviors)
        {
            var activatableBehavior = behavior as EyeXActivatable;
            if (activatableBehavior != null)
            {
                return activatableBehavior;
            }
        }

        return null;
    }
}
