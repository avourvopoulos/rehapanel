// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

    public BITalinoReader reader;
    public int channelRead = 0;
    public double divisor = 1;

    private LineRenderer line;

	// Use this for initialization
	void Start () {
        line = (LineRenderer) this.GetComponent("LineRenderer");
        line.SetVertexCount(reader.BufferSize);
	}


	/// <summary>
	/// Draw the new point of the line
	/// </summary>
	void Update () {

		if (reader.asStart && MainGuiControls.bitalinoMenu)
        {
            int i = 0;
            foreach(BITalinoFrame f in reader.getBuffer())
            {
                float posX = (float) (-7.5f+15f*((1.0/reader.BufferSize)*i));
                float posY = (float) ((f.GetAnalogValue(channelRead)) / divisor);
			    line.SetPosition(i, new Vector3(posX, posY, 0));
                i++;
            }
		}
	}


}
