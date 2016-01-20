using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class EEGLogger : MonoBehaviour 
{
    EmoEngine engine;
    int userID = -1;
    string filename = "outfile.csv"; // output filename
	
	void Start () 
    {
        engine = EmoEngine.Instance;
        engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded);
        engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);
        engine.Connect();
        WriteHeader();
	}

    void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
        Run();
        //Thread.Sleep(100);
    }

    void engine_UserAdded(object sender, EmoEngineEventArgs e)
    {
        userID = (int)e.userId;

        // enable data acquisition for this user
        engine.DataAcquisitionEnable(e.userId, true);

        // ask for up to 1 second of buffered data
        engine.EE_DataSetBufferSizeInSec(1);
    }	
	
	void Update () 
    {
        
	}

    void Run()
    {
        
        // Handle any waiting events
        engine.ProcessEvents();
        
        // If the user has not yet connected, do not proceed
        if ((int)userID == -1)
            return;
        
        Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);

        
        if (data == null)
        {
            return;
        }

        int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
//        print(_bufferSize);

        //Console.WriteLine("Writing " + _bufferSize.ToString() + " lines of data ");

        // Write the data to a file
        TextWriter file = new StreamWriter(filename, true);

        for (int i = 0; i < _bufferSize; i++)
        {
            // now write the data
            foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                file.Write(data[channel][i] + ",");
            file.WriteLine("");

        }
        file.Close(); 
        
    }

    public void WriteHeader()
    {
        TextWriter file = new StreamWriter(filename, false);

        string header = "COUNTER,INTERPOLATED,RAW_CQ,AF3,F7,F3, FC5, T7, P7, O1, O2,P8" +
                        ", T8, FC6, F4,F8, AF4,GYROX, GYROY, TIMESTAMP, ES_TIMESTAMP" +
                        "FUNC_ID, FUNC_VALUE, MARKER, SYNC_SIGNAL,";

        file.WriteLine(header);
        file.Close();
    }

}
