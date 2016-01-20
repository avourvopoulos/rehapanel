// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;

public sealed class BITalinoFrameDecoder
{
    public static BITalinoFrame Decode ( byte [ ] buffer, int nbBytes, int nbAnalogChannels )
    {
        try
        {
            BITalinoFrame decodeFrame = new BITalinoFrame ( );

            int j = ( nbBytes - 1 ), CRC = 0, x0 = 0, x1 = 0, x2 = 0, x3 = 0, outs = 0, inp = 0;
            CRC = ( buffer [ j - 0 ] & 0x0F ) & 0xFF;

            // check CRC
            for ( int bytes = 0; bytes < nbBytes; bytes++ )
            {
                for ( int bit = 7; bit > -1; bit-- )
                {
                    inp = ( buffer [ bytes ] ) >> bit & 0x01;
                    if ( bytes == ( nbBytes - 1 ) && bit < 4 )
                    {
                        inp = 0;
                    }
                    outs = x3;
                    x3 = x2;
                    x2 = x1;
                    x1 = outs ^ x0;
                    x0 = inp ^ outs;
                }
            }
            int val = ( ( x3 << 3 ) | ( x2 << 2 ) | ( x1 << 1 ) | x0 );
            if ( CRC == val )
            {
                /*parse frames*/
                decodeFrame.Sequence = ( short ) ( ( buffer [ j - 0 ] & 0xF0 ) >> 4 ) & 0xf;
                decodeFrame.SetDigitalValue ( 0, ( short ) ( ( buffer [ j - 1 ] >> 7 ) & 0x01 ) );
                decodeFrame.SetDigitalValue ( 1, ( short ) ( ( buffer [ j - 1 ] >> 6 ) & 0x01 ) );
                decodeFrame.SetDigitalValue ( 2, ( short ) ( ( buffer [ j - 1 ] >> 5 ) & 0x01 ) );
                decodeFrame.SetDigitalValue ( 3, ( short ) ( ( buffer [ j - 1 ] >> 4 ) & 0x01 ) );

                switch ( nbAnalogChannels )
                {
                    case 6:
                        decodeFrame.SetAnalogValue ( 5, ( short ) ( ( buffer [ j - 7 ] & 0x3F ) ) * ( 1023.0 / 63.0 ) );
                        goto case 5;
                    case 5:
                        decodeFrame.SetAnalogValue ( 4, ( short ) ( ( ( ( buffer [ j - 6 ] & 0x0F ) << 2 ) | ( ( buffer [ j - 7 ] & 0xC0 ) >> 6 ) ) & 0x3F ) * ( 1023.0 / 63.0 ) );
                        goto case 4;
                    case 4:
                        decodeFrame.SetAnalogValue ( 3, ( short ) ( ( ( ( buffer [ j - 5 ] & 0x3F ) << 4 ) | ( ( buffer [ j - 6 ] & 0xf0 ) >> 4 ) ) & 0x3FF ) );
                        goto case 3;
                    case 3:
                        decodeFrame.SetAnalogValue ( 2, ( short ) ( ( ( ( buffer [ j - 4 ] & 0xFF ) << 2 ) | ( ( ( buffer [ j - 5 ] & 0xC0 ) >> 6 ) ) ) & 0x3FF ) );
                        goto case 2;
                    case 2:
                        decodeFrame.SetAnalogValue ( 1, ( short ) ( ( ( ( buffer [ j - 2 ] & 0x03 ) << 8 ) | ( buffer [ j - 3 ] ) & 0xFF ) & 0x3FF ) );
                        goto case 1;
                    case 1:
                        decodeFrame.SetAnalogValue ( 0, ( short ) ( ( ( ( buffer [ j - 1 ] & 0x0F ) << 6 ) | ( ( buffer [ j - 2 ] & 0XFC ) >> 2 ) ) & 0x3FF ) );
                        break;
                }
            }
            else
            {
                decodeFrame = new BITalinoFrame ( );
                decodeFrame.Sequence = -1;
            }

            return decodeFrame;
        }
        catch
        {
            throw new BITalinoException ( BITalinoErrorTypes.INCORRECT_DECODE );
        }
    }
}
