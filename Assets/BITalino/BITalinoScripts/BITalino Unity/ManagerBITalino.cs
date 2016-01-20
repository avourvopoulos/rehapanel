// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;
using System.Threading;

public class ManagerBITalino : MonoBehaviour {

    public enum AcquisitionState
    {
        Run = 0,
        NotRun = 1
    };

    public enum Channels
    {
        EMG,
        EDA,
        LUX,
        ECG,
        ACC,
        BATT
    };

    // if need a GUI
    public GUIBitalino GUIB;
    public BITalinoSerialPort scriptSerialPort;

    public Channels[] AnalogChannels = { Channels.EMG,Channels.EDA,Channels.LUX,Channels.ECG,Channels.ACC,Channels.BATT };
    public int SamplingRate = 1000;
    public bool logFile = false;
    public string logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    private bool isReady = false;
    private static BITalinoDevice device;
    private IBITalinoCommunication bitalinoCommunication;
    private string version;
    private StreamWriter sw = null;
    private AcquisitionState acquisitionState = AcquisitionState.NotRun;


    #region GETTER/SETTER
    public bool IsReady
    {
        get { return isReady; }
    }
    public AcquisitionState Acquisition_State
    {
        get { return acquisitionState; }
    }
    public IBITalinoCommunication BitalinoCommunication
    {
        get { return bitalinoCommunication; }
        set { bitalinoCommunication = value; }
    }
    #endregion



    // Use this for initialization
	void Start ()
    {
        logPath += "\\" + DateTime.Now.ToString("MMddHHmmssfff") + "_Log.txt";
        if ( GUIB != null )
        {
            GUIB.ManagerB = this;
        }
        
        if( scriptSerialPort != null )
        {
            scriptSerialPort.ManagerB = this;
        } 
	}
	
	/// <summary>
	/// Update the state of the manager
	/// </summary>
	void Update () 
    {
        if ( bitalinoCommunication != null )
        {
            isReady = true;
        }
        else if ( isReady == true )
        {
            isReady = false;
        }
	}

    /// <summary>
    /// Initialize the connection with the BITalino
    /// </summary>
    public void Connection()
    {
        try
        {
            if ( bitalinoCommunication != null && device == null )
            {
                device = new BITalinoDevice(bitalinoCommunication, convertChannels(), SamplingRate);
            }

            device.Connection ( );
 //           WriteLog("Done, Connection");
        }
        catch ( Exception ex )
        {
 //           WriteLog("Error on connection" + ex.Message);
        }
    }

    /// <summary>
    /// Stop the connection with the BITalino
    /// </summary>
    public void Deconnection ( )
    {
        try
        {
            device.Deconnection ( );
            acquisitionState = AcquisitionState.NotRun;
  //          WriteLog("Done, Deconnection");
        }
        catch ( Exception ex )
        {
 //           WriteLog("Error on deconnection" + ex.Message);
        }

    }

    /// <summary>
    /// Get the version of the BITalino
    /// </summary>
    public void GetVersion ()
    {
        try
        {
            version = device.GetVersion ( );
 //           WriteLog("Bitalino's version: " + version);
        }
        catch ( Exception ex )
        {
 //           WriteLog("Error getting version: " + ex.Message);
        } 
    }

    /// <summary>
    /// Start the acquisition
    /// </summary>
    public void StartAcquisition()
    {
        try
        {
            device.SamplingRate = SamplingRate;

            Array.Sort(AnalogChannels);
            device.AnalogChannels = convertChannels();
            device.StartAcquisition ();
            acquisitionState = AcquisitionState.Run;
 //           WriteLog("Done, acquisition started");
        }
        catch ( Exception ex )
        {
 //           WriteLog("Error acquisition: " + ex.Message);
        }
        
    }

    /// <summary>
    /// Stop the acquisition
    /// </summary>
    public void StopAcquisition()
    {
        try
        {
            device.StopAcquisition ( );
            acquisitionState = AcquisitionState.NotRun;
//            WriteLog("DONE acquisition stopped");
        }
        catch ( Exception ex )
        {
//            WriteLog( "Error stopping the acquisition: " + ex.Message );
        }
    }

    /// <summary>
    /// Read data from the BITalino
    /// </summary>
    /// <param name="nbSamples">number of sample reade</param>
    /// <returns>Samples read</returns>
    public BITalinoFrame [ ] Read ( int nbSamples )
    {
        try
        {
            return device.ReadFrames ( nbSamples );
        }
        catch ( Exception ex )
        {
 //           WriteLog( "Error reading the frames: " + ex.Message );
        }

        return null;
    }

    /// <summary>
    /// Write the log data in a file if log_file is true, else write them in the console
    /// </summary>
    /// <param name="log">Data write</param>
    public void WriteLog(String log)
    {
        if (logFile)
        {
            if (sw == null)
            {
                sw = File.AppendText(logPath);
            }
            sw.WriteLine(log);
            sw.Flush();
        }
        else
        {
            Debug.Log(log);
        }
    }

    /// <summary>
    /// Convert the tab of AnalogChannels into an int tab
    /// </summary>
    /// <returns>Int tab of the AnalogChannels</returns>
    public int[] convertChannels()
    {
        int[] convertChannels = new int[AnalogChannels.Length];
        int i = 0;
        foreach (Channels channel in AnalogChannels)
        {
            convertChannels[i] = (int)channel;
            i++;
        }
        return convertChannels;
    }
}
