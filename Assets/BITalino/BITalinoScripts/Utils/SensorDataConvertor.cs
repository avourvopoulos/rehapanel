// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;

/**
 * Shamefully copied from the BITalino Java SDK ... :)
 * Also... http://forum.bitalino.com/viewtopic.php?f=12&t=128
 * 
 * This class holds methods for converting/scaling raw data from BITalino
 * included sensors to human-readable data.
 */

public static class SensorDataConvertor {

    private const double VCC = 3.3; // volts
    private const double Cm = 194.0;
    private const double CM = 276.0;
    //n = 10

    /// <summary>
    /// Convert the raw data from the EMG sensor in V
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleEMG_V(double raw)
    {
        //EMGV = (EMGB * Vcc / (2^n - 1) - Vcc / 2) / GEMG
        //EMGmV = EMGV * 1000
        return (raw * VCC / 1023 - 1.5) / 1000;
    }

    /// <summary>
    /// Convert the raw data from the EMG sensor in mV
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleEMG_mV(double raw)
    {
        //EMGV = (EMGB * Vcc / (2^n - 1) - Vcc / 2) / GEMG
        //EMGmV = EMGV * 1000
        return (ScaleEMG_V(raw)) * 1000;
    }

    /// <summary>
    /// Convert the raw data from the ECG sensor in V
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleECG_V(double raw)
    {
        //ECGV = (ECGB * Vcc / (2^n - 1) - Vcc / 2) / GECG
        //ECGmV = ECGV * 1000
        return (raw * VCC / 1023 - 1.5) / 1100;
    }

    /// <summary>
    /// Convert the raw data from the ECG sensor in mV
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleECG_mV(double raw)
    {
        //ECGV = (ECGB * Vcc / (2^n - 1) - Vcc / 2) / GECG
        //ECGmV = ECGV * 1000
        return (ScaleECG_V(raw)) * 1000;
    }

    /// <summary>
    /// Convert the raw data from the EDA sensor in µS
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleEDA(double raw)
    {
        //RMOhm = 1 - EDAB / ( 2^n - 1)
        //EDAµS = 1 / RMOhm
        return 1 / (1 - raw / 1023);
    }

    /// <summary>
    /// Convert the raw data from the Acceleration sensor in G
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleACC(double raw)
    {
        //ACCg = 2 * ((ACCB - Cm) / (CM - Cm)) - 1
        return 2.0 * ((raw - Cm) / (CM - Cm)) - 1.0;
    }

    /// <summary>
    /// Convert the raw data from the photo-sensor sensor in %
    /// </summary>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleLUX(double raw)
    {
        //LUX% = 100 * (LUXB / (2^n - 1))
        return 100 * (raw / 1023.0);
    }

    /// <summary>
    /// Convert the raw data from the Batterie sensor
    /// </summary>
    /// <remarks>Based on the formula to convert it in volt</remarks>
    /// <param name="raw">Data converted</param>
    /// <returns>Result of the convertion</returns>
    public static double ScaleBATT(double raw)
    {
        //V = (raw * Vcc / (2^n - 1) - Vcc / 2)
        return (raw * VCC / 1023 - 1.5);
    }

}
