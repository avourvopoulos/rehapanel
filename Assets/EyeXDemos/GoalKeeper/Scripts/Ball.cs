//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the ball.
/// </summary>
public class Ball : MonoBehaviour
{
    private BallState _state;

    void Awake()
    {
        SetState(BallState.Playing);
    }

    /// <summary>
    /// Sets the current ball state.
    /// </summary>
    /// <param name="state">The state of the ball.</param>
    public void SetState(BallState state)
    {
        _state = state;

        if (_state == BallState.Hidden)
        {
            // Hide the ball.
            renderer.enabled = false;
            return;
        }

        // Show the ball.
        renderer.enabled = true;

        // Position the ball correctly depending on state.
        if (_state == BallState.Playing)
        {
            transform.position = new Vector3(0f, -3.5f, 0f);
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (_state == BallState.Left)
        {
            transform.position = new Vector3(-4.5f, 3.5f, 0f);
            transform.localScale = new Vector3(0.38f, 0.38f, 1f);
        }
        else if (_state == BallState.Center)
        {
            transform.position = new Vector3(0f, 3.5f, 0f);
            transform.localScale = new Vector3(0.38f, 0.38f, 1f);
        }
        else if (_state == BallState.Right)
        {
            transform.position = new Vector3(4.5f, 3.5f, 0f);
            transform.localScale = new Vector3(0.38f, 0.38f, 1f);
        }
    }
}
