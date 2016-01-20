//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class Instructions : MonoBehaviour
{
	private float _transparency;
	private float _elapsedTime;

	public float delayInSeconds = 6f;
	public string text = "Put instructions here!";
	public float width = 700;
	public Material material;

	public void Awake()
	{
		System.Diagnostics.Debug.Assert (material != null, "Instructions require a material.");
		System.Diagnostics.Debug.Assert (material.passCount > 0, "Material requires at least one pass.");

		_transparency = 1f;
		_elapsedTime = delayInSeconds;	
	}
	
	public void OnGUI()
	{
		var delta = Time.deltaTime * 1f;
		
		// Decrease the elapsed time.
		_elapsedTime -= delta;
		if(_elapsedTime < 0.0f)
		{
			_transparency = Mathf.Lerp (_transparency,0f, delta);
		}
		
		// Draw the GUI if we're supposed to show it.
		if(_transparency > 0)
		{
			var content = new GUIContent(text);

			// Create the font style.
			var style = new GUIStyle();
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 24;
			style.normal.textColor = new Color(0.953f, 0.569f, 0.039f, _transparency);

			// Calculate the boundaries.
			var height = style.CalcHeight(content, width) + 30;
			var bounds = new Rect((Screen.width - width) / 2, Screen.height / 2 - (height / 2), width, height);

			// Draw the background rectangle.
			DrawRectangle(bounds);

			// Draw the label.
			GUI.Label(bounds, content, style);
		}	
	}

	void DrawRectangle (Rect position)
	{    
		// We shouldn't draw until we are told to do so.
		if (Event.current.type != EventType.Repaint) 
		{
			return;
		}
		// Make sure we have a material with at least on pass.
		if(material == null || material.passCount == 0)
		{
			return;
		}

		// Activate the first pass.
		material.SetPass (0);

		GL.Color ( new Color (0f, 0f, 0f, _transparency));
		GL.Begin (GL.QUADS);
		GL.Vertex3 (position.x, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y + position.height, 0);
		GL.Vertex3 (position.x, position.y + position.height, 0);
		GL.End ();
	}
}