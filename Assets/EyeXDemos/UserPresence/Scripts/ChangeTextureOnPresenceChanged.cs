//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Framework;
using UnityEngine;

public class ChangeTextureOnPresenceChanged : MonoBehaviour
{
    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;

    public Texture textureUsedWhenUserIsPresent;

    private Texture _savedTexture;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
    }

    void Update()
    {
        var userPresenceStateValue = _eyeXHost.UserPresence;
        if (userPresenceStateValue.IsValid && 
            userPresenceStateValue.Value == UserPresence.Present)
        {
            OnUserPresent();
        }
        else
        {
            OnUserGone();
        }
    }

    private void OnUserPresent()
    {
        if (_savedTexture == null)
        {
            _savedTexture = renderer.material.mainTexture;
            renderer.material.mainTexture = textureUsedWhenUserIsPresent;
        }
    }

    private void OnUserGone()
    {
        if (_savedTexture != null)
        {
            renderer.material.mainTexture = _savedTexture;
            _savedTexture = null;
        }
    }
}
