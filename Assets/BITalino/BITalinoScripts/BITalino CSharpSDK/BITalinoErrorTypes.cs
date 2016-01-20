// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;

class BITalinoErrorTypes
{
    public static readonly BITalinoErrorTypes COM_ACCESS_NOT_ALLOWED    = new BITalinoErrorTypes ( 0, "The access to the port COM is not allowed." );
    public static readonly BITalinoErrorTypes COM_ALREADY_OPEN          = new BITalinoErrorTypes ( 1, "The COM port is already open." );
    public static readonly BITalinoErrorTypes COM_NOT_OPEN              = new BITalinoErrorTypes ( 2, "The COM port is not open." );
    public static readonly BITalinoErrorTypes COM_CONNECTION_ERROR      = new BITalinoErrorTypes ( 3, "Bluetooth device not connected OR BITalino unreachable." );
    public static readonly BITalinoErrorTypes ANALOG_CHANNELS_NOT_VALID = new BITalinoErrorTypes ( 4, "Analog Channel value must be set as 0, 1, 2, 3, 4 or 5." );
    public static readonly BITalinoErrorTypes SAMPLING_RATE_NOT_DEFINED = new BITalinoErrorTypes ( 5, "Sampling Rate value must be set as 1000, 100, 10 or 1." );
    public static readonly BITalinoErrorTypes INVALID_ARGUMENT          = new BITalinoErrorTypes ( 6, "Invalid parameter(s)." );
    public static readonly BITalinoErrorTypes TIME_OUT                  = new BITalinoErrorTypes ( 7, "The operation timed out." );
    public static readonly BITalinoErrorTypes INCORRECT_DECODE          = new BITalinoErrorTypes ( 8, "Incorrect data to be decoded." );

    #region GETTER/SETTER

    public int Value { get; private set; }

    public string Message { get; private set; }

    #endregion

    private BITalinoErrorTypes ( int value, String message )
    {
        Value = value;

        Message = message;
    }
}