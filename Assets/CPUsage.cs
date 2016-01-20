using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;

public class CPUsage : MonoBehaviour {
	
	const float sampleFrequencyMillis = 1000;

    protected object syncLock = new object();
    protected PerformanceCounter counter;
    protected float lastSample;
    protected DateTime lastSampleTime;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
    {
		  //display CPU
		  GUI.Label(new Rect((Screen.width / 2)-155, (Screen.height / 2)-250, 100, 20), "CPU: "+GetCurrentValue()); 
	}


    /// <summary>
    /// 
    /// </summary>
    public void ProcessorUsage()
    {
        this.counter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetCurrentValue()
    {
        if ((DateTime.UtcNow - lastSampleTime).TotalMilliseconds > sampleFrequencyMillis)
        {
            lock (syncLock)
            {
                if ((DateTime.UtcNow - lastSampleTime).TotalMilliseconds > sampleFrequencyMillis)
                {
                    lastSample = counter.NextValue();
                    lastSampleTime = DateTime.UtcNow;
                }
            }
        }

        return lastSample;
    }
	
}
