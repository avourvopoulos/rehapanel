using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
//using System.Diagnostics;

public class RawEEG : MonoBehaviour {
	
	EmoEngine engine; // Access to the EDK is via the EmoEngine 
    int userID = -1; // userID is used to uniquely identify a user's headset
	
	public double AF3;
	public double AF4;
	public double F3;
	public double F4;	
	public double F7;	
	public double F8;
	public double FC5;	
	public double FC6;
	public double T7;
	public double T8;	
	public double P7;
	public double P8;
	public double O1;	
	public double O2;

    
	void Start()
    {
//		engine = EmoEngine.Instance;
//        engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded);
//        engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);
//        engine.Connect();
	}
	
	
	void Update () 
    {
        
	}
	
	
	void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
        Run();
    //    Thread.Sleep(100);
    }



  void engine_UserAdded(object sender, EmoEngineEventArgs e)
    {
        userID = (int)e.userId;

        // enable data acquisition for this user
        engine.DataAcquisitionEnable(e.userId, true);

        // ask for up to 1 second of buffered data
        engine.EE_DataSetBufferSizeInSec(1);
    }	
  
    
    
     void Run()
    {
        // Handle any waiting events
        engine.ProcessEvents();
		
        // If the user has not yet connected, do not proceed
        if ((int)userID == -1)
            return;

        Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);
		

        if (data == null)
        {
            return;
        }

        int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
		Debug.Log("bufferSize: "+ _bufferSize);
        
		
        for (int i = 0; i < _bufferSize; i++)
        {
            // now write the data
            foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
			{
				Debug.Log(data[channel][i]);
			}

				
//			  switch(data[channel][i].ToString())       
//		      {         
//		         case "AF3":   
//		            AF3 = data[channel][i];
//		            break;                  
//		         case "AF4":            
//		            AF4 = data[channel][i];
//		            break;           
//		         case "F3":            
//		            F3 = data[channel][i];
//		           break;
//				 case "F4":            
//		            F4 = data[channel][i];
//		           break;
//				 case "F7":   
//		            F7 = data[channel][i];
//		            break;                  
//		         case "F8":            
//		            F8 = data[channel][i];
//		            break;           
//		         case "FC5":            
//		            FC5 = data[channel][i];
//		           break;
//				 case "FC6":            
//		            FC6 = data[channel][i];
//		           break;
//				 case "T7":            
//		            T7 = data[channel][i];
//		           break;
//				 case "T8":   
//		            T8 = data[channel][i];
//		            break;                  
//		         case "P7":            
//		            P7 = data[channel][i];
//		            break;           
//		         case "P8":            
//		            P8 = data[channel][i];
//		           break;
//				 case "O1":            
//		            O1 = data[channel][i];
//		           break;
//				 case "O2":            
//		            O2 = data[channel][i];
//		           break;		
//		         default:            
//		                        
//		            break;      
//		       }//switch	
			 

        }//for
     

    }//run()


	
	
	
}
