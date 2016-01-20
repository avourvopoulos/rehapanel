// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class ConnectionState : MonoBehaviour {
    public ManagerBITalino manager;
    public BITalinoReader reader;
    public BITalinoSerialPort serial;
    public GUIText state;
    public GUIText data;

	// Use this for initialization
    void Start()
    {
        state.text = "";
        data.text = "";
        StartCoroutine(start());
	}

    /// <summary>
    /// Initialise the connection
    /// </summary>
    private IEnumerator start()
    {
        state.text = "Connecting port " + serial.portName;
        while (!manager.IsReady)
            yield return new WaitForSeconds(0.5f);
        state.text = "Connected";
        while ((int)manager.Acquisition_State != 0)
            yield return new WaitForSeconds(0.5f);
        state.text = "Acquisition start";


    }
	
	/// <summary>
	/// Write the data read from the bitalino
	/// </summary>
	void Update () 
    {
        if (reader.asStart)
        {
            data.text = reader.getBuffer()[reader.BufferSize - 1].ToString();
        }
	}
}
