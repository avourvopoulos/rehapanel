//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using Rect = UnityEngine.Rect;

/// <summary>
/// This script shows how the Activatable Behavior can be used with GUITextures.
/// <para>
/// An activation can be triggered on an activatable interactor by looking at it
/// and pressing the Direct Click key (by default the Right Ctrl key)
/// on the keyboard. The activation is triggered when the key is released, so the
/// user can move around their gaze point between different interactors while holding 
/// down the activation key, and then release the key to activate the one currently
/// looked at. 
/// </para><para>
/// While the activation key is pressed down, the interactor that the user looks at
/// has activation focus. An interactor with activation focus should be highlighted
/// so the user knows which interactor will be activated when the activation key
/// is released.
/// </para><para>
/// In a full screen menu situation where the user has to click one of the buttons,
/// it is appropriate to highlight buttons as soon as the user is looking at them.
/// This can be done using the tentative activation focus. 
/// </para><para>
/// For more information about the Activatable Behavior, see the Developer's Guide.
/// </para>
/// </summary>
public class ActivatableMenuItems : MonoBehaviour
{
    private readonly List<Transform> _menuItems = new List<Transform>();

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private GameMenu _gameMenu;

    public bool showInteractorBounds = false;

    /// <summary>
    /// Initialize EyeX host and game menu on Awake
    /// </summary>
    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();

        _gameMenu = GameObject.Find("Game Menu").GetComponent<GameMenu>();

        // add all GUITextures under game menu to the list of menu items
        foreach (Transform childTransform in transform)
        {
            _menuItems.Add(childTransform);
        }
    }

    /// <summary>
    /// Register an activatable interactor for each menu item when the game object is enabled.
    /// </summary>
    public void OnEnable()
    {
        foreach (var menuItem in _menuItems)
        {
            var interactorId = menuItem.name;
            var interactor = new EyeXInteractor(interactorId, EyeXHost.NoParent);
            interactor.EyeXBehaviors.Add(new EyeXActivatable(_eyeXHost.ActivationHub) { IsTentativeFocusEnabled = true });
            interactor.Location = CreateLocation(menuItem, _gameMenu.IsVisible);
            _eyeXHost.RegisterInteractor(interactor);
        }
    }

    /// <summary>
    /// Unregister the interactors when the game object is disabled.
    /// </summary>
    public void OnDisable()
    {
        foreach (var menuItem in _menuItems)
        {
            var interactorId = menuItem.name;
            _eyeXHost.UnregisterInteractor(interactorId);
        }
    }

    /// <summary>
    /// Update interactor location and act on events
    /// </summary>
    public void Update()
    {
        foreach (Transform menuItem in _menuItems)
        {
            var interactorId = menuItem.name;

            // Update location
            var interactor = _eyeXHost.GetInteractor(interactorId);
            interactor.Location = CreateLocation(menuItem, _gameMenu.IsVisible);

            // Check if activated
            if (interactor.IsActivated())
            {
                HandleActivation(interactorId);
            }

            // Check if focus has changed
            var activationFocusState = interactor.GetActivationFocusState();
            if (activationFocusState == ActivationFocusState.HasActivationFocus 
                || activationFocusState == ActivationFocusState.HasTentativeActivationFocus)
            {
                HighlightMenuItem(menuItem);
            }
            else
            {
                RestoreMenuItem(menuItem);
            }

            // Manually bind the Right Shift key to trigger an activation
            // (in addition to the Direct Click key configured in EyeX Interaction settings)
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                _eyeXHost.TriggerActivation();
            }
        }
    }

    /// <summary>
    /// Draws interactor bounds if enabled
    /// </summary>
    public void OnGUI()
    {
        if (showInteractorBounds)
        {
            DrawInteractorBounds();
        }
    }

    private void HandleActivation(string interactorId)
    {
        switch (interactorId)
        {
            case "Resume":
                _gameMenu.IsVisible = false;
                break;
            case "Quit":
                QuitApplication();
                break;
        }
    }

    private static void HighlightMenuItem(Transform menuItem)
    {
        menuItem.guiTexture.color = new Color(0.65f, 0.65f, 0.65f, 1);
    }

    private static void RestoreMenuItem(Transform menuItem)
    {
        menuItem.guiTexture.color = Color.grey;
    }

    private void DrawInteractorBounds()
    {
        foreach (Transform menuItem in _menuItems)
        {
            var interactorId = menuItem.GetInstanceID().ToString();
            var location = CreateLocation(menuItem, _gameMenu.IsVisible);
            if (location.isValid)
            {
                GUI.Box(location.rect, interactorId);
            }
        }
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static ProjectedRect CreateLocation(Transform menuItem, bool isVisible)
    {
        var width = menuItem.guiTexture.texture.width;
        var height = menuItem.guiTexture.texture.height;
        var xPosition = menuItem.position.x * Screen.width + menuItem.guiTexture.pixelInset.x;
        var yPositionUpperLeft = menuItem.position.y * Screen.height + menuItem.guiTexture.pixelInset.y + height;
        var locationRect = new Rect(xPosition, Screen.height - yPositionUpperLeft, width, height);

        return new ProjectedRect() { isValid = isVisible, rect = locationRect, relativeZ = 1000 };
    }
}
