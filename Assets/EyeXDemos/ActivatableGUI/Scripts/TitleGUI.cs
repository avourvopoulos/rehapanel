//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using Rect = UnityEngine.Rect;

/// <summary>
/// This script shows how the Activatable Behavior can be used with GUI.Button.
/// <para>
/// An activation can be triggered on an activatable interactor by looking at it
/// and pressing the activation key (by default the Applications key) on the
/// keyboard. The activation is triggered when the key is released, so the user
/// can move around their gaze point between different interactors while holding 
/// down the activation key, and then release the key to activate the one 
/// currently looked at. 
/// </para><para>
/// While the activation key is pressed down, the interactor that the user looks 
/// at has activation focus. An interactor with activation focus should be 
/// highlighted so the user knows which interactor will be activated when the 
/// activation key is released.
/// </para><para>
/// For more information about the Activatable Behavior, see the Developer's Guide.
/// </para>
/// </summary>
public class TitleGUI : MonoBehaviour
{
    private const double Z = 1000;
    private const string SpinButtonId = "spin";
    private const string StopButtonId = "stop";
    private const int MenuHintWidth = 270;
    private const int MenuHintHeight = 60;
    private const int MenuHintMargin = 10;

    private static readonly Rect MenuBounds = new Rect(10, 10, 170, 250);
    private static readonly Rect SpinButtonBounds = new Rect(20, 40, 150, 100);
    private static readonly Rect StopButtonBounds = new Rect(20, 150, 150, 100);

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private bool _shouldClearFocus;

    public GameObject target;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
    }

    public void OnEnable()
    {
        // Register activatable interactors for the GUI buttons when the game object is enabled.
        var spinInteractor = new EyeXInteractor(SpinButtonId, EyeXHost.NoParent);
        spinInteractor.EyeXBehaviors.Add(new EyeXActivatable(_eyeXHost.ActivationHub) { IsTentativeFocusEnabled = false });
        spinInteractor.Location = CreateLocation(SpinButtonBounds);
        _eyeXHost.RegisterInteractor(spinInteractor);

        var stopInteractor = new EyeXInteractor(StopButtonId, EyeXHost.NoParent);
        stopInteractor.EyeXBehaviors.Add(new EyeXActivatable(_eyeXHost.ActivationHub) { IsTentativeFocusEnabled = false });
        stopInteractor.Location = CreateLocation(StopButtonBounds);
        _eyeXHost.RegisterInteractor(stopInteractor);
    }

    public void OnDisable()
    {
        // Unregister the interactors when the game object is disabled.
        _eyeXHost.UnregisterInteractor(SpinButtonId);
        _eyeXHost.UnregisterInteractor(StopButtonId);
    }

    public void OnGUI()
    {
        if (_shouldClearFocus)
        {
            GUI.FocusControl(string.Empty);
            _shouldClearFocus = false;
        }

        // Draw the GUI.
        GUI.Box(MenuBounds, "GUI demo");

        // Draw Spin button, and set up handling for it
        var spinButtonInteractor = _eyeXHost.GetInteractor(SpinButtonId);
        GUI.SetNextControlName(SpinButtonId);
        if (GUI.Button(SpinButtonBounds, "Take it for a spin") ||
            spinButtonInteractor.IsActivated())
        {
            // Either the button has been clicked, or the corresponding interactor has been activated
            StartCoroutine("ShowActivationFeedback", SpinButtonId);
            StartSpinning();
        }
        else if (spinButtonInteractor.GetActivationFocusState() == ActivationFocusState.HasActivationFocus)
        {
            // else, if user is looking at button while pressing down activation key
            GUI.FocusControl(SpinButtonId);
        }

        // Draw Stop button, and set up handling for it
        var stopButtonInteractor = _eyeXHost.GetInteractor(StopButtonId);
        GUI.SetNextControlName(StopButtonId);
        if (GUI.Button(StopButtonBounds, "Stop it") ||
            stopButtonInteractor.IsActivated())
        {
            // Either the button has been clicked, or the corresponding interactor has been activated
            StartCoroutine("ShowActivationFeedback", StopButtonId);
            StopSpinning();
        }
        else if (stopButtonInteractor.GetActivationFocusState() == ActivationFocusState.HasActivationFocus)
        {
            // else, if user is looking at button while pressing down activation key
            GUI.FocusControl(StopButtonId);
        }
    }

    private void StartSpinning()
    {
        print("Start spinning command given");
        var rigidbody = target.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.constantForce.enabled = true;
    }

    private void StopSpinning()
    {
        print("Stop spinning command given");
        var rigidbody = target.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    // Coroutine started from OnGUI() if a button is activated using EyeX.
    private IEnumerator ShowActivationFeedback(string buttonId)
    {
        GUI.FocusControl(buttonId);
        yield return new WaitForSeconds(0.1f);
        _shouldClearFocus = true;
    }

    private static ProjectedRect CreateLocation(Rect bounds)
    {
        return new ProjectedRect { isValid = true, rect = bounds, relativeZ = Z };
    }
}
