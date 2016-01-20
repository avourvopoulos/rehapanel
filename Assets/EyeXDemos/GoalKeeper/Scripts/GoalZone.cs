//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a part of the goal.
/// </summary>
public class GoalZone : EyeXGameObjectInteractorBase 
{
	private bool _isLooking;
	private bool _wasClicked;
    
    /// <summary>
    /// Determines whether or not this part of the 
    /// goal is being looked at.
    /// </summary>
	public bool IsBeingLookedAt 
	{
		get { return _isLooking; }
	}
	
    /// <summary>
    /// Determines whether or not this part of the
    /// goal was clicked.
    /// </summary>
	public bool WasClicked
	{
		get { return _wasClicked; }
	}

    protected override void Update()
	{
        // Make sure we call the base class implementation.
        // This will make sure that the interactor location gets properly updated.
		base.Update();
		
		// Are we looking at the game object?
		_isLooking = GameObjectInteractor.HasGaze ();
		_wasClicked = Input.GetMouseButtonDown (0) && PerformHitTest ();
	}
	
    /// <summary>
    /// Tests if the current mouse position was contained within the zone.
    /// </summary>
    /// <returns><c>true</c> if the mouse position was contained; otherwise <c>false</c>.</returns>
	private bool PerformHitTest()
	{
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) 
		{
			if (hit.collider.name.Equals(gameObject.name, System.StringComparison.Ordinal)) 
			{
				return true;
			}
		}
		return false;
	}
	
	protected override IList<IEyeXBehavior> GetEyeXBehaviorsForGameObjectInteractor ()
	{
		return new List<IEyeXBehavior> { new EyeXGazeAware() };
	}
}
