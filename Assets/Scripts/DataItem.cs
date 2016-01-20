using UnityEngine;
using System.Collections;
using Tobii.Eyetracking.Sdk;

public class DataItem {
	public GazeDataItem gazeItem;
	public float objectX;	
	public float objectY;
	public int isVisible;
	
	public DataItem(GazeDataItem g, float x, float y, bool v)
	{
		gazeItem = g;
		objectX = x;
		objectY = y;
		isVisible = (v ? 1 : 0);
	}
}
