// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO;
using System.Diagnostics;

public class BITalinoReader : MonoBehaviour {
    public ManagerBITalino manager;
    public int BufferSize = 100;
    public bool rawData = false;
    public bool dataFile = false;
    public string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ;

    private Thread readThread;
    private BITalinoFrame[] frameBuffer;
    private bool _Start = false;
    private StreamWriter sw;
    private Stopwatch stopWatch;

    void Start()
    {
        stopWatch = new Stopwatch();
        dataPath += "\\" + DateTime.Now.ToString("MMddHHmmssfff") + "_Data.csv";
        frameBuffer = new BITalinoFrame[BufferSize];
        
        StartCoroutine(start());
	}

	void Update(){
//		UnityEngine.Debug.Log (frameBuffer[1].ToString ());
		}

    /// <summary>
    /// Start the connection
    /// </summary>
    private IEnumerator start()
    {
        readThread = new Thread(Read);
        while (manager.IsReady == false)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        manager.Connection();
        yield return new WaitForSeconds(0.5f);
        
        manager.StartAcquisition();
        yield return new WaitForSeconds(0.5f);

        stopWatch.Start();
        for (int i = 0; i < BufferSize; i++)
        {
            if (rawData)
            {
                frameBuffer[i] = manager.Read(1)[0];
            }
            else
            {
                frameBuffer[i] = convert(manager.Read(1)[0]);
            }
            WriteData(frameBuffer[i]);
        }

        _Start = true;
        readThread.Start();
    }

    /// <summary>
    /// Read a frame and store it in the buffer
    /// </summary>
    private void Read(object obj)
    {
        while (_Start)
        {
            BITalinoFrame[] frames = manager.Read(1);
            int i;
            for (i = 0; i < BufferSize - 1; i++)
            {
                frameBuffer[i] = frameBuffer[i + 1];
            }
            if (rawData)
            {
                frameBuffer[i] = frames[0];
            }
            else
            {
                frameBuffer[i] = convert(frames[0]);
            }
            WriteData(frameBuffer[i]);
        }
    }

    /// <summary>
    /// Return the content of the buffer
    /// </summary>
    /// <returns>Return the content of the buffer</returns>
    public BITalinoFrame[] getBuffer()
    {
        return this.frameBuffer;
    }

    /// <summary>
    /// Get the state of the instance
    /// </summary>
    public bool asStart
    {
        get
        {
            return _Start;
        }
    }
	
    /// <summary>
    /// Convert the data if raw_data is false
    /// </summary>
    /// <param name="frame">Frame that will be convert</param>
    /// <returns>Convert frame</returns>
    private BITalinoFrame convert (BITalinoFrame frame)
    {
        int i = 0;
        foreach(ManagerBITalino.Channels channels in manager.AnalogChannels)
        {
            switch(channels)
            {
                case ManagerBITalino.Channels.EMG :
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleEMG_mV(frame.GetAnalogValue(i)));
                    break;
                case ManagerBITalino.Channels.EDA:
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleEDA(frame.GetAnalogValue(i)));
                    break;
                case ManagerBITalino.Channels.LUX:
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleLUX(frame.GetAnalogValue(i)));
                    break;
                case ManagerBITalino.Channels.ECG:
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleECG_mV(frame.GetAnalogValue(i)));
                    break;
                case ManagerBITalino.Channels.ACC:
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleACC(frame.GetAnalogValue(i)));
                    break;
                case ManagerBITalino.Channels.BATT:
                    frame.SetAnalogValue(i, SensorDataConvertor.ScaleBATT(frame.GetAnalogValue(i)));
                    break;
            }
            i++;
        }
        return frame;
    }

    /// <summary>
    /// Save the read data in a file if data_file is true
    /// </summary>
    /// <param name="frame">data read</param>
    private void WriteData(BITalinoFrame frame)
    {
        try
        {
            if (dataFile)
            {
                if (sw == null)
                {
                    sw = File.AppendText(dataPath);
                    sw.WriteLine(getChannelsRead());
                    sw.Flush();
                }
                sw.WriteLine(CSV_Parser.ToCSV((stopWatch.Elapsed.TotalSeconds) + " " + frame.ToString(),manager.AnalogChannels.Length));
                sw.Flush();
            }
        }
        catch (Exception e)
        { UnityEngine.Debug.Log(e); }  
    }

    /// <summary>
    /// Stop the connection on the stop of the application
    /// </summary>
    private void OnApplicationQuit()
    {
        if (asStart == true)
        {
            _Start = false;
            while (readThread.IsAlive) ;

            manager.StopAcquisition();
            manager.Deconnection();
        }
    }

    /// <summary>
    /// get the time since the start of the aquisition
    /// </summary>
    /// <returns>Return the time in S</returns>
    internal string getTime()
    {
        return stopWatch.Elapsed.TotalSeconds.ToString();
    }

    /// <summary>
    /// Return under CSV format, the name of the read channels
    /// </summary>
    private string getChannelsRead()
    {
        string result = "Time";
        foreach (ManagerBITalino.Channels channels in manager.AnalogChannels)
        {
            switch (channels)
            {
                case ManagerBITalino.Channels.EMG:
                    result += ";EMG";
                    break;
                case ManagerBITalino.Channels.EDA:
                    result += ";EDA";
                    break;
                case ManagerBITalino.Channels.LUX:
                    result += ";LUX";
                    break;
                case ManagerBITalino.Channels.ECG:
                    result += ";ECG";
                    break;
                case ManagerBITalino.Channels.ACC:
                    result += ";ACC";
                    break;
                case ManagerBITalino.Channels.BATT:
                    result += ";BATT";
                    break;
            }
        }
        return result + ";Digit0;Digit1;Digit2;Digit3";
    }
}
