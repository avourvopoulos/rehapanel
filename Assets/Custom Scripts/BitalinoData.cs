using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using LSL;

public class BitalinoData : MonoBehaviour {

	// create LSL stream info and outlet
//	static liblsl.StreamInfo info;
//	static liblsl.StreamOutlet outlet;
	static float[] data;
	
	// receiving Thread
	Thread receiveThread;
	bool isConnected = false;

	public BITalinoReader reader;
	public int channelRead = 0;
	public double divisor = 1;

	// Use this for initialization
	void Start () 
	{
		init ();

		// create LSL stream info and outlet
//		info = new liblsl.StreamInfo("bitalino", "EMG", 6, 2000, liblsl.channel_format_t.cf_float32, "rehabnet");
//		outlet = new liblsl.StreamOutlet(info);
//		data = new float[6];
	}

	public void init()
	{		
		// Local endpoint define (where messages are received).
		// Create a new thread to receive incoming messages.
		isConnected = true;
		receiveThread = new Thread(ReceiveData);
		receiveThread.IsBackground = true;	
		receiveThread.Start();
	//	Debug.Log ("receiveThread");		
	}


	public void ReceiveData() 
	{	

		BITalinoFrame[] frames = reader.getBuffer ();

		while (isConnected)
		{

			try 
			{ 

//				int i = 0;
//				foreach(BITalinoFrame f in reader.getBuffer())
//				{
				if (reader.asStart){
//					float eda =(float)frames [reader.BufferSize - 1].GetAnalogValue (5);
//					Debug.Log("EDA: "+eda);
					for(int i=0; i<reader.BufferSize-1; i++){

						data[i] =  (float)frames [reader.BufferSize-1].GetAnalogValue (i);
//						outlet.push_sample(data);

			//			Debug.Log(reader.BufferSize+" - "+i+": "+(float)frames [reader.BufferSize-1].GetAnalogValue (i));

						if(!DevicesLists.availableDev.Contains("BITALINO:ANALOG:ALL:DATA"))
						{
							DevicesLists.availableDev.Add("BITALINO:ANALOG:ALL:DATA");		
						}
						if(DevicesLists.selectedDev.Contains("BITALINO:ANALOG:ALL:DATA") && UDPData.flag==true)
						{					
							UDPData.sendString("[$]analog,[$$]"+"bitalino"+",[$$$]data,"+i.ToString()+","+data[i].ToString()+";"); 
						}

					}

				}
//					i++;
//				}

				
				
			}//try
			catch (Exception err) 
			{
				print(err.ToString());
			}
			
		}//while true		
		
	}//ReceiveData
	

	// Update is called once per frame
	void Update () {

	
	}

	void OnDisable() 
	{ 
		if(isConnected)
		{
			receiveThread.Abort(); 
			isConnected = false;
		}
	} 	
	
	void OnApplicationQuit () 
	{
		if(isConnected)
		{
			receiveThread.Abort();
			isConnected = false;
		}
	}


}
