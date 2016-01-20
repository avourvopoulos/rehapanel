// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class GUI_Selector : MonoBehaviour {

	public Texture2D backIcon;
//	public Camera BitalinoCam;

    private bool EMG = true;
    public LineRenderer LineEMG;
    private bool EDA = true;
    public LineRenderer LineEDA;
    private bool LUX = true;
    public LineRenderer LineLUX;
    private bool ECG = true;
    public LineRenderer LineECG;
    private bool ACC = true;
    public LineRenderer LineACC;
    private bool BATT = true;
    public LineRenderer LineBATT;

	// Use this for initialization
	void Start () {
	}
	
	/// <summary>
	/// Update the state of the line renderer
	/// </summary>
	void Update () {

        LineEMG.enabled = EMG;
        LineEDA.enabled = EDA;
        LineLUX.enabled = LUX;
        LineECG.enabled = ECG;
        LineACC.enabled = ACC;
        LineBATT.enabled = BATT;
	}

    /// <summary>
    /// Drawn the GUI
    /// </summary>
    void OnGUI () {

		if (MainGuiControls.bitalinoMenu)
		{
			//back button	
			if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
			{
				MainGuiControls.bitalinoMenu = false;
				MainGuiControls.hideMenus = false;
				this.camera.enabled = false;
			}	

	        GUI.Box(new Rect(10, 110, 120, 160), "Graph Selector");
	        EMG = GUI.Toggle(new Rect(15, 130, 50, 20), EMG, new GUIContent("EMG"));
	        EDA = GUI.Toggle(new Rect(15, 150, 50, 20), EDA, new GUIContent("EDA"));
	        LUX = GUI.Toggle(new Rect(15, 170, 50, 20), LUX, new GUIContent("LUX"));
	        ECG = GUI.Toggle(new Rect(15, 190, 50, 20), ECG, new GUIContent("ECG"));
	        ACC = GUI.Toggle(new Rect(15, 210, 50, 20), ACC, new GUIContent("ACC"));
	        BATT= GUI.Toggle(new Rect(15, 230, 50, 20), BATT, new GUIContent("BATT"));



		}

    }
}
