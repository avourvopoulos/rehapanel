using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;

public class MyomoFunctions : MonoBehaviour {
	
	//User Initiated Commands
	
	/*
	 * The Get Serial Number Command returns the 8 digit serial number of the Device.
	 * */	
	public static string GetSerialNumber()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$t\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","t","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		
			return output[0];
	}
	
	/*
	 * The Set Serial Number Command allows the 8 digit serial number of the Device to be modified. 
	 * Only numerical characters can be used for a valid serial number and it must be eight digits long. 
	 * The Serial Number is stored in non-volatile Memory.
	 * */
//	public static void SetSerialNumber(int serial){}
	
	/*
	 * The Get Arm command indicates weather the firmware is configured for right arm or left arm operation.
	 * The Arm Configuration is stored in non-volatile Memory.
	 * Returns: 1=Right, 2=Left
	 * */
	public static string GetArm()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$w\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","w","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//indicate arm based in return value	
		string arm="";
			if(output[0]=="1")
			{arm="Right";}
			else if(output[0]=="2")
			{arm="Left";}
			else {arm="Not_Set";}
		
			return arm;
	} 
	
//	public static void SetArm(){}			
//	public static void GetRTC(){} 
//	public static void SetRTC(){} 			
//	public static void SetMotorLimits(){}			
//	public static void ResetDefaultValues(){} 
				
	/*
	 * The Get Battery Level command returns the current Battery Level Voltage.
	 * The Integer returned by the device needs to be scaled by 0.0179 to determine the Battery Level in units of Volts.
	 * Where X can range from 0 to 1023 and Voltage = X * .0179 Volts.
	 * */
	public static double GetBatteryLevel()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$g\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","g","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//convert to voltage
		double voltage = (double.Parse(output[0])) * 0.0179;

			return voltage;
	}
	
//	public static void GetCurrentLevel(){} 
//	public static void GetThermistorLevel(){} 			
//	public static void GetLEDColor(){}
//	public static void SetLEDColor(){} 			
//	public static void GetRemoteMode(){} 
	
	/*
	 * The Set Remote Mode command modifies the remote mode setting on the device.
	 * The remote mode setting affects what information the device sends over the Bluetooth connection.
	 * */
	public static string SetRemoteModeOn()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$L 1\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","L","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			return output[0];
	} //always on
				
//	public static void EnableBluetoothLogging(){} 
//	public static void DisableBluetoothLogging(){}
	
	/*
	 * The Get Motor Encoder Value command returns
	 * the position of the encoder relative to the minimum and goes from 0 to 240.
	 * */
	public static int GetMotorEncoderValue()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$b\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","b","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int encvalue = int.Parse(output[0]);
		
			return encvalue;
	}
	
	/*
	 * The Get Upper Limit on ROM range command returns the max limit on Flexion.
	 * */
	public static int GetUpperLimitonROMrange()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$Z\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","Z","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int rom = int.Parse(output[0]);
		
			return rom;
	} 
	
	/*
	 * The Get EMG Calibration command returns the Calibration values (from 0 to 1023)
	 * for both the Bicep and Tricep EMG sensors.
	 * */
	public static int[] GetEMGCalibration()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$e\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","e","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int array
		int[] e_array = new int[2]; 
		e_array[0] = int.Parse(output[0]);
		e_array[1] = int.Parse(output[1]);
		
			return e_array;
	} 
				
//	public static void ResetEMGCalibration(){} 
	
	/*
	 * The Get EMG Command returns the Average EMG Signal for both the Tricep and Bicep EMG signals.
	 * Where E1 and E2 can range from 0 to 2222222. 
	 * */
	public static int[] GetEMG()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$d\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","d","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int array
		int[] emg = new int[2]; 
		emg[0] = int.Parse(output[0]);//tricep
		emg[1] = int.Parse(output[1]);//bicep
		
			return emg;
	}
				
//	public static void SetMode(){} 
	
	/*
	 * The Get Mode command returns the current operational mode of the device. 
	 * */
	public static string GetMode()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$f\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","f","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//indicate mode based in return value	
			string mode="";
			if(output[0]=="1")
			{mode="Clockwise";}
			else if(output[0]=="2")
			{mode="Counterclockwiselockwise";}
			else if(output[0]=="3")
			{mode="Dual";}
			else{mode="OFF";}
		
			return mode;
	} 
	
	/*
	 * The Set Clockwise Assist Level command Modifies the 
	 * current clockwise assist level to one of 4 predefined settings:
	 * 0:OFF, 1:Low, 2:Medium, 3:High and returns a bool if the level was applied.
	 * */
	public static bool SetBicepAssistLevel(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$1 "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			
		if(output[0]=="OK")
			return true;
		else
			return false;
	} 
	
	
	/*
	 * The Set Counter Clockwise Assist Level command Changes the current
	 * Counter Clockwise assist level to one of 4 predefined settings:
	 * 0:OFF, 1:Low, 2:Medium, 3:High nd returns a bool if the level was applied.
	 * */
	public static bool SetTricepAssistLevel(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$2 "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			
		if(output[0]=="OK")
			return true;
		else
			return false;
	} 
			
	/*
	 * The Get Default Bicep Mode Flexion command returns an integer from 1-20.
	 * The specified default Flexion Assist Level for Bicep Mode.
	 * Levels -> 1:Low, 2:Medium, 3:High 
	 * */
	public static int GetDefaultBicepModeFlexion(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$F "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","F","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int bicmode = int.Parse(output[0]);
		
			return bicmode;
	}
	
	
	/*
	 * The Set Default Bicep Mode Flexion command modifies the Default Flexion Assist Levels for Bicep Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * Send: $E X Y where X is one of the Programmable assist Levels (1-3) and Y is an integer from 1-20.
	 * Returns true if the mode is set.
	 * */
	public static bool SetDefaultBicepmodeFlexion(int level, int flexion)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$D "+level+" "+flexion+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="DOK")
			return true;
		else
			return false;
			
	}
				
	
	/*
	 * The Get Default Bicep Mode Extension command returns the specified
	 * default Extension Assist Level for Bicep Mode.
	 * */
	public static int GetDefaultBicepModeExtension(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$G "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","F","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int bicmode = int.Parse(output[0]);
		
			return bicmode;
	}
	
	
	/*
	 * The Set Default Bicep Mode Extension command modifies the Default Extension Assist Levels for Bicep Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * Send: $E X Y where X is one of the Programmable assist Levels (1-3) and Y is an integer from 1-20.
	 * Returns true if the mode is set.
	 * */
	public static bool SetDefaultBicepmodeExtension(int level, int extension)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$E "+level+" "+extension+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="EOK")
			return true;
		else
			return false;
	} 
	
	/*
	 * The Set Default Tricep Mode Flexion command modifies the Default Flexion Assist Levels for Tricep Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * */
	public static bool SetDefaultTricepModeFlexion(int level, int flexion)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$H "+level+" "+flexion+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="HOK")
			return true;
		else
			return false;	
	} 
	
	/*
	 * The Get Default Tricep Mode Extension command returns 
	 * the specified default Extension Assist Level for Tricep Mode.
	 * */
	public static int GetDefaultTricepModeExtension(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$K "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","K","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int tricmode = int.Parse(output[0]);
		
			return tricmode;
	} 
	
	/*
	 * The Set Default Tricep Mode Extension command modifies the Default Extension Assist Levels for Tricep Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * Send: $I X Y where X is one of the Programmable assist Levels (1-3) and Y is an integer from 1-20.
	 * Returns true if the mode is set.
	 */
	public static bool SetDefaultTricepModeExtension(int level, int extension)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$I "+level+" "+extension+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="IOK")
			return true;
		else
			return false;
	}
	
	/*
	 * The Get Default Dual Mode Flexion command returns the specified default Flexion Assist Level for Dual Mode.
	 * Send: X where X is one of the Programmable assist Levels (1-3)
	 * Returns an integer from 1-20 
	 * */
	public static int GetDefaultDualModeFlexion(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$m "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","m","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int mode = int.Parse(output[0]);
		
			return mode;
	} 
	
	/*
	 * The Set Default Dual Mode Flexion command modifies the Default Flexion Assist Levels for Dual Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * Send: X where X is one of the Programmable assist Levels (1-3)
	 * Returns an integer from 1-20
	 * */
	public static bool SetDefaultDualModeFlexion(int level, int flexion)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$k "+level+" "+flexion+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="kOK")
			return true;
		else
			return false;
	} 
				
	/*
	 * The Get Default Dual Mode Extension command returns the specified default Extension Assist Level for Dual Mode.
	 * */
	public static int GetDefaultDualModeExtension(int level)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$n "+level+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","n","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int mode = int.Parse(output[0]);
		
			return mode;
	} 
	
	
	/*
	 * The Set Default Dual Mode Extension command modifies the Default Extension Assist Levels for Dual Mode.
	 * The assist levels set are stored in non-volatile memory.
	 * Send: X where X is one of the Programmable assist Levels (1-3)
	 * Returns an integer from 1-20
	 * */
	public static bool SetDefaultDualModeExtension(int level, int extension)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$l "+level+" "+extension+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="lOK")
			return true;
		else
			return false;
	} 
	
	/*
	 * The Get Bicep Flexion command returns the current flexion assist value for Bicep Mode or Dual Mode.
	 * Returns an integer from 1-20. 
	 * */
	public static int GetBicepFlexion()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$P\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","P","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int val = int.Parse(output[0]);
		
			return val;
	}
	
	/*
	 * The Set Bicep Flexion command Modifies the current bicep flexion assist level for Bicep Mode and Dual Mode to the specified value.
	 * The Flexion assist level LEDs will be updated according to the value specified.
	 * Returns true when set is OK. 
	 * */
	public static bool SetBicepFlexion(int flexion)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$N "+flexion+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="NOK")
			return true;
		else
			return false;
	}
	
	/*
	 * The Get Bicep Mode command returns the current extension assist value for Bicep Mode.
	 * Returns an integer from 1-20.
	 * */
	public static int GetBicepExtension()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$Q\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","Q","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int val = int.Parse(output[0]);
		
			return val;	
	} 
	
	/*
	 * The Set Bicep Extension command Modifies the current extension assist level in for Bicep Mode to the specified value.
	 * The Extension assist level LEDs will be updated according to the value specified.
	 * input int from 1-20, returns true if set is OK
	 * */
	public static bool SetBicepExtension(int extension)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$O "+extension+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="OOK")
			return true;
		else
			return false;
	}
				
	
	/*
	 * The Get Tricep Flexion command returns the current flexion assist value for Tricep Mode.
	 * */
	public static int GetTricepFlexion()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$T\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","T","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int val = int.Parse(output[0]);
		
			return val;
	} 
	
	
	public static bool SetTricepFlexion(int flexion)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$R "+flexion+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="ROK")
			return true;
		else
			return false;
	}
	
	
	public static int GetTricepExtension()
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$U\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$","OK"," ","U","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//string to int
		int val = int.Parse(output[0]);
		
			return val;
	} 
	
	
	public static bool SetTricepExtension(int extension)
	{
		//send instruction to myomo
		MyomoConnection.sp.Write("$S "+extension+"\r\n");
		//read incoming data
		string input = MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine()+MyomoConnection.sp.ReadLine();
		//split and decompose message
		string[] separators = {"$"," ","\n"};
		//reconstruct msg
		string[] output = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//check if is set
		string mode = output[0]+output[1];
		if(mode=="SOK")
			return true;
		else
			return false;
	}
	
	
	//Event Initiated Commands
//	public static void CWbuttonPressed(){}
//	public static void CCWbuttonPressed(){}
//	public static void ModeButtonPressed(){}
	
	
	
}// end of MyomoFunctions
