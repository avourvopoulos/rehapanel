//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;

/// <summary>
/// Script for a game object that occludes (hides) other EyeX interactors.
/// </summary>
public class Occluder : EyeXGameObjectInteractorBase 
{
    protected override IList<IEyeXBehavior> GetEyeXBehaviorsForGameObjectInteractor()
    {
        // No behaviors, just occlude other interactors.
        return new List<IEyeXBehavior>();
    }
}
