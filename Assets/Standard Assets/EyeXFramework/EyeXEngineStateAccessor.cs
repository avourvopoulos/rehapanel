//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using Tobii.EyeX.Client;

/// <summary>
/// Accesses and monitors engine states.
/// Used by the EyeXHost.
/// </summary>
/// <typeparam name="T">Data type of the engine state.</typeparam>
internal class EyeXEngineStateAccessor<T>
{
    private readonly string _statePath;
    private readonly AsyncDataHandler _handler;
    private EventHandler<EyeXEngineStateValue<T>> _eventHandler;
    private EyeXEngineStateValue<T> _currentValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="EyeXEngineStateAccessor{T}"/> class.
    /// </summary>
    /// <param name="statePath">The state path.</param>
    /// <param name="handler">Callback to be invoked when the state changes.</param>
    public EyeXEngineStateAccessor(string statePath, AsyncDataHandler handler)
    {
        _statePath = statePath;
        _handler = handler;
    }

    /// <summary>
    /// Gets the current value of the engine state.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <returns>The state value.</returns>
    public EyeXEngineStateValue<T> GetCurrentValue(Context context)
    {
        if (_currentValue == null)
        {
            // this is the first time someone asks for the current value.
            // we don't have a value yet, but we'll make sure we get one.
            _currentValue = EyeXEngineStateValue<T>.Invalid;
            RegisterListener((s, e) => { _currentValue = e; }, context);
        }

        return _currentValue;
    }

    /// <summary>
    /// Registers a listener for state-changed events.
    /// </summary>
    /// <param name="listener">Event listener to be registered.</param>
    /// <param name="context">The interaction context.</param>
    private void RegisterListener(EventHandler<EyeXEngineStateValue<T>> listener, Context context)
    {
        if (_eventHandler == null &&
            context != null)
        {
            // when the first event listener is registered: register a state-changed handler with the context.
            context.RegisterStateChangedHandler(_statePath, _handler);
            context.GetStateAsync(_statePath, _handler);
        }

        _eventHandler += listener;
    }

    /// <summary>
    /// Handles a state-changed event that may or may not affect the state path handled by this instance.
    /// </summary>
    /// <param name="stateBag">Event data.</param>
    /// <param name="sender">The source of the event.</param>
    public void HandleStateChanged(StateBag stateBag, object sender)
    {
        var handler = _eventHandler;
        if (handler != null)
        {
            T value;
            if (stateBag.TryGetStateValue(out value, _statePath))
            {
                handler(sender, new EyeXEngineStateValue<T>(value));
            }
        }
    }

    /// <summary>
    /// Method to be invoked when a connection to the EyeX Engine has been established.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    public void OnConnected(Context context)
    {
        // when connected: send a request for the initial state.
        if (_eventHandler != null)
        {
            context.GetStateAsync(_statePath, _handler);
        }
    }

    /// <summary>
    /// Method to be invoked when the connection to the EyeX Engine has been lost.
    /// </summary>
    public void OnDisconnected()
    {
        // when disconnected: raise a state-changed event marking the state as invalid.
        var handler = _eventHandler;
        if (handler != null)
        {
            handler(this, EyeXEngineStateValue<T>.Invalid);
        }
    }
}
