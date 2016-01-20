//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Script to display instructions for the Map Navigation scene.
/// </summary>
public class MapNavigationInstructions : MonoBehaviour
{
    public void OnGUI()
    {
        var defaultStyle = GUI.skin.box;
        GUI.skin.box = CreateBoxStyle();

        var message = "Press and hold the [Space] key to zoom out and get an overview.\nRelease the key to zoom in on your gaze point.";
        var width = 700;
        var height = GUI.skin.box.CalcHeight(new GUIContent(message), width);
        var bounds = new Rect((Screen.width - width) / 2, 10, width, height);
        GUI.Box(bounds, message);

        GUI.skin.box = defaultStyle;
    }

    private static GUIStyle CreateBoxStyle()
    {
        var style = new GUIStyle(GUI.skin.box);

        style.fontSize = 24;

        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.black);
        texture.Apply();

        style.normal.background = texture;
		style.normal.textColor = new Color (0.992f, 0.608f, 0.039f);

        return style;
    }
}
