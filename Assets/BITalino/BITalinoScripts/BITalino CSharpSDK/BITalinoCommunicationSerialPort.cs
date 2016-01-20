// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.IO.Ports;
using System.Text;

public sealed class BITalinoCommunicationSerialPort : IBITalinoCommunication
{
    #region GETTER/SETTER

    SerialPort SerialPort { get; set; }

    #endregion

    public BITalinoCommunicationSerialPort ( SerialPort serialPort )
    {
        SerialPort = serialPort;
    }

    public void Write ( int data )
    {
        try
        {
            byte[] buffer = BitConverter.GetBytes ( data );

            SerialPort.Write ( buffer, 0, 1 );

            SerialPort.DiscardOutBuffer ( );
        }
        catch ( Exception ex )
        {
            if ( ex is InvalidOperationException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_NOT_OPEN );
            }
            else if ( ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is ArgumentException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.INVALID_ARGUMENT );
            }
            else if ( ex is TimeoutException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.TIME_OUT );
            }
            else
            {
                throw ex;
            }
        }
    }

    /// <summary>Read frames from the device. </summary>
    /// <param name="nbBytes">Size of a frame in byte. Calculate from <see>CalcNbBytes()</see>.</param>
    /// <param name="nbAnalogChannels">Number of channel read.</param>
    /// <param name="nbSamples">Number of frames read.</param>
    /// <returns>Return a table of frame that contain the frames read.</returns>
    /// <exception cref="BITalinoException"/>
    public BITalinoFrame [ ] ReadFrames ( int nbBytes, int nbAnalogChannels, int nbFrames )
    {
        try
        {
            BITalinoFrame[] frames = new BITalinoFrame [ nbFrames ];

            byte[] buffer = new byte [ nbBytes ];

            byte[] bTemp = new byte [ 1 ];

            int sampleCounter = 0;

            BITalinoFrame decodedFrame;
            while ( sampleCounter < nbFrames )
            {
                buffer = new byte [ nbBytes ];

                for ( int i = 0; i < nbBytes; i++ )
                {
                    SerialPort.Read ( bTemp, 0, 1 );

                    buffer [ i ] = bTemp [ 0 ];

                    bTemp = new byte [ 1 ];
                }

                decodedFrame = BITalinoFrameDecoder.Decode ( buffer, nbBytes, nbAnalogChannels );

                if ( decodedFrame.Sequence == -1 )
                {
                    while ( decodedFrame.Sequence == -1 )
                    {
                        SerialPort.Read ( bTemp, 0, 1 );

                        for ( int j = nbBytes - 2; j >= 0; j-- )
                        {
                            buffer [ j + 1 ] = buffer [ j ];
                        }

                        buffer [ 0 ] = bTemp [ 0 ];

                        decodedFrame = BITalinoFrameDecoder.Decode ( buffer, nbBytes, nbAnalogChannels );
                    }

                    frames [ sampleCounter ] = decodedFrame;
                }
                else
                {
                    frames [ sampleCounter ] = decodedFrame;
                }

                sampleCounter++;
            }

            return frames;
        }
        catch ( Exception ex )
        {
            if ( ex is InvalidOperationException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_NOT_OPEN );
            }
            else if ( ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is ArgumentException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.INVALID_ARGUMENT );
            }
            else if ( ex is TimeoutException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.TIME_OUT );
            }
            else
            {
                throw ex;
            }
        }
    }

    public string ReadVersion ( )
    {
        try
        {
            string message = "";

            while ( message.Length == 0 || message [ message.Length - 1 ] != '\n' )
            {
                byte[] buffer = new byte [ SerialPort.ReadBufferSize ];

                int bytesRead = SerialPort.Read ( buffer, 0, buffer.Length );

                message += Encoding.ASCII.GetString ( buffer, 0, bytesRead );
            }

            return message;
        }
        catch ( Exception ex )
        {
            if ( ex is InvalidOperationException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_NOT_OPEN );
            }
            else if ( ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is ArgumentException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.INVALID_ARGUMENT );
            }
            else if ( ex is TimeoutException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.TIME_OUT );
            }
            else
            {
                throw ex;
            }
        }
    }

    public void Close ( )
    {
        try
        {
            SerialPort.Close ( );
        }
        catch
        {
            throw new BITalinoException ( BITalinoErrorTypes.COM_CONNECTION_ERROR );
        }
    }

    public void Open ( )
    {
        try
        {
            SerialPort.Open ( );
        }
        catch ( Exception ex )
        {
            if ( ex is UnauthorizedAccessException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_ACCESS_NOT_ALLOWED );
            }
            else if ( ex is InvalidOperationException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_ALREADY_OPEN );
            }
            else if ( ex is System.IO.IOException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.COM_CONNECTION_ERROR );
            }
            else if ( ex is ArgumentOutOfRangeException || ex is ArgumentException )
            {
                throw new BITalinoException ( BITalinoErrorTypes.INVALID_ARGUMENT );
            }
            else
            {
                throw ex;
            }
        }
    }
}
