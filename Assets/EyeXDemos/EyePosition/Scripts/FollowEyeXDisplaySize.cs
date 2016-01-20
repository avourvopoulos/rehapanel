using UnityEngine;
using System.Collections;

public class FollowEyeXDisplaySize : MonoBehaviour
{
    // Scale: 1 mm maps to 0.001 units in world space. and a plane is 10x10 units.
    private const float Scale = 1 / 10000.0f;

    // A reference to the EyeX host instance, initialized on Awake. See EyeXHost.GetInstance().
    private EyeXHost _eyeXHost;

    public void Awake()
    {
        _eyeXHost = EyeXHost.GetInstance();
    }

    void Update()
    {
        var displaySize = _eyeXHost.DisplaySize;
        if (displaySize.IsValid)
        {
            transform.localScale = new Vector3((float)(Scale * displaySize.Value.Width), 1, (float)(Scale * displaySize.Value.Height));
        }
    }
}
