using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using LSL;

public class SendLSL : MonoBehaviour {

	// create stream info and outlet
	static liblsl.StreamInfo info;
	static liblsl.StreamOutlet outlet;
	static float[] data;

	// Use this for initialization
	void Start () {

		// create stream info and outlet
		info = new liblsl.StreamInfo("openBCI", "EEG", 1, 30, liblsl.channel_format_t.cf_float32, "rehabnet");
		outlet = new liblsl.StreamOutlet(info);
		data = new float[8];

//		while (true)
//		{
//			// generate random data and send it
//			for (int k = 0; k < data.Length; k++)
//				data[k] = 0.0f;
//			outlet.push_sample(data);
//			System.Threading.Thread.Sleep(10);
//		}
	
	}
	
	// Update is called once per frame
	void Update () {

		// generate random data and send it
		for (int k = 0; k < data.Length; k++){
			data[k] =  UnityEngine.Random.Range(-100,100);
			outlet.push_sample(data);
		}
	
	}
}
