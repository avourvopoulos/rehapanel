// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class GUIBitalino : MonoBehaviour {

    private ManagerBITalino managerB;

    public ManagerBITalino ManagerB { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI ( )
    {
        if ( GUI.Button ( new Rect ( 10, 10, 100, 25 ), "CONNECTION" ) )
        {
            ManagerB.Connection ( );
        }

        if (GUI.Button(new Rect(10, 50, 100, 25), "DECONNECTION"))
        {
            ManagerB.Deconnection ( );
        }

        if (GUI.Button(new Rect(10, 100, 100, 25), "START ACQUISITION"))
        {
            ManagerB.StartAcquisition ( );
        }

        if (GUI.Button(new Rect(110, 100, 100, 25), "STOP ACQUISITION"))
        {
            ManagerB.StopAcquisition ( );
        }

        if (GUI.Button(new Rect(10, 150, 100, 25), "VERSION"))
        {
            ManagerB.GetVersion ( );
        }
    }
}
