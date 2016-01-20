//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

public enum ActivationFocusState
{
    None,
    HasActivationFocus,
    HasTentativeActivationFocus
}

public interface IEyeXActivationHub
{
    /// <summary>
    /// Handles an event. The event will take effect immediately, if no read operations have been performed yet in the current frame,
    /// or it will take effect at the end of the frame.
    /// </summary>
    /// <param name="interactorId">The ID of the interactor targeted by the event.</param>
    /// <param name="behaviors">The <see cref="Behavior"/> instances containing the event data.</param>
    void HandleEvent(string interactorId, IEnumerable<Behavior> behaviors);

    /// <summary>
    /// Signals that the frame ends.
    /// </summary>
    void EndFrame();

    /// <summary>
    /// Gets a value indicating whether the specified interactor has received an activation event.
    /// </summary>
    /// <param name="interactorId">ID of the interactor.</param>
    /// <returns>True if the interactor has been activated.</returns>
    bool GetIsActivated(string interactorId);

    /// <summary>
    /// Gets a value indicating the activation focus state of the specified interactor.
    /// </summary>
    /// <param name="interactorId">ID of the interactor.</param>
    /// <returns>The activation focus state.</returns>
    ActivationFocusState GetActivationFocusState(string interactorId);
}

/// <summary>
/// Aggregates activation-related events from the EyeX Engine so that they appear consistently within rendering frames.
/// That is, keeps track of the activation state at any time and updates the state based on the events that it receives.
/// All read operations within the same frame return the same result, regardless of the timing of incoming events.
/// </summary>
public class EyeXActivationHub : IEyeXActivationHub
{
    private readonly List<Action> _cachedActions = new List<Action>();
    private bool _isFrozenUntilEndOfFrame;

    private string _activatedInteractor;
    private string _tentativelyFocusedInteractor;
    private string _focusedInteractor;

    public void HandleEvent(string interactorId, IEnumerable<Behavior> behaviors)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        lock (this)
        {
            foreach (var behavior in behaviors)
            {
                ActivatableEventType eventType;
                if (!behavior.TryGetActivatableEventType(out eventType)) { continue; }

                if (eventType == ActivatableEventType.Activated)
                {
                    _cachedActions.Add(new Action(() =>
                        {
                            if (_activatedInteractor == null)
                            {
                                // keep the FIRST activation in the sequence.
                                _activatedInteractor = interactorId;
                            }
                        }));
                }
                else if (eventType == ActivatableEventType.ActivationFocusChanged)
                {
                    ActivationFocusChangedEventParams focusChangedParams;
                    if (behavior.TryGetActivationFocusChangedEventParams(out focusChangedParams))
                    {
                        _cachedActions.Add(new Action(() =>
                            {
                                _tentativelyFocusedInteractor = IsTrue(focusChangedParams.HasTentativeActivationFocus) ? interactorId : null;
                                _focusedInteractor = IsTrue(focusChangedParams.HasActivationFocus) ? interactorId : null;
                            }));
                    }
                }
            }

            if (!_isFrozenUntilEndOfFrame)
            {
                ExecuteCachedActions();
            }
        }
    }

    public void EndFrame()
    {
        lock (this)
        {
            _activatedInteractor = null;

            _isFrozenUntilEndOfFrame = false;
            ExecuteCachedActions();
        }
    }

    public bool GetIsActivated(string interactorId)
    {
        FreezeFrame();

        return string.Equals(_activatedInteractor, interactorId);
    }

    public ActivationFocusState GetActivationFocusState(string interactorId)
    {
        FreezeFrame();

        if (string.Equals(_focusedInteractor, interactorId))
        {
            return ActivationFocusState.HasActivationFocus;
        }
        else if (string.Equals(_tentativelyFocusedInteractor, interactorId))
        {
            return ActivationFocusState.HasTentativeActivationFocus;
        }
        else
        {
            return ActivationFocusState.None;
        }
    }

    private static bool IsTrue(int booleanValue)
    {
        return booleanValue != EyeXBoolean.False;
    }

    private void FreezeFrame()
    {
        lock (this)
        {
            _isFrozenUntilEndOfFrame = true;
        }
    }

    private void ExecuteCachedActions()
    {
        foreach (var action in _cachedActions)
        {
            action();
        }

        _cachedActions.Clear();
    }
}
