using UnityEngine;
using System.Collections;

public class HeadsetGUI : MonoBehaviour
{

    // Headset node position
    // 0: AF3 3
    // 1: AF4  16
    // 2: F3    5
    // 3: F4   14
    // 4: F7    4
    // 5: F8    15
    // 6: FC5   6
    // 7: FC6  13
    // 8: T7    7
    // 9: T8    12
    // 10: CMS   0
    // 11: DLR   1
    // 12: P7     8
    // 13: P8     11
    // 14: O1     9
    // 15: O2     10
    //typedef enum EE_InputChannels_enum 
    //{
    //            EE_CHAN_CMS = 0, EE_CHAN_DRL 1, EE_CHAN_FP1 2 , EE_CHAN_AF3 3, EE_CHAN_F7 4,  
    //			EE_CHAN_F3 5, EE_CHAN_FC5 6 , EE_CHAN_T7 7 , EE_CHAN_P7 8 , EE_CHAN_O1 9,
    //			EE_CHAN_O2, EE_CHAN_P8, EE_CHAN_T8, EE_CHAN_FC6, EE_CHAN_F4,
    //		    EE_CHAN_F8, EE_CHAN_AF4, EE_CHAN_FP2
    //} EE_InputChannels_t;

    //var EpocManager:GameObject; 


    /// scale

     float int_scale = 1.7f;
	 float eeg_scale = 0.7f;
	
    public Rect headArea;
    private Rect head;
    public static int[] node;

    public Texture2D headset;
    public Texture2D redButt;
    public Texture2D blackButt;
    public Texture2D orangeButt;
    public Texture2D yellowButt;
    public Texture2D greenButt;
	public Texture2D backIcon;
    public bool isEnable = true;

    public float hXPos = 0.0f;
    public float hYPos = 0.0f;
	
	//headset position within the box
	int posX = 30; 
	int posY = 50;
	
	int eegposX = 30; 
	int eegposY = 50;
	
	private bool toggle1 = false; //AF3
	private bool toggle2 = false; //AF4
	private bool toggle3 = false; //F3
	private bool toggle4 = false; //F4	
	private bool toggle5 = false; //F7	
	private bool toggle6 = false; //F8
	private bool toggle7 = false; //FC5	
	private bool toggle8 = false; //FC6
	private bool toggle9 = false; //T7
	private bool toggle10 = false; //T8	
	private bool toggle11 = false; //P7
	private bool toggle12 = false; //P8
	private bool toggle13 = false; //O1	
	private bool toggle14 = false; //O2
	
	public static bool toggle15 = false; //GYRO
	int gyrox, gyroy;
	
	private bool toggle16 = false; //CMS
	private bool toggle17 = false; //DLR
	
	private bool toggle18 = true; //show labels
	
	public static bool toggle19 = false; //Affectiv
	public static bool toggle20 = false; //Expressiv
	
	string DeviceName = string.Empty;

    // nodeStatus: 
    // O: Black
    // 1: Red
    // 2: Orange
    // 3: Yellow
    // 4: Green
    Texture2D nodeStatus(int node)
    {
        Texture2D returnButt;
        switch (node)
        {
            case 0:
                returnButt = blackButt;
                break;
            case 1:
                returnButt = redButt;
                break;
            case 2:
                returnButt = orangeButt;
                break;
            case 3:
                returnButt = yellowButt;
                break;
            case 4:
                returnButt = greenButt;
                break;
            default:
                returnButt = blackButt;
                break;
        }
        return returnButt;
    }

    void DisableInfo()
    {
        isEnable = false;
    }

    void EnableInfo()
    {
        isEnable = true;
    }

    void Start()
    {
        int i;
        node = new int[18];
        for (i = 0; i < 18; i++)
        {
            node[i] = 0;
        }

        if (headArea == new Rect(0, 0, 0, 0))
        {
            headArea = new Rect(600, 70, 225 * int_scale, 200 * int_scale);
        }
        if (head == new Rect(0, 0, 0, 0))
        {
            head = new Rect(posX, posY, 200 * int_scale, 200 * int_scale);
        }
    }

    void DrawGUI()
    {
		// Hien headset o giua man hinh
//		headArea.x = (Screen.width - headArea.width) / 2;
//		headArea.y = (Screen.height - headArea.height) / 2;
		

//        headArea.x += hXPos;
//        headArea.y += hYPos;

//        GUI.BeginGroup(headArea);
if(MainGuiControls.EmotivMenu)
{
			
	if (GUI.Button (new Rect (Screen.width/2-100 , 10 ,60,60), backIcon))
		{
			MainGuiControls.EmotivMenu = false;
			MainGuiControls.hideMenus = false;
		}	
			
			GUI.BeginGroup (new Rect (Screen.width / 2 - 200-KinectGUI.gone, (Screen.height/2 - 270), 400, 540));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,400,450), "Headset");
		GUI.color = Color.white;
		
//		GUI.BeginGroup (new Rect (Screen.width / 2 + 320, (Screen.height/2 - 210)*MainGuiControls.emotivmenu, 100, 100));
	//	GUI.Box (new Rect (0,0,100,100), " ");
		
		
        GUI.DrawTexture(new Rect(posX, posY, 200 * int_scale, 200 * int_scale), headset);

        GUI.DrawTexture(new Rect(47 * int_scale+posX, 26 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[3]));
        GUI.DrawTexture(new Rect(130 * int_scale+posX, 26 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[16]));

        GUI.DrawTexture(new Rect(67 * int_scale+posX, 51 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[5]));
        GUI.DrawTexture(new Rect(110 * int_scale+posX, 51 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[14]));

        GUI.DrawTexture(new Rect(18 * int_scale+posX, 53 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[4]));
        GUI.DrawTexture(new Rect(159 * int_scale+posX, 53 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[15]));

        GUI.DrawTexture(new Rect(37 * int_scale+posX, 71 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[5]));
        GUI.DrawTexture(new Rect(141 * int_scale+posX, 71 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[13]));

        // T7 T8
        GUI.DrawTexture(new Rect(8 * int_scale+posX, 93 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[7]));
        GUI.DrawTexture(new Rect(169 * int_scale+posX, 93 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[12]));

        //CMS
        GUI.DrawTexture(new Rect(18 * int_scale+posX, 118 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[0]));
        GUI.DrawTexture(new Rect(159 * int_scale+posX, 118 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[1]));

        GUI.DrawTexture(new Rect(37 * int_scale+posX, 148 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[8]));
        GUI.DrawTexture(new Rect(140 * int_scale+posX, 148 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[11]));

        GUI.DrawTexture(new Rect(64 * int_scale+posX, 172 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[9]));
        GUI.DrawTexture(new Rect(113 * int_scale+posX, 172 * int_scale+posY, 23 * int_scale, 23 * int_scale), nodeStatus(node[10]));
		
		
		toggle18 = GUI.Toggle (new Rect (220, 405, 15, 100), toggle18, "Labels");
		 GUI.Label(new Rect(180, 405, 100, 20), "Labels");
		
		if (toggle18==true)
		{
		GUI.Label(new Rect(47 * int_scale+posX+6, 26 * int_scale+posY+6, 100, 20), "AF3");
		GUI.Label(new Rect(130 * int_scale+posX+6, 26 * int_scale+posY+6, 100, 20), "AF4");
		
		GUI.Label(new Rect(67 * int_scale+posX+10, 51 * int_scale+posY+6 , 100, 20), "F3");
		GUI.Label(new Rect(110 * int_scale+posX+10, 51 * int_scale+posY+6 , 100, 20), "F4");
		
		GUI.Label(new Rect(18 * int_scale+posX+10, 53 * int_scale+posY+6 , 100, 20), "F7");
		GUI.Label(new Rect(159 * int_scale+posX+10, 53 * int_scale+posY+6 , 100, 20), "F8");
		
		GUI.Label(new Rect(37 * int_scale+posX+6, 71 * int_scale+posY+6 , 100, 20), "FC5");
		GUI.Label(new Rect(141 * int_scale+posX+6, 71 * int_scale+posY+6 , 100, 20), "FC6");
		
		GUI.Label(new Rect(8 * int_scale+posX+10, 93 * int_scale+posY+6 , 100, 20), "T7");
		GUI.Label(new Rect(169 * int_scale+posX+10, 93 * int_scale+posY+6 , 100, 20), "T8");
		
		GUI.Label(new Rect(18 * int_scale+posX+6, 118 * int_scale+posY+6 , 100, 20), "CMS");
		GUI.Label(new Rect(159 * int_scale+posX+6, 118 * int_scale+posY+6 , 100, 20), "DLR");
		
		GUI.Label(new Rect(37 * int_scale+posX+10, 148 * int_scale+posY+6 , 100, 20), "P7");
		GUI.Label(new Rect(140 * int_scale+posX+10, 148 * int_scale+posY+6 , 100, 20), "P8");
		
		GUI.Label(new Rect(64 * int_scale+posX+10, 172 * int_scale+posY+6 , 100, 20), "O1");
		GUI.Label(new Rect(113 * int_scale+posX+10, 172 * int_scale+posY+6 , 100, 20), "O2");
		}

        GUI.EndGroup();  
	
		
//GUI.enabled = UDPData.flag;			
		
GUI.BeginGroup (new Rect (Screen.width - 212, (Screen.height/2 - 160), 200, 280));
		GUI.color = Color.yellow;
		GUI.Box (new Rect (0,0,200,280), "Send EEG Data");
		GUI.color = Color.white;
		
		 GUI.DrawTexture(new Rect(eegposX, eegposY, 200 * eeg_scale, 200 * eeg_scale), headset);
		
		toggle1 = GUI.Toggle (new Rect (47 * eeg_scale+eegposX, 26 * eeg_scale+eegposY, 15, 20), toggle1, new GUIContent(" ", "AF3")); 	
		toggle2 = GUI.Toggle (new Rect (130 * eeg_scale+eegposX, 26 * eeg_scale+eegposY, 15, 20), toggle2, new GUIContent(" ", "AF4")); 	
		toggle3 = GUI.Toggle (new Rect (67 * eeg_scale+eegposX, 51 * eeg_scale+eegposY, 15, 20), toggle3, new GUIContent(" ", "F3")); 		
		toggle4 = GUI.Toggle (new Rect (110 * eeg_scale+eegposX, 51 * eeg_scale+eegposY, 15, 20), toggle4, new GUIContent(" ", "F4")); 		
		toggle5 = GUI.Toggle (new Rect (18 * eeg_scale+eegposX, 53 * eeg_scale+eegposY, 15, 20), toggle5, new GUIContent(" ", "F7")); 		
		toggle6 = GUI.Toggle (new Rect (159 * eeg_scale+eegposX, 53 * eeg_scale+eegposY, 15, 20), toggle6, new GUIContent(" ", "F8")); 
		toggle7 = GUI.Toggle (new Rect (37 * eeg_scale+eegposX, 71 * eeg_scale+eegposY, 15, 20), toggle7, new GUIContent(" ", "FC5")); 		
		toggle8 = GUI.Toggle (new Rect (141 * eeg_scale+eegposX, 71 * eeg_scale+eegposY, 15, 20), toggle8, new GUIContent(" ", "FC6")); 	
		toggle9 = GUI.Toggle (new Rect (169 * eeg_scale+eegposX, 93 * eeg_scale+eegposY, 15, 20), toggle9, new GUIContent(" ", "T7"));
		
		toggle16 = GUI.Toggle (new Rect (18 * eeg_scale+eegposX, 118 * eeg_scale+eegposY, 15, 20), toggle16, new GUIContent(" ", "CMS")); 	
		toggle17 = GUI.Toggle (new Rect (159 * eeg_scale+eegposX, 118 * eeg_scale+eegposY, 15, 20), toggle17, new GUIContent(" ", "DLR"));
		
		toggle10 = GUI.Toggle (new Rect (8 * eeg_scale+eegposX, 93 * eeg_scale+eegposY, 15, 20), toggle10, new GUIContent(" ", "T8")); 	
		toggle11 = GUI.Toggle (new Rect (37 * eeg_scale+eegposX, 148 * eeg_scale+eegposY, 15, 20), toggle11, new GUIContent(" ", "P7")); 		
		toggle12 = GUI.Toggle (new Rect (140 * eeg_scale+eegposX, 148 * eeg_scale+eegposY, 15, 20), toggle12, new GUIContent(" ", "P8")); 
		toggle13 = GUI.Toggle (new Rect (64 * eeg_scale+eegposX, 172 * eeg_scale+eegposY, 15, 20), toggle13, new GUIContent(" ", "O1")); 		
		toggle14 = GUI.Toggle (new Rect (113 * eeg_scale+eegposX, 172 * eeg_scale+eegposY, 15, 20), toggle14, new GUIContent(" ", "O2")); 
		
		GUI.color = Color.white;
		toggle15 = GUI.Toggle (new Rect (120, 205, 80, 20), toggle15, " "); 
		GUI.Label(new Rect(135, 205, 100, 80), "GYRO XY");
		
		toggle19 = GUI.Toggle (new Rect (5, 190, 80, 20), toggle19, " "); 
		GUI.Label(new Rect(20, 190, 100, 80), "Affectiv");
		toggle20 = GUI.Toggle (new Rect (5, 210, 80, 20), toggle20, " "); 
		GUI.Label(new Rect(20, 210, 100, 80), "Expressiv");
		
		GUI.color = Color.white;//hovering text color
		GUI.Label(new Rect(90, 220, 100, 40), GUI.tooltip);
		
		GUI.color = Color.white;	
		if(GUI.Button(new Rect(110,250,65,20), "Clear All"))
		{
			toggle1 = false; //
			toggle2 = false;
			toggle3 = false;			
			toggle4 = false;
			
			toggle5 = false;		
			toggle6 = false;
			toggle7 = false;
			
			toggle8 = false;
			toggle9 = false;
			toggle10 = false;
			
			toggle11 = false;			
			toggle12 = false; 
			toggle13 = false;
			
			toggle14 = false;
			toggle15 = false;
			toggle16 = false;
			toggle17 = false;
			
			toggle19 = false;
			toggle20 = false;
		}
		
		GUI.color = Color.white;	
		if(GUI.Button(new Rect(25,250,65,20), "Send All"))
		{
			toggle1 = true; //
			toggle2 = true;
			toggle3 = true;			
			toggle4 = true;
			
			toggle5 = true;		
			toggle6 = true;
			toggle7 = true;
			
			toggle8 = true;
			toggle9 = true;
			toggle10 = true;
			
			toggle11 = true;			
			toggle12 = true; 
			toggle13 = true;
			
			toggle14 = true;
			toggle15 = true;
			toggle16 = true;
			toggle17 = true;
			
			toggle19 = true;
			toggle20 = true;
		}
		
//GUI.enabled = true;
		
		GUI.EndGroup();
			
	}//if emotiv menu		
	
    }

    void OnGUI()
    {
        if (isEnable) DrawGUI();
    }
	
	void FixedUpdate()
	{
		//select name of device
		if(DevicesLists.device)
		{
			DeviceName = DevicesLists.newDevice;
		}
		else
		{
			DeviceName = "emotiv";
		}
	}

    void Update()
    {
		
if(EmoEngineInst.IsStarted==true)
	{		
        if (EmoUserManagement.numUser == 0)
        {
            for (int i = 0; i < EmoEngineInst.nChan; i++)
            {
                EmoEngineInst.Cq[i] = 0;
                node[i] = EmoEngineInst.Cq[i];
            }
        }
        else
        {
            for (int i = 0; i < EmoEngineInst.nChan; i++)
            {
                node[i] = EmoEngineInst.Cq[i];
            }
        }
			
		gyrox=-EmoGyroData.GyroX;
		gyroy=-EmoGyroData.GyroY;
			
		sendEEG();//send EEG data via UDP
			
	  }//if EmoEngineInst.IsStarted
    }
	
	
	void sendEEG()
	{
	//	send UDP
	//	[$]<data type> , [$$]<device> , [$$]<item> , <transformation> , <param_1> , <param_2> , .... , <param_N>
		//	[$]EEG , [$$]Emotiv , [$$$]<NameOfElectrode> , <NumberOfElectrode> , <val> 
		
//		if(UDPData.flag==true)
//		{
		if(toggle1 || toggle2 || toggle3 || toggle4 || toggle5 || toggle6 || toggle7 || toggle8 || toggle9 || toggle10 || toggle11 || toggle12|| toggle13 || toggle14 || toggle15)
		{
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:COUNTER:N0") && UDPData.flag==true)
			{
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]COUNTER, n0,"+EmoEngineInst.dict["COUNTER"]+","+"0"+","+"0"+","+"0"+";");
			}
			
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:COUNTER:N0"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:COUNTER:N0");
			}
		}		
			
		if(toggle1)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:AF3:N1"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:AF3:N1");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:AF3:N0") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]AF3, n1,"+EmoEngineInst.dict["AF3"]+","+"0"+","+"0"+","+"0"+";"); //Debug.Log("AF3: "+EmoEngineInst.dict["AF3"]);
			}
		}
				
		if(toggle2)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:AF4:N2"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:AF4:N2");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:AF4:N2") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]AF4, n2,"+ EmoEngineInst.dict["AF4"]+","+"0"+","+"0"+","+"0"+";"); //Debug.Log("AF3: "+EmoEngineInst.dict["AF3"]);
			}

		}
				
		if(toggle3)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:F3:N3"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:F3:N3");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:F3:N3") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]F3, n3,"+ EmoEngineInst.dict["F3"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle4)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:F4:N4"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:F4:N4");		
			}			
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:F4:N4") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]F4, n4,"+ EmoEngineInst.dict["F4"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle5)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:F7:N5"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:F7:N5");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:F7:N5") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]F7, n5,"+ EmoEngineInst.dict["F7"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
		if(toggle6)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:F8:N6"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:F8:N6");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:F8:N6") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]F8, n6,"+ EmoEngineInst.dict["F8"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle7)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:FC5:N7"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:FC5:N7");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:FC5:N7") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]FC5, n7,"+ EmoEngineInst.dict["FC5"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle8)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:FC6:N8"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:FC6:N8");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:FC6:N8") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]FC6, n8,"+ EmoEngineInst.dict["FC6"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle9)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:T7:N9"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:T7:N9");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:T7:N9") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]T7, n9,"+ EmoEngineInst.dict["T7"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle10)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:T8:N10"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:T8:N10");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:T8:N10") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]T8, n10,"+ EmoEngineInst.dict["T8"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle11)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:P7:N11"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:P7:N11");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:P7:N11") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]P7, n11,"+ EmoEngineInst.dict["P7"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle12)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:P8:N12"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:P8:N12");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:P8:N12") && UDPData.flag==true)
			{				
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]P8, n12,"+ EmoEngineInst.dict["P8"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle13)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:O1:N13"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:O1:N13");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:O1:N13") && UDPData.flag==true)
			{					
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]O1, n13,"+ EmoEngineInst.dict["O1"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle14)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:O2:N14"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:O2:N14");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:O2:N14") && UDPData.flag==true)
			{
				UDPData.sendString("[$]EEG,[$$]"+DeviceName+",[$$$]O2, n14,"+ EmoEngineInst.dict["O2"]+","+"0"+","+"0"+","+"0"+";");
			}
		}
				
		if(toggle15)
		{
			if(!DevicesLists.availableDev.Contains("EMOTIV:EEG:GYRO:POSITION"))
			{
				DevicesLists.availableDev.Add("EMOTIV:EEG:GYRO:POSITION");		
			}
			if(DevicesLists.selectedDev.Contains("EMOTIV:EEG:GYRO:POSITION") && UDPData.flag==true)
			{			
				UDPData.sendString("[$]tracking,[$$]"+DeviceName+",[$$$]Gyro,position,"+gyrox.ToString()+","+gyroy.ToString()+","+"0"+","+"0"+";");
			}
		}
/*			
			if(toggle16)
			{
				UDPData.sendString("[$]EEG,[$$]Emotiv,[$$]CMS,"+ " ");
			}
			if(toggle17)
			{
				UDPData.sendString("[$]EEG,[$$]Emotiv,[$$]DLR,"+ " ");
			}
*/			
			
			if(toggle19)//Affectiv Data
			{
				if(!DevicesLists.availableDev.Contains("EMOTIV:AFFECTIVE:LONGTERMEXCITEMENT:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:AFFECTIVE:LONGTERMEXCITEMENT:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:AFFECTIVE:LONGTERMEXCITEMENT:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]LongTermExcitement,value,"+EmoAffectiv.longTermExcitementScore.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}
				if(!DevicesLists.availableDev.Contains("EMOTIV:AFFECTIVE:SHORTTERMEXCITEMENT:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:AFFECTIVE:SHORTTERMEXCITEMENT:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:AFFECTIVE:SHORTTERMEXCITEMENT:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]ShortTermExcitement,value,"+EmoAffectiv.shortTermExcitementScore.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}			
				if(!DevicesLists.availableDev.Contains("EMOTIV:AFFECTIVE:MEDITATION:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:AFFECTIVE:MEDITATION:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:AFFECTIVE:MEDITATION:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]Meditation,value,"+EmoAffectiv.meditationScore.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}	
				if(!DevicesLists.availableDev.Contains("EMOTIV:AFFECTIVE:FRUSTRATION:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:AFFECTIVE:FRUSTRATION:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:AFFECTIVE:FRUSTRATION:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]Frustration,value,"+EmoAffectiv.frustrationScore.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}
				if(!DevicesLists.availableDev.Contains("EMOTIV:AFFECTIVE:BOREDOM:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:AFFECTIVE:BOREDOM:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:AFFECTIVE:BOREDOM:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]Boredom,value,"+EmoAffectiv.boredomScore.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}			
			}
			if(toggle20)//Expressiv Data
			{
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:BLINK:BOOL"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:BLINK:BOOL");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:BLINK:BOOL") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Button,[$$]"+DeviceName+",[$$$]Blink,bool,"+EmoExpressiv.isBlink.ToString()+","+"0"+","+"0"+","+"0"+";");
				}				
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:LOOKINGUP:BOOL"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:LOOKINGUP:BOOL");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:LOOKINGUP:BOOL") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Button,[$$]"+DeviceName+",[$$$]LookingUp,bool,"+EmoExpressiv.isLookingUp.ToString()+","+"0"+","+"0"+","+"0"+";");
				}		
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:LOOKINGDOWN:BOOL"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:LOOKINGDOWN:BOOL");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:LOOKINGDOWN:BOOL") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Button,[$$]"+DeviceName+",[$$$]LookingDown,bool,"+EmoExpressiv.isLookingDown.ToString()+","+"0"+","+"0"+","+"0"+";");
				}			
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:LOOKINGLEFT:BOOL"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:LOOKINGLEFT:BOOL");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:LOOKINGLEFT:BOOL") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Button,[$$]"+DeviceName+",[$$$]LookingLeft,bool,"+EmoExpressiv.isLookingLeft.ToString()+","+"0"+","+"0"+","+"0"+";");
				}
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:LOOKINGRIGHT:BOOL"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:LOOKINGRIGHT:BOOL");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:LOOKINGRIGHT:BOOL") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Button,[$$]"+DeviceName+",[$$$]LookingRight,bool,"+EmoExpressiv.isLookingRight.ToString()+","+"0"+","+"0"+","+"0"+";");
				}			
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:EYELOCATIONX:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:EYELOCATIONX:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:EYELOCATIONX:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]EyeLocationX,value,"+EmoExpressiv.eyeLocationX.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:EYELOCATIONY:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:EYELOCATIONY:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:EYELOCATIONY:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]EyeLocationY,value,"+EmoExpressiv.eyeLocationY.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}		
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:EYEBROWEXTENT:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:EYEBROWEXTENT:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:EYEBROWEXTENT:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]EyebrowExtent,value,"+EmoExpressiv.eyebrowExtent.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}			
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:SMILEEXTENT:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:SMILEEXTENT:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:SMILEEXTENT:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]SmileExtent,value,"+EmoExpressiv.smileExtent.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}		
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:UPPERFACEPOWER:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:UPPERFACEPOWER:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:UPPERFACEPOWER:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]UpperFacePower,value,"+EmoExpressiv.upperFacePower.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}
				if(!DevicesLists.availableDev.Contains("EMOTIV:EXPRESSIV:LOWERFACEPOWER:ANALOG"))
				{
					DevicesLists.availableDev.Add("EMOTIV:EXPRESSIV:LOWERFACEPOWER:ANALOG");		
				}
				if(DevicesLists.selectedDev.Contains("EMOTIV:EXPRESSIV:LOWERFACEPOWER:ANALOG") && UDPData.flag==true)
				{			
					UDPData.sendString("[$]Analog,[$$]"+DeviceName+",[$$$]LowerFacePower,value,"+EmoExpressiv.lowerFacePower.ToString("0.00")+","+"0"+","+"0"+","+"0"+";");
				}			

			}
			
			
//		}//if udp data
	}

}
