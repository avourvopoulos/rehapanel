// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;

public class BITalinoFrame
{

    private double[] analog = new double [ 6 ];

    private int[] digital = new int [ 4 ];

    #region GETTER/SETTER

    public int CRC { get; set; }

    public int Sequence { get; set; }

    public double GetAnalogValue ( int idx )
    {
        try
        {
            return analog [ idx ];
        }
        catch ( IndexOutOfRangeException ex )
        {
            throw ex;
        }
    }

    public void SetAnalogValue ( int idx, double value )
    {
        try
        {
            analog [ idx ] = value;
        }
        catch ( IndexOutOfRangeException ex )
        {
            throw ex;
        }
    }

    public int GetDigitalValue ( int idx )
    {
        try
        {
            return digital [ idx ];
        }
        catch ( IndexOutOfRangeException ex )
        {
            throw ex;
        }
    }

    public void SetDigitalValue ( int idx, int value )
    {
        try
        {
            digital [ idx ] = value;
        }
        catch ( IndexOutOfRangeException ex )
        {
            throw ex;
        }
    }

    #endregion

    public BITalinoFrame ( )
    { }

    public override string ToString ( )
    {
        return "CRC " + CRC +
            " SEQ " + Sequence +
            " Analog values " + String.Join ( ";", new List<double> ( analog ).ConvertAll ( i => i.ToString ( ) ).ToArray ( ) ) +
            " Digital values " + String.Join ( ";", new List<int> ( digital ).ConvertAll ( i => i.ToString ( ) ).ToArray ( ) );
    }
}
