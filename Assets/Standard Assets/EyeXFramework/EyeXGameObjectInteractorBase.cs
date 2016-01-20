//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tobii.EyeX.Framework;
using UnityEngine;

/// <summary>
/// Base class for scripts that own a region-bound EyeX interactor.
/// The bounds of the interactor are kept in sync with the script's game object automatically.
/// </summary>
public abstract class EyeXGameObjectInteractorBase : MonoBehaviour
{
    // Size (resolution) of the mask used for this interactor. The default setting, which is 
    // "None", makes the interactor fill the entire projected bounds.
    public EyeXMaskType maskType;

    // Option to draw the projected bounds of the interactor. For debugging purposes.
    public bool showProjectedBounds = false;

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private string _interactorId;
	private Renderer _gameObjectRenderer;

    protected EyeXHost Host
    {
        get { return _eyeXHost; }
    }

    protected EyeXInteractor GameObjectInteractor
    {
        get { return _eyeXHost.GetInteractor(_interactorId); }
    }

    protected virtual void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _interactorId = gameObject.GetInstanceID().ToString();
    }

    protected virtual void Update()
    {
        var interactor = _eyeXHost.GetInteractor(_interactorId);
        UpdateInteractorLocation(interactor);
    }

    /// <summary>
    /// Register the interactor when the game object is enabled.
    /// </summary>
    protected virtual void OnEnable()
    {
        var interactor = new EyeXInteractor(_interactorId, EyeXHost.NoParent);
        interactor.EyeXBehaviors = GetEyeXBehaviorsForGameObjectInteractor();
        UpdateInteractorLocation(interactor);
        _eyeXHost.RegisterInteractor(interactor);
    }

    /// <summary>
    /// Unregister the interactor when the game object is disabled.
    /// </summary>
    protected virtual void OnDisable()
    {
        _eyeXHost.UnregisterInteractor(_interactorId);
    }

    /// <summary>
    /// Draw the projected bounds if 'showProjectedBounds' is enabled.
    /// NOTE: Could be replaced by Gizmos
    /// </summary>
    protected virtual void OnGUI()
    {
        if (showProjectedBounds)
        {
            var face = GetProjectedRect();
            if (face.isValid)
            {
                GUI.Box(face.rect, _interactorId);
            }
        }
    }

    /// <summary>
    /// Gets the EyeX behaviors for the interactor.
    /// </summary>
    /// <returns>List of EyeX behaviors.</returns>
    protected abstract IList<IEyeXBehavior> GetEyeXBehaviorsForGameObjectInteractor();

    /// <summary>
    /// Gets the ProjectedRect on Screen for a Game Object based on its bounding box in World.
    /// Override this method if you want to calculate the World to Screen projected
    /// rectangle for the game object some other way.
    /// </summary>
    /// <returns></returns>
    protected virtual ProjectedRect GetProjectedRect()
    {
        return ProjectedRect.GetProjectedRect(GetBounds(), Camera.main);
    }

    /// <summary>
    /// Gets the bounds (bounding box in World) for a Game Object from its renderer. 
    /// Override this method to provide bounds for Game Objects that do not have a 
    /// renderer. Alternatively override GetProjectedRect() to provide the projected
    /// rectangle on Screen directly.
    /// </summary>
    /// <returns></returns>
    protected virtual Bounds GetBounds()
    {
        return GetRenderer().bounds;
    }

    private void UpdateInteractorLocation(EyeXInteractor interactor)
    {
        var location = GetProjectedRect();

        EyeXMask mask = null;
        if (location.isValid && 
            maskType != EyeXMaskType.None)
        {
            // Create a mask for the interactor and move it to the top.
            mask = CreateMask(location.rect, Screen.height);
            location.relativeZ = Camera.main.farClipPlane;
        }

        interactor.Location = location;
        interactor.Mask = mask;
    }

    private EyeXMask CreateMask(UnityEngine.Rect locationRect, int screenHeight)
    {
        var mask = new EyeXMask(maskType);

        int size = mask.Size;
        for (int row = 0; row < size; row++)
        {
            var y = screenHeight - (locationRect.yMin + (row + 0.5f) * locationRect.height / (size - 1));

            for (int col = 0; col < size; col++)
            {
                var x = locationRect.xMin + (col + 0.5f) * locationRect.width / (size - 1);

                var ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo) &&
                    hitInfo.collider.gameObject.Equals(gameObject))
                {
                    mask[row, col] = MaskWeights.Default;
                }
            }
        }

        return mask;
    }

	private Renderer GetRenderer()
	{
		if (_gameObjectRenderer == null) 
		{
			_gameObjectRenderer = gameObject.renderer ?? GetComponentInChildren<Renderer>(); 

			if(_gameObjectRenderer == null)
			{
				throw new InvalidOperationException("Could not resolve renderer for game object.");
			}
		}
		return _gameObjectRenderer;
	}
}
