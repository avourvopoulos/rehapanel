using UnityEngine;
using System.Collections;

public class LineGraphs : MonoBehaviour {
	
	public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
	

	// Use this for initialization
	void Start () 
	{
		 LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(0.2F, 0.2F);
        lineRenderer.SetVertexCount(lengthOfLineRenderer);
	}
	
	// Update is called once per frame
	void Update () 
	{
		 LineRenderer lineRenderer = GetComponent<LineRenderer>();
        int i = 0;
        while (i < lengthOfLineRenderer) {
            Vector3 pos = new Vector3(i * 0.5F, Mathf.Sin(i + Time.time), 0);
            lineRenderer.SetPosition(i, pos);
            i++;
		}
	
	}
}