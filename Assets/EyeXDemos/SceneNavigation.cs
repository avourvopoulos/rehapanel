//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Script that makes it possible to switch to the next scene using the tab key and exit the game using the escape key.
/// </summary>
public class SceneNavigation : MonoBehaviour
{
	private Font _font;

    public bool enableInstructions;
	public string text = "Use the Tab key to cycle through sample scenes. Press Esc to exit.";
	public SceneNavigationAlignment alignment = SceneNavigationAlignment.Bottom;
	public Material material;
    public float width = 700;

	public void Awake()
	{
	}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OnGUI()
    {
        if (!enableInstructions)
        {
            return;
        }

        var defaultStyle = GUI.skin.box;

        try
        {
            var content = new GUIContent(text);

            // Calculate the boundaries.
            var height = GUI.skin.box.CalcHeight(content, width);
            var bounds = new Rect((Screen.width - width) / 2f, Screen.height - 30, width, height);

            // Draw the background rectangle.
            DrawRectangle(bounds);

            // Setup the GUI style.
            var style = new GUIStyle 
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = {textColor = new Color(0.992f, 0.608f, 0.039f)}
            };

            // Draw the label
            GUI.Label(bounds, content, style);
        }
        finally
        {
            // Restore the original style.
            GUI.skin.box = defaultStyle;
        }
    }

    private void LoadNextScene()
    {
        var nextLevel = (Application.loadedLevel + 1) % Application.levelCount;
        Application.LoadLevel(nextLevel);
    }

	private void DrawRectangle (Rect position)
	{    
		// We shouldn't draw until we are told to do so.
		if (Event.current.type != EventType.Repaint) 
		{
			return;
		}
		// Make sure we have a material.
		if(material == null)
		{
			return;
		}
		
        // Set the first pass.
		material.SetPass (0);
	
        // Draw the background rectangle using GL calls.
		GL.Color (new Color (0f, 0f, 0f, 0.8f));
		GL.Begin (GL.QUADS);
		GL.Vertex3 (position.x, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y + position.height, 0);
		GL.Vertex3 (position.x, position.y + position.height, 0);
		GL.End ();
	}
}
