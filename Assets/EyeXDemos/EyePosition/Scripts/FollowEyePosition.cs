//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Unity script for a game object that follows the position of an eye in 3D space.
/// </summary>
public class FollowEyePosition : MonoBehaviour
{
    // Scale: 1 mm maps to 0.001 units in world space
    private const float Scale = 1 / 1000.0f;

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private IEyeXDataProvider<EyeXEyePosition> _eyePositionProvider;

    /// <summary>
    /// Choice of eye position to follow, the position of the right or the left eye.
    /// </summary>
    public Eye eyeToFollow = Eye.Left;

    public enum Eye
    {
        Left,
        Right
    }

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _eyePositionProvider = _eyeXHost.GetEyePositionDataProvider();
    }

    public void OnEnable()
    {
        _eyePositionProvider.Start();
    }

    public void OnDisable()
    {
        _eyePositionProvider.Stop();
    }

    public void Update()
    {
        // Get the latest eye position data for both eyes from the eye position provider 
        var eyePosition = _eyePositionProvider.Last;

        // Get the eye position of the selected eye to follow
        var singleEyePosition = eyeToFollow == Eye.Left ? eyePosition.LeftEye : eyePosition.RightEye;

        if (singleEyePosition.IsValid)
        {
            // show the game object
            renderer.enabled = true;

            // move the game object to the current position of the selected eye to follow
            transform.position = new Vector3(-singleEyePosition.X * Scale, singleEyePosition.Y * Scale, singleEyePosition.Z * Scale);
        } 
        else
        {
            // if there is no position for the eye to follow, for example during blink:
            // hide the game object
            renderer.enabled = false;
        }
    }
}