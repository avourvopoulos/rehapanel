// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class BITalinoSerialPort : MonoBehaviour {

    public string portName = "COM4";
    public int baudRate = 9600;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;
    public int ReadTimeOut = 5000;
    public int WriteTimeOut = 5000;

    private ManagerBITalino managerB;
    private SerialPort serialPort;
    private IBITalinoCommunication bitalinoCommunication;

    public ManagerBITalino ManagerB { get; set; }

	bool start = false;
	bool open = false;

	/// <summary>
	/// Initialize the serial connection
	/// </summary>
	void Start () {

//		init ();
    }

	void init()
	{

		serialPort = new SerialPort ( portName, baudRate, parity, dataBits, stopBits );
		
		serialPort.ReadTimeout = ReadTimeOut;
		serialPort.WriteTimeout = WriteTimeOut;
		
		bitalinoCommunication = new BITalinoCommunicationSerialPort ( serialPort );

		open = true;
	}

	void openConnection()
	{

		//bitalinoCommunication.Open ();
		ManagerB.StartAcquisition();
		start = true;
	}


	void closeConnection()
	{
		//bitalinoCommunication.Close ();
		ManagerB.StopAcquisition ();
		start = false;
		open = false;
	}
	
	/// <summary>
	/// Set the serial connection in the manager
	/// </summary>
	void Update () {

	    if ( ManagerB.BitalinoCommunication == null )
        {
            ManagerB.BitalinoCommunication = bitalinoCommunication;
		}

	}

	void OnGUI () {
		
		if (MainGuiControls.bitalinoMenu)
		{
			if(open)
			{
				if(!start)
				if (GUI.Button (new Rect (280, 100, 120, 30), "Start Acquisition"))
						openConnection ();
				if(start)
				if(GUI.Button(new Rect(280, 100, 120, 30), "Stop Acquisition"))
						closeConnection();
			}
			else{
				if(!start){
				if(GUI.Button(new Rect(220, 100, 100, 30), "Open"))
					init();
			
				//fields
				portName = GUI.TextField(new Rect(250, 150, 50, 20), portName, 25);
				GUI.Label (new Rect (200, 150, 100, 20), "Port: ");
				}

			}


		}
	}


}
