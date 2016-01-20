//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using SystemRandom = System.Random;

/// <summary>
/// Represents the goal keeper and acts as the game controller.
/// </summary>
public class Goalkeeper : MonoBehaviour
{
    private readonly Dictionary<GoalkeeperState, Vector3> _spriteStates;
    private Transform _transform;
    private SpriteRenderer _renderer;
    private GoalkeeperState _currentState;
    private Vector3 _position;
    private SystemRandom _random;
    private GoalZone _leftZone;
    private GoalZone _centerZone;
    private GoalZone _rightZone;
    private Ball _ball;

    public Sprite[] Sprites;

    private enum GoalkeeperState
    {
        MissRight = 0,
        MissLeft = 1,
        CatchRight = 2,
        CatchLeft = 3,
        CatchCenter = 4,
        Playing = 5,
        MissCenter = 6
    }

    public Goalkeeper()
    {
        _spriteStates = new Dictionary<GoalkeeperState, Vector3>();
        _random = new SystemRandom(DateTime.Now.Millisecond);
    }

    /// <summary>
    /// Script initialization.
    /// </summary>
    public void Awake()
    {
        // Get the transform and renderer;
        _transform = GetComponent<Transform>();
        _renderer = GetComponent<SpriteRenderer>();

        // Get the zones.
        _leftZone = GameObject.Find("Left-Zone").GetComponent<GoalZone>();
        _centerZone = GameObject.Find("Center-Zone").GetComponent<GoalZone>();
        _rightZone = GameObject.Find("Right-Zone").GetComponent<GoalZone>();

        // Get the ball.
        _ball = GameObject.Find("Ball").GetComponent<Ball>();
        _ball.SetState(BallState.Playing);

        // Initialize all sprite states.
        _spriteStates.Add(GoalkeeperState.Playing, new Vector3(0, 0, 0));
        _spriteStates.Add(GoalkeeperState.CatchLeft, new Vector3(-4, 0, 0));
        _spriteStates.Add(GoalkeeperState.CatchRight, new Vector3(4, 0, 0));
        _spriteStates.Add(GoalkeeperState.CatchCenter, new Vector3(0, 0, 0));
        _spriteStates.Add(GoalkeeperState.MissLeft, new Vector3(-4, 0, 0));
        _spriteStates.Add(GoalkeeperState.MissCenter, new Vector3(0, 0, 0));
        _spriteStates.Add(GoalkeeperState.MissRight, new Vector3(4, 0, 0));

        // Set the default state.
        SetGoalkeeperState(GoalkeeperState.Playing);
    }

    /// <summary>
    /// Updates the behaviour.
    /// </summary>
    public void Update()
    {
        if (_currentState == GoalkeeperState.Playing)
        {
            // Update the position of the goalie.
            UpdateGoalkeeperPosition();

            if (Input.GetMouseButtonDown(0))
            {
                // Get the zone the player looked at.
                var gazeZone = GetPlayerGazeZone();

                // Get the zone the player clicked.
                var clickedZone = GetClickedZone();
                if (clickedZone == GoalZoneType.None)
                {
                    return;
                }

                // Update the goalie state.
                var catched = UpdateGoalkeeperState(gazeZone, clickedZone);

                // Update the ball state.
                UpdateBallState(clickedZone, catched);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Reset the game.
                SetGoalkeeperState(GoalkeeperState.Playing);
                _ball.SetState(BallState.Playing);
            }
        }
    }

    /// <summary>
    /// Updates the state of the goalkeeper.
    /// </summary>
    /// <returns><c>true</c>, if the goalkeeper catched the ball, <c>false</c> otherwise.</returns>
    /// <param name="gazeZone">The zone where the player looked.</param>
    /// <param name="clickedZone">The zone where the player clicked.</param>
    private bool UpdateGoalkeeperState(GoalZoneType gazeZone, GoalZoneType clickedZone)
    {
        if (gazeZone == GoalZoneType.None)
        {
            // Player isn't looking at anything.
            // Randomize the outcome depending on where the player clicked.
            return RandomizeGoalkeeperState(clickedZone);
        }
        else if (gazeZone == GoalZoneType.Left)
        {
            if (clickedZone == GoalZoneType.Left)
            {
                SetGoalkeeperState(GoalkeeperState.CatchLeft);
                return true;
            }
            SetGoalkeeperState(GoalkeeperState.MissLeft);
        }
        else if (_centerZone.IsBeingLookedAt)
        {
            if (_centerZone.WasClicked)
            {
                SetGoalkeeperState(GoalkeeperState.CatchCenter);
                return true;
            }
            SetGoalkeeperState(GoalkeeperState.MissCenter);
        }
        else if (_rightZone.IsBeingLookedAt)
        {
            if (_rightZone.WasClicked)
            {
                SetGoalkeeperState(GoalkeeperState.CatchRight);
                return true;
            }
            SetGoalkeeperState(GoalkeeperState.MissRight);
        }
        return false;
    }

    /// <summary>
    /// Updates the ball state.
    /// </summary>
    /// <param name="clickedZone">The zone where the player clicked.</param>
    /// <param name="catched">If set to <c>true</c> the ball was catched.</param>
    private void UpdateBallState(GoalZoneType clickedZone, bool catched)
    {
        if (!catched)
        {
            if (_leftZone.WasClicked)
            {
                _ball.SetState(BallState.Left);
            }
            else if (_centerZone.WasClicked)
            {
                _ball.SetState(BallState.Center);
            }
            else if (_rightZone.WasClicked)
            {
                _ball.SetState(BallState.Right);
            }
        }
        else
        {
            _ball.SetState(BallState.Hidden);
        }
    }

    /// <summary>
    /// Sets the goalkeeper state (sprite and position).
    /// </summary>
    /// <param name="state">State.</param>
    private void SetGoalkeeperState(GoalkeeperState state)
    {
        _currentState = state;
        _position = _spriteStates[_currentState];
        _renderer.sprite = Sprites[(int)_currentState];
        _transform.position = _position;
    }

    /// <summary>
    /// Updates the goalkeeper state.
    /// </summary>
    private void UpdateGoalkeeperPosition()
    {
        // Get the target position.
        var x = _leftZone.IsBeingLookedAt ? -2.0f : _centerZone.IsBeingLookedAt ? 0.0f : _rightZone.IsBeingLookedAt ? 2.0f : 0.0f;
        var targetPosition = new Vector3(_position.x + x, _position.y, _position.z);

        // Lerp towards the target position (2 units/sec).
        var speed = Time.deltaTime * 2.0f;
        var diff = Mathf.Lerp(_transform.position.x, targetPosition.x, speed);
        _transform.position = new Vector3(diff, _position.y, _position.z);
    }

    /// <summary>
    /// Gets the zone where the user clicked.
    /// </summary>
    /// <returns>The clicked zone.</returns>
    private GoalZoneType GetClickedZone()
    {
        return _leftZone.WasClicked ? GoalZoneType.Left :
            _centerZone.WasClicked ? GoalZoneType.Center :
                _rightZone.WasClicked ? GoalZoneType.Right : GoalZoneType.None;
    }

    /// <summary>
    /// Gets the zone where the user looked.
    /// </summary>
    /// <returns>The gaze zone.</returns>
    private GoalZoneType GetPlayerGazeZone()
    {
        return _leftZone.IsBeingLookedAt ? GoalZoneType.Left :
            _centerZone.IsBeingLookedAt ? GoalZoneType.Center :
                _rightZone.IsBeingLookedAt ? GoalZoneType.Right : GoalZoneType.None;
    }

    /// <summary>
    /// Randomizes the state of the goalkeeper when we have no gaze data.
    /// </summary>
    /// <returns><c>true</c>, if the goalkeeper catched the ball, <c>false</c> otherwise.</returns>
    /// <param name="clickedZone">The clicked zone.</param>
    private bool RandomizeGoalkeeperState(GoalZoneType clickedZone)
    {
        var state = GoalkeeperState.Playing;

        var catched = _random.Next(0, 2) == 1;
        if (catched)
        {
            // Make sure we catch the ball in the same zone the player clicked.
            state = clickedZone == GoalZoneType.Left ? GoalkeeperState.CatchLeft
                : clickedZone == GoalZoneType.Center ? GoalkeeperState.CatchCenter
                : clickedZone == GoalZoneType.Right ? GoalkeeperState.CatchRight : GoalkeeperState.MissLeft;
        }
        else
        {
            // Randomize a state.
            state = (GoalkeeperState)_random.Next(0, 2);
        }

        // Set the goalkeepers state.
        SetGoalkeeperState(state);

        // Return whether or not the goalkeeper catched the ball.
        return catched;
    }
}
