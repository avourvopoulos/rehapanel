using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Threading;

public class MyoData : MonoBehaviour {

	//csv log
	TextWriter file;
	public static string timestamp;
	static string date = String.Empty;
	string filepath = String.Empty;
	public float uptime;

	//sensor data

	
	public static bool startLog = false;
	bool islogging = false;
	
	string DeviceName = string.Empty;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{


		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "myo";
		}


		//start/stop logging
		if(startLog && !islogging)
		{
			logInit();
		}
		else if (!startLog && islogging)
		{
			endLog();
		}
	
	}


	void logInit()
	{
		islogging = true;
		
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/RehabNet Log/Wii/";
		if(!Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		
		//xml init
		if(XmlDataWriter.xml)
		{

		}
		
		//csv init
		if(XmlDataWriter.csv)
		{
			filepath = path + "Myo_"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			
			file = new StreamWriter(filepath, false);
			
			string header = "timestamp, uptime, "+
				"AccX, AccY, AccZ,"+
					"GyroX, GyroY, GyroZ,"+
					"EMG1, EMG2,"+
					"EMG3, EMG4,"+
					"EMG5, EMG6"+
					"EMG7, EMG8";
			file.WriteLine(header);
			file.Close();
			
			InvokeRepeating("csvWrite", 1F, 0.0010F);//1000 Hz
		}
		
		Debug.Log("Started Myo Logging");
	}//log init
	
	
	void csvWrite()
	{
		uptime+= Time.deltaTime;
		
		file = new StreamWriter(filepath, true);
		file.Write(timestamp +","+ uptime.ToString()+ "," +	
		           ThalmicMyo.accx.ToString() + "," + ThalmicMyo.accy.ToString() + "," + ThalmicMyo.accz.ToString() + "," +	
		           ThalmicMyo.gyrox.ToString() + "," + ThalmicMyo.gyroy.ToString() + "," + ThalmicMyo.gyroz.ToString() + "," +
		           ThalmicMyo.EmgData[0].ToString() + "," + ThalmicMyo.EmgData[1].ToString() + "," +
		           ThalmicMyo.EmgData[2].ToString() + "," + ThalmicMyo.EmgData[3].ToString() + "," +
		           ThalmicMyo.EmgData[4].ToString() + "," + ThalmicMyo.EmgData[5].ToString() + "," +
		           ThalmicMyo.EmgData[6].ToString() + "," + ThalmicMyo.EmgData[7].ToString());
		file.WriteLine("");
		file.Close();			
	}	

	void endLog()
	{
		if(XmlDataWriter.xml)
		{		
		
		}
		
		//csv
		if (XmlDataWriter.csv)
		{
			uptime = 0;
			CancelInvoke("csvWrite");
			islogging = false;
			file.Close();
			file.Dispose();
		}		
		Debug.Log("Stoped Myo Logging");
	}
}
