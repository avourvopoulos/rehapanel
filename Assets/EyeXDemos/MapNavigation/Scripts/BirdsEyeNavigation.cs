//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Framework;
using UnityEngine;

/// <summary>
/// Script to navigate on an object, for example a map, using eye fixation data.
/// </summary>
public class BirdsEyeNavigation : MonoBehaviour
{
    /// <summary>
    /// The camera to be used for navigation
    /// </summary>
    public new Camera camera;

    /// <summary>
    /// The zoom speed 
    /// </summary>
    public float speed = 5.0f;

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;
    private IEyeXDataProvider<EyeXFixationPoint> _fixationDataProvider;

    private float _initialSize;
    private Vector3 _initialPosition;
    private float _progress;
    private float _sourceSize;
    private float _targetSize;
    private Vector3 _sourcePosition;
    private Vector3 _targetPosition;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
        _fixationDataProvider = _eyeXHost.GetFixationDataProvider(FixationDataMode.Sensitive);
    }

    public void Start()
    {
        _initialSize = camera.orthographicSize;
        _initialPosition = camera.transform.position;

        _targetSize = _initialSize;
        _targetPosition = camera.transform.position;
        _progress = 1.0f;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // to zoom out to a bird's eye view: center the camera and set its orthographic size 
            // large enough to cover the bounds of the game object (i.e., the map).
            SetTargetPosition(new Vector3(0, 0, camera.transform.position.z));

            var bounds = gameObject.renderer.bounds;
            var w = bounds.extents.x / camera.aspect;
            var h = bounds.extents.y;
            SetTargetSize(Mathf.Max(w, h));

            // start detecting fixations
            _fixationDataProvider.Start();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // stop detecting fixations
            _fixationDataProvider.Stop();

            var lastFixation = _fixationDataProvider.Last;
            if (lastFixation.IsValid && lastFixation.GazePoint.IsWithinScreenBounds)
            {
                var gazePointScreen = new Vector3(lastFixation.GazePoint.Screen.x, lastFixation.GazePoint.Screen.y, 0);

                // center the camera on the gaze point.
                var gazePointWorld = camera.ScreenToWorldPoint(gazePointScreen);
                var targetPositionBeforeAdjustment = new Vector3(gazePointWorld.x, gazePointWorld.y, camera.transform.position.z);

                // ...but when we zoom in, we want to zoom in at the gaze point. That is, the map 
                // position at the gaze point should remain at the same spot in the viewport after
                // the zoom.
                var originalSize = camera.orthographicSize;
                var originalPosition = camera.transform.position;

                // temporarily set size and position to calculate adjustment
                // TODO: This can probably be calculated in a smarter way
                camera.orthographicSize = _initialSize;
                camera.transform.position = targetPositionBeforeAdjustment;
                var adjustment = camera.ScreenToWorldPoint(gazePointScreen) - gazePointWorld;

                // restore size and position before setting new targets
                camera.orthographicSize = originalSize;
                camera.transform.position = originalPosition;

                SetTargetSize(_initialSize);
                SetTargetPosition(targetPositionBeforeAdjustment - new Vector3(adjustment.x, adjustment.y, 0));
            }
            else
            {
                SetTargetSize(_initialSize);
                SetTargetPosition(_initialPosition);
            }
        }

        UpdateCameraSizeAndPosition();
    }

    private void UpdateCameraSizeAndPosition()
    {
        if (_progress < 1.0f)
        {
            _progress = Mathf.Clamp01(_progress + speed * Time.deltaTime);
        }

        camera.orthographicSize = Mathf.Lerp(_sourceSize, _targetSize, _progress);
        camera.transform.position = Vector3.Lerp(_sourcePosition, _targetPosition, _progress);
    }

    private void SetTargetPosition(Vector3 targetPosition)
    {
        _sourcePosition = camera.transform.position;
        _targetPosition = targetPosition;

        _progress = 0.0f;
    }

    private void SetTargetSize(float targetSize)
    {
        _sourceSize = camera.orthographicSize;
        _targetSize = targetSize;

        _progress = 0.0f;
    }
}
