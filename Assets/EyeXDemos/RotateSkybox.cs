//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//---------------------------------------------------------------------

using UnityEngine;

[RequireComponent(typeof(Skybox))]
public class RotateSkybox : MonoBehaviour 
{
	private Skybox _skybox;
	private float _rotation;

	void Start () 
	{
		_skybox = GetComponent<Skybox> ();
	}

	void Update()
	{
		// Increase the rotation.
		_rotation += Time.deltaTime * 1.0f;

		// Create the rotation matrix.
		Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0f, _rotation, 0f), new Vector3(1,1,1) );

		// Set the rotation on the shader.
		_skybox.material.SetMatrix ("_Rotation", m);
	}
}