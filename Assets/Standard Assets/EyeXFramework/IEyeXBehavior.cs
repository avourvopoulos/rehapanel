//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.EyeX.Client;

/// <summary>
/// Interface of EyeX behavior classes. Used by the EyeXHost.
/// </summary>
public interface IEyeXBehavior
{
    /// <summary>
    /// Assigns the behavior to an interactor object.
    /// </summary>
    /// <param name="interactor">The interactor object.</param>
    void AssignBehavior(Interactor interactor);

    /// <summary>
    /// Handles an event addressed to the interactor.
    /// </summary>
    /// <param name="interactorId">The ID of the interactor targeted by the event.</param>
    /// <param name="behaviors">The <see cref="Behavior"/> instances containing the event data.</param>
    void HandleEvent(string interactorId, IEnumerable<Behavior> behaviors);
}
