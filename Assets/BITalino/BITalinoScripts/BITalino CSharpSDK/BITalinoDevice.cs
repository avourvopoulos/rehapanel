// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class BITalinoDevice
{
    private int[] analogChannels;

    private int samplingRate;

    private int nbBytes;

    private int timeSleep = 200;

    private IBITalinoCommunication bitalinoCommunication;

    #region GETTER/SETTER

    public int [ ] AnalogChannels
    {
        get { return analogChannels; }
        set
        {
            if ( CheckAnalogChannels ( value ) )
            {
                analogChannels = value;
            }
        }
    }

    public int SamplingRate
    {
        get { return samplingRate; }
        set
        {
            if ( CheckSamplingRate ( value ) )
            {
                samplingRate = value;
            }
        }
    }
    #endregion

    public BITalinoDevice ( IBITalinoCommunication bitalinoCommunication, int [ ] analogChannels, int samplingRate )
    {
        if ( CheckBitalinoCommunication ( bitalinoCommunication ) )
        {
            this.bitalinoCommunication = bitalinoCommunication;
        }

        if ( CheckAnalogChannels ( analogChannels ) )
        {
            this.analogChannels = analogChannels;
        }

        if ( CheckSamplingRate ( samplingRate ) )
        {
            this.samplingRate = samplingRate;
        }

        CalcNbBytes ( );
    }

    #region Check and Format and Calc

    private bool CheckBitalinoCommunication ( IBITalinoCommunication bitalinoCommunication )
    {
        if ( bitalinoCommunication == null )
        {
            throw new BITalinoException ( BITalinoErrorTypes.INVALID_ARGUMENT );
        }

        return true;
    }

    private bool CheckAnalogChannels ( int [ ] analogChannels )
    {
        if ( analogChannels.Length > 6 | analogChannels.Length == 0 )
        {
            throw new Exception ( "Length analogChannels" );
        }

        foreach ( int i in analogChannels )
        {
            if ( i < 0 | i > 5 )
            {
                throw new BITalinoException ( BITalinoErrorTypes.ANALOG_CHANNELS_NOT_VALID );
            }
        }

        return true;
    }

    private bool CheckSamplingRate ( int samplingRate )
    {
        return (
            samplingRate == 1000 ||
            samplingRate == 100 ||
            samplingRate == 10 ||
            samplingRate == 1
            );
    }

    private int FormatSamplingRate ( )
    {
        int bit = 0x0;

        switch ( samplingRate )
        {
            case 1000:
                bit = 0x3;
                break;
            case 100:
                bit = 0x2;
                break;
            case 10:
                bit = 0x1;
                break;
            case 1:
                bit = 0x0;
                break;
            default:
                throw new BITalinoException ( BITalinoErrorTypes.SAMPLING_RATE_NOT_DEFINED );
        }

        bit = ( bit << 6 ) | 0x03;

        return bit;
    }

    private int FormatAnalogChannels ( )
    {
        int bit = 1;

        foreach ( int i in analogChannels )
        {
            if ( i < 0 | i > 5 )
            {
                throw new BITalinoException ( BITalinoErrorTypes.ANALOG_CHANNELS_NOT_VALID );
            }
            else
            {
                bit = bit | 1 << ( 2 + i );
            }
        }

        return bit;
    }

    public void CalcNbBytes ( )
    {
        int numberChannels = analogChannels.Length;

        if ( numberChannels <= 4 )
        {
            nbBytes = ( int ) Math.Ceiling ( ( 12.0 + 10.0 * numberChannels ) / 8.0 );
        }
        else
        {
            nbBytes = ( int ) Math.Ceiling ( ( 52.0 + 6.0 * ( numberChannels - 4 ) ) / 8.0 );
        }
    }

    #endregion

    public void Connection ( )
    {
        bitalinoCommunication.Open ( );

        Thread.Sleep ( timeSleep );

        int bitSamplingRate = FormatSamplingRate ( );

        bitalinoCommunication.Write ( bitSamplingRate );

    }

    public void Deconnection ( )
    {
        bitalinoCommunication.Close ( );
    }

    public void StartAcquisition ( )
    {
        int bitAnalogChannels = FormatAnalogChannels ( );

        CalcNbBytes ( );

        bitalinoCommunication.Write ( bitAnalogChannels );
    }

    public void StopAcquisition ( )
    {
        bitalinoCommunication.Write ( 0 );
    }

    public string GetVersion ( )
    {
        bitalinoCommunication.Write ( 7 );

        return bitalinoCommunication.ReadVersion ( );
    }

    public BITalinoFrame [ ] ReadFrames ( int nbFrames )
    {
        return bitalinoCommunication.ReadFrames ( nbBytes, analogChannels.Length, nbFrames );
    }
}
