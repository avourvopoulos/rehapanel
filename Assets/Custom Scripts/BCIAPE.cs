#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class BCIAPE : MonoBehaviour
{	
	string file_line = "";
	
	public string file2;
	public string file_dir;
	
//	public bool leftarrow = false;
//	public bool rightarrow = false;
	
	public List<double> right_list = new List<double>();
	public List<double> left_list = new List<double>();
	public List<double> LDA_Output;
	public List<int> Classifier;
	public List<int> StateOut;
	
	private Queue newUdpData = new Queue();
	string[] temp;
	public bool training = true;
	public bool selfPassed = false;
	double sr2pi = Math.Sqrt(2*Math.PI);
	double PL;
	double PR;
	double PB;
	double std;
	public int currentState = 0;
	double item;
	string incoming = "";
	int i = 0;
	Text incomingData;

	string lastValue = "";

	public double weight_0 = 0;
	public double weight_1 = 0;
	public double weight_2 = 0.3;
	
	public Boolean on = true;
	
	// receiving Thread
	Thread receiveThread;
	//use data thread
	Thread useThread;

//	Semaphore wait;
	
	// udpclient object
	UdpClient client;
	UdpClient send_client;

	Byte[] to_send;
	string sendIP = "127.0.0.1";
	string sendPort = "1205";
	IPEndPoint sendEndpoint;
	
	// udpclient port
	public int port = 1205;
	string user_name = "";

	#region GUI variables

	bool gui_training = false;
	bool gui_live = false;
	bool gui_ongoing = false;
	bool gui_user_ok = false;
	float hSliderValue = 0.0f;

	public Texture Left_texture;
	public Texture Right_texture;
	public Texture Neutral_texture;

	bool first = true;

	#endregion

	string show = "";

	public bool sendOnStateOne = false;
	public bool sendOnStateTwo = false;
	public bool sendOnStateThree = false;

	public float percentageBuffer = 0.7f;
	public string result = "";

	string LDA = "";
	string State = "";
	
	// Use this for initialization
	void Start()
	{
//		file_dir = Application.persistentDataPath;
		file_dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\APE_LOGS";
		if(!Directory.Exists(file_dir))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(file_dir);
			
		}
//		file_dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\APE_LOGS";
//		GameObject.Find("StartMenu").GetComponent<Canvas>().enabled = true;

//		wait = new Semaphore(1,1);

//		mutex = new Mutex(false);
	}

	void OnGUI()
	{
		if(MainGuiControls.BCIAPE){
		// Make a background box
//		GUI.Box(new Rect(10,10,780,580), "");

		// Main Menu
		if(!gui_live && !gui_training && !gui_ongoing)
		{
//			if(GUI.Button(new Rect(20,100,370,400), "Training Session"))
//			{
//				gui_training = true;
//				training = true;
//			}
			
//			if(GUI.Button(new Rect(410,100,370,400), "Live Session"))
//			{
				gui_live = true;
				training = false;
//			}
		}
		// End Main Menu

		// Training Session Menu
		if(gui_training)
		{
			user_name = GUI.TextField(new Rect(10, 10, 200, 20), user_name, 25);

//			if(GUI.Button(new Rect(Screen.width - 100,10,90,50), "Main Menu"))
//			{
//				user_name = "";
//				training = false;
//				gui_training = false;
//				gui_live = false;
//				MainMenu();
//			}

			if(GUI.Button(new Rect(Screen.width - 100,300,90,50), "Start"))
			{
				gui_training = false;
				gui_live = false;
				gui_ongoing = true;
				StartTrain();
			}
		}
		// End Training Session Menu

		// Live Session Menu
		if(gui_live)
		{
			if(GUI.Button(new Rect(10,10,90,50), "Load\nClassifier"))
			{
				ChooseUser();
			}

			if(GUI.Button(new Rect(170,10,100,50), "Create & Load\nClassifier"))
			{
				CreateLoad();
			}

			if(gui_user_ok)
			{
				GUI.Label(new Rect(100, 100, 100, 20), "User loaded!");
				GUI.Label(new Rect(100, 350, 100, 20), result);
			}
			else
			{
				GUI.Label(new Rect(100, 100, 100, 20), "No user loaded!");
			}

			GUI.Label(new Rect(500, 175, 200, 20), "Percentage Buffer");
			percentageBuffer = GUI.HorizontalSlider(new Rect(500, 200, 100, 30), percentageBuffer, 0.1f, 1);
			percentageBuffer = Mathf.Round(percentageBuffer * 10f) / 10f;
			GUI.Label(new Rect(605, 195, 20, 20), percentageBuffer+"");

			if(GUI.Button(new Rect(Screen.width - 100,300,90,50), "Start Live"))
			{
				if(gui_user_ok)
				{
					gui_user_ok = false;
					gui_training = false;
					gui_live = false;
					gui_ongoing = true;
					PerformanceIncrease();
					hSliderValue = 0;
					StartLive();

					if(first)
					{
						first = false;
						//create udp client to send to
						send_client = new UdpClient();
//						Byte[] to_send = Encoding.ASCII.GetBytes(send);
						IPAddress ip;
						IPAddress.TryParse(sendIP, out ip);
						sendEndpoint = new IPEndPoint(ip, Convert.ToInt32(sendPort));
					}
				}
				else
				{
					ChooseUser();
				}
			}

			sendOnStateOne = GUI.Toggle(new Rect(Screen.width/2 - 250,250,135,20), sendOnStateOne, " Send On State One");
			sendOnStateTwo = GUI.Toggle(new Rect(Screen.width/2 - 90,250,135,20), sendOnStateTwo, " Send On State Two");
			sendOnStateThree = GUI.Toggle(new Rect(Screen.width/2 + 70,250,145,20), sendOnStateThree, " Send On State Three");

			sendIP = GUI.TextArea(new Rect(Screen.width/2 - 90,300,110,20), sendIP, 15);
			sendPort = GUI.TextArea(new Rect(Screen.width/2 + 25,300,40,20), sendPort, 4);


			GUI.Label(new Rect(90, 175, 200, 20), "Performance Increase");
			hSliderValue = GUI.HorizontalSlider(new Rect(100, 200, 100, 30), hSliderValue, 0, 20);
			hSliderValue = Mathf.Round(hSliderValue);
			GUI.Label(new Rect(205, 195, 20, 20), hSliderValue+"");

			selfPassed = GUI.Toggle(new Rect(Screen.width/2 - 90,10,90,50), selfPassed, " Self Passed");

//			if(GUI.Button(new Rect(Screen.width - 100,10,90,50), "Main Menu"))
//			{
//				user_name = "";
//				training = false;
//				selfPassed = false;
//				gui_training = false;
//				gui_live = false;
//				gui_ongoing = false;
//				Back();
//			}
		}
		// End Live Session Menu

		// Ongoing Session Menu
		if(gui_ongoing)
		{
			if(GUI.Button(new Rect(Screen.width - 100,150,90,50), "Save Data"))
			{
				saveData();
			}

			if(GUI.Button(new Rect(200,300,90,50), "Save and\nMain Menu"))
			{
				gui_training = false;
				gui_live = false;
				gui_ongoing = false;
				training = false;
				selfPassed = false;
				MainMenu();
			}

			if(GUI.Button(new Rect(500,300,90,50), "Save and\nExit"))
			{
				gui_training = false;
				gui_live = false;
				gui_ongoing = false;
				Exit();
			}

			GUI.TextArea(new Rect(250,150,300,25), show);

			ShowFeedback();
		}
		// End Ongoing Session Menu

		}// if MainGuiControls.BCIAPE
	}

	void saveData()
	{
		string right = "";
		string left = "";
		foreach(double item in right_list)
		{
			right += item + ",";
		}
		foreach(double item in left_list)
		{
			left += item + ",";
		}
		
		string[] sep = {"/", "\\",".dat"};
		string[] wordsplit = file2.Split(sep, StringSplitOptions.RemoveEmptyEntries);
		//		print(wordsplit[5]);
		//		string path = "E:\\APE DATA\\";
		string path = file_dir+"\\";
		string f_name = wordsplit[(wordsplit.Count()-1)];
		
		//		print(file_dir);
		
		System.IO.File.AppendAllText(path+f_name+"_left.txt", left);
		System.IO.File.AppendAllText(path+f_name+"_right.txt", right);
		System.IO.File.AppendAllText(path+f_name+"_LDA.txt", LDA);
		System.IO.File.AppendAllText(path+f_name+"_State.txt", State);
		
		print("done\n");
	}

	void ShowFeedback()
	{
		/* 0 */GUI.DrawTexture(new Rect(Screen.width/2 - 3, 10, 6, 60), Neutral_texture, ScaleMode.ScaleAndCrop, true, 0);

		Color colPreviousGUIColor = GUI.color;

		switch(currentState)
		{
		case -3:
			/* -1 */GUI.DrawTexture(new Rect((Screen.width/2) - (1*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			/* -2 */GUI.DrawTexture(new Rect((Screen.width/2) - (2*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PL));
			/* -3 */GUI.DrawTexture(new Rect((Screen.width/2) - (3*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		case -2:
			/* -1 */GUI.DrawTexture(new Rect((Screen.width/2) - (1*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PL));
			/* -2 */GUI.DrawTexture(new Rect((Screen.width/2) - (2*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		case -1:
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PL));
			/* -1 */GUI.DrawTexture(new Rect((Screen.width/2) - (1*(60 + 10)), 10, 60, 60), Left_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		case 1:
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PR));
			/* 1 */GUI.DrawTexture(new Rect((Screen.width/2) + (0*60 + 1*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		case 2:
			/* 1 */GUI.DrawTexture(new Rect((Screen.width/2) + (0*60 + 1*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PR));
			/* 2 */GUI.DrawTexture(new Rect((Screen.width/2) + (1*60 + 2*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		case 3:
			/* 1 */GUI.DrawTexture(new Rect((Screen.width/2) + (0*60 + 1*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			/* 2 */GUI.DrawTexture(new Rect((Screen.width/2) + (1*60 + 2*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, Convert.ToSingle(PR));
			/* 3 */GUI.DrawTexture(new Rect((Screen.width/2) + (2*60 + 3*10), 10, 60, 60), Right_texture, ScaleMode.ScaleAndCrop, true, 0);
			break;
		}
	}
	
	public static double StandardDeviation(List<double> valueList)
	{
		double M = 0.0;
		double S = 0.0;
		int k = 1;
		foreach (double value in valueList) 
		{
			double tmpM = M;
			M += (value - tmpM) / k;
			S += (value - tmpM) * (value - M);
			k++;
		}
		return ( Math.Sqrt(S / (k-2)));
	}
	
	// Update is called once per frame
	void Update()
	{
//		print(0+newUdpData.Count);

		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			print("right");
//			SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
//			SendData("[$]button,[$$]ape,[$$$]rightarrow,event,1;");
			SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			print("left");
//			SendData("[$]button,[$$]ape,[$$$]leftarrow,event,1;");
//			SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
			SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
		}
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			print("straight");
//			SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
//			SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
			SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
		}

//		if(currentState == 0)
//		{
//			staten3.GetComponent<Image>().enabled = false;
//			staten2.GetComponent<Image>().enabled = false;
//			staten1.GetComponent<Image>().enabled = false;
//			state1.GetComponent<Image>().enabled = false;
//			state2.GetComponent<Image>().enabled = false;
//			state3.GetComponent<Image>().enabled = false;
//		}

//		UseDataThread();
	}

	public void stateMachine(double left, double right)
	{
//		print ("statemachine\n");
		switch(currentState)
		{
		case -3:
			if(right - left >= 0)
			{
				currentState = -2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten3.GetComponent<Image>().enabled = false;
//				staten2.GetComponent<Image>().enabled = true;
//				image = staten2.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(left);
//				staten2.GetComponent<Image>().color = color;
			}
			else
			{
				currentState = -3;
				//			SendData("[$]button,[$$]ape,[$$$]leftarrow,event,1;");
				//			SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
				
				if(sendOnStateThree)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
				//			image = staten3.GetComponent<Image>();
				//			color = image.color;
				//			color.a = Convert.ToSingle(left);
				//			staten3.GetComponent<Image>().color = color;
			}
			break;
		case -2:
			if(left - right >= weight_2)
			{
				currentState = -3;
//				SendData("[$]button,[$$]ape,[$$$]leftarrow,event,1;");
//				SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
				if(sendOnStateThree)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten3.GetComponent<Image>().enabled = true;
//				image = staten3.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(left);
//				staten3.GetComponent<Image>().color = color;
//				color.a =1;
//				staten2.GetComponent<Image>().color = color;
			}
			else
			if(right - left >= 0)
			{
				currentState = -1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten2.GetComponent<Image>().enabled = false;
//				image = staten1.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(left);
//				staten1.GetComponent<Image>().color = color;
			}
			else
			{
				currentState = -2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
			}
			break;
		case -1:
			if(left - right >= weight_1)
			{
				currentState = -2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten2.GetComponent<Image>().enabled = true;
//				image = staten2.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(left);
//				staten2.GetComponent<Image>().color = color;
//				color.a =1;
//				staten1.GetComponent<Image>().color = color;
			}
			else
			if(right - left >= 0)
			{
				currentState = 0;
//				SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
//				SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
				SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten1.GetComponent<Image>().enabled = false;
			}
			else
			{
				currentState = -1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
			}
			break;
		case 0:
			if(left - right >= weight_0)
			{
				currentState = -1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,-1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				staten1.GetComponent<Image>().enabled = true;
//				image = staten1.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(left);
//				staten1.GetComponent<Image>().color = color;
			}
			else
			if(right - left >= weight_0)
			{
				currentState = 1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state1.GetComponent<Image>().enabled = true;
//				image = state1.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(right);
//				state1.GetComponent<Image>().color = color;
			}
			else
			{
				currentState = 0;
				SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
			}
			break;
		case 1:
			if(right - left >= weight_1)
			{
				currentState = 2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state2.GetComponent<Image>().enabled = true;
//				image = state2.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(right);
//				state2.GetComponent<Image>().color = color;
//				color.a =1;
//				state1.GetComponent<Image>().color = color;
			}
			else
			if(left - right >= 0)
			{
				currentState = 0;
//				SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
//				SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
				SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state1.GetComponent<Image>().enabled = false;
			}
			else
			{
				currentState = 1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
			}
			break;
		case 2:
			if(right - left >= weight_2)
			{
				currentState = 3;
//				SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
//				SendData("[$]button,[$$]ape,[$$$]rightarrow,event,1;");
				if(sendOnStateThree)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state3.GetComponent<Image>().enabled = true;
//				image = state3.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(right);
//				state3.GetComponent<Image>().color = color;
//				color.a =1;
//				state2.GetComponent<Image>().color = color;
			}
			else
			if(left - right >= 0)
			{
				currentState = 1;
				if(sendOnStateOne)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state2.GetComponent<Image>().enabled = false;
//				image = state1.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(right);
//				state1.GetComponent<Image>().color = color;
			}
			else
			{
				currentState = 2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
			}
			break;
		case 3:
			if(left - right >= 0)
			{
				currentState = 2;
				if(sendOnStateTwo)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
//				state3.GetComponent<Image>().enabled = false;
//				image = state2.GetComponent<Image>();
//				color = image.color;
//				color.a = Convert.ToSingle(right);
//				state2.GetComponent<Image>().color = color;
			}
			else
			{
				currentState = 3;
				//			SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
				//			SendData("[$]button,[$$]ape,[$$$]rightarrow,event,1;");
				if(sendOnStateThree)
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,1,00;");
				else
					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
				//			image = state3.GetComponent<Image>();
				//			color = image.color;
				//			color.a = Convert.ToSingle(right);
				//			state3.GetComponent<Image>().color = color;
			}
			break;
		}
	}
	
	public void LogData(string data)
	{
//		System.IO.File.AppendAllText(file_dir+"\\"+file2+".txt", data);
		System.IO.File.AppendAllText(file2+".txt", data);
		System.IO.File.AppendAllText(file2+"_LDA_Classifier_Temporal.txt", LDA);
	}

	public void PerformanceIncrease()
	{
//		GameObject.Find("Text_increase").GetComponent<Text>().text = ((Slider)GameObject.Find("Slider").GetComponent<Slider>()).value.ToString() + "% increase";
		if(hSliderValue > 0)
		{
			weight_0 = 0.000114424219646283*Math.Pow(hSliderValue,3) - 0.00365168772245993*Math.Pow(hSliderValue,2) + 0.0470138620958514*hSliderValue - 0.0582081587701065;
			weight_1 = (8.76622201717681*Math.Pow(10,-5))*Math.Pow(hSliderValue,3) - 0.00326127035736112*Math.Pow(hSliderValue,2) + 0.0240128947579489*hSliderValue + 0.163655785412321;
		}
		else
		{
			weight_0 = 0;
			weight_1 = 0;
		}
	}
	
	public void SendData(string send)
	{
		//print ("sending "+send+"\n");
//		UdpClient send_client = new UdpClient();
		//send_client.EnableBroadcast = true;
		//send_client.Connect(new IPEndPoint(IPAddress.Broadcast, 1204));
		to_send = Encoding.ASCII.GetBytes(send);
//		IPAddress ip;
//		IPAddress.TryParse("127.0.0.1", out ip);
//		IPEndPoint endpoint = new IPEndPoint(ip, 1205);

		//descomentar
		send_client.Send(to_send, to_send.Length, sendEndpoint);
		
//		send_client.Close();
	}
	
	void OnApplicationQuit()
	{
		//client.EndReceive();
		on = false;

		if(client != null)
			client.Close();

		if(send_client != null)
			send_client.Close();

		if(receiveThread != null)
			receiveThread.Abort();

		if(useThread != null)
			useThread.Abort();	
	}
	
	void OnDisable()
	{
		on = false;

		if(client != null)
			client.Close();

		if(send_client != null)
			send_client.Close();

		if(receiveThread != null)
			receiveThread.Abort();

		if(useThread != null)
			useThread.Abort();	
	}
	
	// init
	private void init()
	{
//		GetVRPN.Start();

		receiveThread = new Thread(ReceiveData);
		receiveThread.IsBackground = true;
		receiveThread.Start();

		useThread = new Thread(UseDataThread);
		useThread.IsBackground = true;
		useThread.Start();
	}

	//use values thread
	private void UseDataThread()
	{
		long a;
		System.TimeSpan span = new System.TimeSpan(10000000 / 140);// ticks in one cycle of a 140Hz frequency
		System.TimeSpan sleepTime;

		while(on)
		{
			a = System.DateTime.Now.Ticks;

			if(newUdpData.Count > 0)
			{
				temp = newUdpData.Dequeue() as String[];
				item = Convert.ToDouble(temp[4]);

				LDA += item + ",";
				LDA_Output.Add(item);
				
				if(temp[5].Equals("10"))
					Classifier.Add(-1);
				else
					if(temp[5].Equals("01"))
						Classifier.Add(1);

				//log data
				if(temp[5].Equals("10"))
				{
					incoming = ++i + ": " + item + ",-1," + currentState + "\n" + incoming;
				}
				else
					if(temp[5].Equals("01"))
					{
						incoming = ++i + ": " + item + ",1," + currentState + "\n" + incoming;
					}
				
				if(training)
				{
					if(temp[5].Equals("10") && !temp[4].Equals("NaN"))//if it's left
					{
						left_list.Add(item);
					}
					else
						if(temp[5].Equals("01") && !temp[4].Equals("NaN"))//if it's right
					{
						right_list.Add(item);
					}
				}
				else // Live Session
				{
					Probability(item);
//					std = StandardDeviation(left_list);
//					PL = (1 / (std * sr2pi)) * Math.Exp(-(Math.Pow((item - (left_list.Average())), 2)) / (2 * Math.Pow(std, 2)));
//					
//					std = StandardDeviation(right_list);
//					PR = (1 / (std * sr2pi)) * Math.Exp(-(Math.Pow((item - (right_list.Average())), 2)) / (2 * Math.Pow(std, 2)));
//					
//					PB = PL + PR;
//					
//					PL = (PL * 0.5f) / PB;
//					PR = (PR * 0.5f) / PB;
					
					//TODO save value to the left or right list (before or after calculations?????)
					
					stateMachine(PL, PR);
					
					State += currentState + ",";
					StateOut.Add(currentState);
					
					//log data
					if(temp[5].Equals("10"))
					{
						file_line += item + ",-1," + currentState + "\n";
						if(!selfPassed) left_list.Add(item);
					}
					else
					{
						file_line += item + ",1," + currentState + "\n";
						if(!selfPassed) right_list.Add(item);
					}
				}
			}

			sleepTime = span.Subtract(new System.TimeSpan(System.DateTime.Now.Ticks - a));

			if(sleepTime.Ticks > 0)
				Thread.Sleep(sleepTime);

//			print((10000000.0 / (System.DateTime.Now.Ticks - a)).ToString("F0"));
		}
	}

	void Probability(double thisItem)
	{
		std = StandardDeviation(left_list);
		PL = (1 / (std * sr2pi)) * Math.Exp(-(Math.Pow((thisItem - (left_list.Average())), 2)) / (2 * Math.Pow(std, 2)));
		
		std = StandardDeviation(right_list);
		PR = (1 / (std * sr2pi)) * Math.Exp(-(Math.Pow((thisItem - (right_list.Average())), 2)) / (2 * Math.Pow(std, 2)));
		
		PB = PL + PR;
		
		PL = (PL * 0.5f) / PB;
		PR = (PR * 0.5f) / PB;
	}

	// receive thread
	private void ReceiveData()
	{
		client = new UdpClient(port);
		//client.Client.ReceiveBufferSize = 0;
//		client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 0);
		IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
		byte[] udpdata;
		string text;
		string[] sep = {";"};
		string[] wordsplit;
		
		while(on)//change to while is connected
		{
			try
			{
				udpdata = client.Receive(ref anyIP);
				text = Encoding.UTF8.GetString(udpdata);
				
				if(text!=String.Empty)
				{
					wordsplit = text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
					foreach (string wsp in wordsplit)
					{
						if(wsp.Length > 5)
							TranslateData(wsp);
					}
				}
				else
				{
					//	clearLists();//if no data clear lists
				}
				
			}
			catch (Exception err)
			{
				print("cenas "+err.ToString());
			}

//			Thread.Sleep(10);
		}
	}
	
	public void TranslateData(string text)
	{
		//	[$]<data  type> , [$$]<device> , [$$$]<joint> , <transformation> , <param_1> , <param_2> , .... , <param_N>
		//	[$]GameData , [$$]TPT-VR , [$$$]<joint> , <transformation> , <param_1> , <param_2> , .... , <param_N>

		//analog - openvibe - vrpn - signal - value(float)
		//button - openvibe - leftarrow - event - value (0;1)
		
		// Decompose incoming data based on the protocol rules
		string[] separators = {"[$]","[$$]","[$$$]",",", ";", " "};
		
		string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

		if(words[2].Equals("vrpn") && words[3].Equals("both"))
		{
//			print (words[0]+" "+words[1]+" "+words[2]+" "+words[3]+" "+words[4]+" "+words[5]+"\n");
			show = words[0]+" "+words[1]+" "+words[2]+" "+words[3]+" "+words[4]+" "+words[5];
			if(!selfPassed)
			{
				if(words[5].Equals("00"))
				{
					lastValue = words[4];
				}

				if(!words[5].Equals("00") && !words[4].Equals(lastValue))
				{
//					wait.WaitOne();
					newUdpData.Enqueue(words);
//					wait.Release(1);
				}
//				else
//				{
//					currentState = 0;
////					SendData("[$]button,[$$]ape,[$$$]leftarrow,event,0;");
////					SendData("[$]button,[$$]ape,[$$$]rightarrow,event,0;");
//					SendData("[$]button,[$$]openvibe,[$$$]vrpn,both,0,00;");
////					lastValue = words[4];
//				}
			}
			else
			{
				newUdpData.Enqueue(words);
			}
		}
	}

	public void ToggleTraining(bool toggle)
	{
		training = toggle;
	}

	public void ToggleSelfPassed(bool toggle)
	{
		selfPassed = toggle;
	}

	public void MainMenu()
	{
		if(!selfPassed)
		{
			Save();
		}

		file2 = "";
	}

	public void Back()
	{
		file2 = "";
//		GameObject.Find("LoadComplete").GetComponent<Text>().enabled = false;
//		GameObject.Find("LiveSession").GetComponent<Canvas>().enabled = false;
//		GameObject.Find("TrainSession").GetComponent<Canvas>().enabled = false;
//		GameObject.Find("StartMenu").GetComponent<Canvas>().enabled = true;
	}

	public void Exit()
	{

		if(!selfPassed)
		{
			Save();
		}

		Application.Quit();
	}

	public void StartLive()
	{
		if(file2.Equals(""))
		{
			ChooseUser();
		}
		else
		{
//			Load();
			
//			GameObject.Find("LoadComplete").GetComponent<Text>().enabled = false;
//			GameObject.Find("LiveSession").GetComponent<Canvas>().enabled = false;
//			GameObject.Find("OngoingSession").GetComponent<Canvas>().enabled = true;
			
//			if(selfPassed)
//			{
//				GameObject.Find("Save").GetComponent<Button>().interactable = false;
//			}

//			incomingData = GameObject.Find("IncomingData").GetComponent<Text>();
			
			init();
		}
	}

	public void StartTrain()
	{
		string day = System.DateTime.Now.Day.ToString();
		string month = System.DateTime.Now.Month.ToString();
		string year = System.DateTime.Now.Year.ToString();
		string hour = System.DateTime.Now.Hour.ToString();
		string minute = System.DateTime.Now.Minute.ToString();
		string second = System.DateTime.Now.Second.ToString();

//		file2 = Application.persistentDataPath+"\\"+user_name+"_"+day+"-"+month+"-"+year+"_"+hour+"-"+minute+"-"+second;
//		file2 = "C:\\Users\\neurorehablab\\Desktop\\APE_LOGS"+"\\"+user_name+"_"+day+"-"+month+"-"+year+"_"+hour+"-"+minute+"-"+second;
		file2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\APE_LOGS"+"\\"+user_name+"_"+day+"-"+month+"-"+year+"_"+hour+"-"+minute+"-"+second;

//		incomingData = GameObject.Find("IncomingData").GetComponent<Text>();

		init();
	}

	public void TrainSession()
	{
		training = true;
	}

	public void LiveSession()
	{
		training = false;
		send_client = new UdpClient();
	}

	public void CreateLoad()
	{
		System.Windows.Forms.OpenFileDialog choofdlog = new System.Windows.Forms.OpenFileDialog();
		choofdlog.Title = "Select \"left\", \"right\" and \"signal\" files";
		//		choofdlog.InitialDirectory = Application.persistentDataPath;
		choofdlog.InitialDirectory = file_dir;
		choofdlog.Filter = "txt files|*.txt";
		choofdlog.FilterIndex = 1;
		choofdlog.Multiselect = true;

		string[] files;
		string[] fileNames;

		string left = "", right = "", signal = "";

		if(choofdlog.ShowDialog() ==  System.Windows.Forms.DialogResult.OK && choofdlog.FileNames.Count() == 3)
		{
			files = choofdlog.FileNames;
			fileNames = choofdlog.SafeFileNames;


			int index = 0;
			foreach(string str in fileNames)
			{
				if(str.Equals("left.txt"))
				{
					left = files[index];
				}
				else if(str.Equals("right.txt"))
				{
					right = files[index];
				}
				else if(str.Equals("signal.txt"))
				{
					signal = files[index];
				}

				index++;
			}

			StreamReader theReader = new StreamReader(left, Encoding.UTF8);

			string line;

			List<double> time = new List<double>();
			List<string> values = new List<string>();


			// merge left, right and signal files into one file
			using (theReader)
			{
				line = theReader.ReadLine(); // ignore the headers

				do
				{
					line = theReader.ReadLine();

					if(line == null)
						break;

					string[] data = line.Split(',');
					time.Add(System.Convert.ToDouble(data[0]));
					values.Add(data[1]);
				}
				while(line != null);

				theReader = new StreamReader(right, Encoding.UTF8);
				line = theReader.ReadLine(); // ignore the headers
				
				do
				{
					line = theReader.ReadLine();
					
					if(line == null)
						break;

					string[] data = line.Split(',');
					time.Add(System.Convert.ToDouble(data[0]));
					values.Add(data[1]);
				}
				while(line != null);

				theReader = new StreamReader(signal, Encoding.UTF8);
				line = theReader.ReadLine(); // ignore the headers
				
				do
				{
					line = theReader.ReadLine();
					
					if(line == null)
						break;

					string[] data = line.Split(',');
					time.Add(System.Convert.ToDouble(data[0]));
					values.Add(data[1]);
				}
				while(line != null);

				theReader.Close();
			}

			// Sort data by time of events
//			time.Sort();

			double[] time1 = time.ToArray();
			string[] values1 = values.ToArray();

			Array.Sort(time1, values1);

			time = time1.ToList();
			values = values1.ToList();

			// Remove all zeros
			values.RemoveAll(x => x == "0.00000e+000");

			// Remove sequentialy repeated values
			values = values.Where((x, i) => i == 0 || values[i] != values[i - 1]).ToList();

			// Add side info
			string side = "0";
			List<string> sides = new List<string>();
			
			foreach(string str in values)
			{
				if(str.Equals("769")) // LEFT
					side = "-1";
				else if(str.Equals("770")) // RIGHT
					side = "1";
				else if(str.Equals("800")) // TRIAL END
					side = "0";
				
				sides.Add(side);
			}

			// Remove Openvibe flags
			var result = Enumerable.Range(0, values.Count).Where(i => values[i] == "769" || values[i] == "770" || values[i] == "800").ToList();

			foreach(int res in result)
			{
				sides[res] = "remove";
			}

			sides = sides.FindAll(x => x !="remove");

			values = values.Where(x => x != "769" && x != "770" && x != "800").ToList();


			// Remove sequentialy repeated values
			result = Enumerable.Range(0, values.Count).Where(i => i == 0 || values[i] != values[i - 1]).ToList();


			List<string> sides1 = new List<string>();
			foreach(int res in result)
			{
//				if(res > 0)
//					sides[res] = "remove";
				sides1.Add(sides[res]);
			}

//			sides = sides.FindAll(x => x != "remove");
			sides = sides1;

			values = values.Where((x, i) => i == 0 || values[i] != values[i - 1]).ToList();

			// Load values to the classifier
			int j = 0;
			foreach(string str in values)
			{
				if(sides[j] == "-1")
					left_list.Add(System.Convert.ToDouble(str));
				else if(sides[j] == "1")
					right_list.Add(System.Convert.ToDouble(str));

				j++;
			}

			// Create and Save the .dat file
			System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog1.Filter = "dat files|*.dat";
			saveFileDialog1.Title = "Save the classifier binary file";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.ShowDialog();

			if(saveFileDialog1.FileName != "")
			{
				file2 = saveFileDialog1.FileName;

				string day = System.DateTime.Now.Day.ToString();
				string month = System.DateTime.Now.Month.ToString();
				string year = System.DateTime.Now.Year.ToString();
				string hour = System.DateTime.Now.Hour.ToString();
				string minute = System.DateTime.Now.Minute.ToString();
				string second = System.DateTime.Now.Second.ToString();

				file2 = file2.Replace(".dat", "_"+day+"-"+month+"-"+year+"_"+hour+"-"+minute+"-"+second+".dat");

				Save(); // create the .dat file
				Load(); // load the created .dat file

				gui_user_ok = true;
			}
			else
			{
				//error
			}
		}
		else
		{
			//error
		}
	}

	public void ChooseUser()
	{

	#if UNITY_EDITOR

		file2 = EditorUtility.OpenFilePanel("Load user training set", file_dir, "dat");
		if(file2.Length != 0)
		{
			gui_user_ok = true;
		}

	#else

		System.Windows.Forms.OpenFileDialog choofdlog = new System.Windows.Forms.OpenFileDialog();
		choofdlog.Title = "Load user training set";
//		choofdlog.InitialDirectory = Application.persistentDataPath;
		choofdlog.InitialDirectory = file_dir;
		choofdlog.Filter = "dat files|*.dat";
		choofdlog.FilterIndex = 1;
		choofdlog.Multiselect = false;

		if(choofdlog.ShowDialog() ==  System.Windows.Forms.DialogResult.OK)
		{
//			print (choofdlog.FileName);
//			file2 = choofdlog.SafeFileName.Replace(".dat", "");
			file2 = choofdlog.FileName;
//			print (file2);
			gui_user_ok = true;
		}
		else
		{
//			GameObject.Find("LoadComplete").GetComponent<Text>().text = "Something went wrong! Please try again";
		}

//		GameObject.Find("LoadComplete").GetComponent<Text>().enabled = true;


//		string[] files = Directory.GetFiles(path);
//		file2 = Path.GetFileNameWithoutExtension(path).ToString();
	#endif

		Load();
	}

	public void Save()
	{
		UserData data = new UserData();
//		data.LDA_Output = LDA_Output;
//		data.StateOut = StateOut;
//		data.Classifier = Classifier;

		//remove outliers
		double std = StandardDeviation(left_list);
		double mean = left_list.Average();

		List<double> temp;

		temp = left_list.FindAll(item => item < (mean + (3*std)));
		left_list = temp;
		temp = left_list.FindAll(item => item > (mean - (3*std)));
		left_list = temp;


		std = StandardDeviation(right_list);
		mean = right_list.Average();

		temp = right_list.FindAll(item => item < (mean + (3*std)));
		right_list = temp;
		temp = right_list.FindAll(item => item > (mean - (3*std)));
		right_list = temp;



		data.LeftList = left_list;
		data.RighttList = right_list;
		
		BinaryFormatter bf = new BinaryFormatter();
//		file2 = file2.Replace(".dat", "");
//		FileStream file = File.Create(file2+".dat");
		FileStream file = File.Create(file2);
		
//		print (Application.persistentDataPath);
		
		bf.Serialize(file, data);
		file.Close();
		
		LogData(file_line);
//		bool exc = false;
//		string error = "";
//		try
//		{
//			System.IO.File.AppendAllText(file2+".txt", file_line);
//		}
//		catch(Exception e)
//		{
////			error = e.ToString();
//			print(e.ToString());
//		}

//		if(exc)
//		{
//			GameObject.Find("Complete").GetComponent<Text>().text = "Error: "+ error;
//		}
//
//		GameObject.Find("Complete").GetComponent<Text>().enabled = true;
	}

	public void Load()
	{
		if(File.Exists(file2))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(file2, FileMode.Open);
			UserData data = (UserData)bf.Deserialize(file);
			file.Close();

//			LDA_Output = data.LDA_Output;
//			StateOut = data.StateOut;
//			Classifier = data.Classifier;
			left_list = data.LeftList;
			right_list = data.RighttList;

			// Check maximum percentage
			double min, max;
			double ml = left_list.Min();
			double mr = right_list.Min();
			if(ml < mr)
			{
				min = ml;
			}
			else
			{
				min = mr;
			}

			ml = left_list.Max();
			mr = right_list.Max();

			if(ml > mr)
			{
				max = ml;
			}
			else
			{
				max = mr;
			}

			double step = (max - min) / 100;
			double i;
			List<double> l1 = new List<double>();
			List<double> l2 = new List<double>();
			for(i = min; i < max; i += step)
			{
				Probability(i);
				l1.Add(PL - PR);
				l2.Add(PR - PL);
			}

			List<double> l = new List<double>();
			l.Add(l1.Max());
			l.Add(l2.Max());

			double m = l.Min();
			m *= percentageBuffer;
//			print (m+"\n");

			int j, stateOne = 0, stateTwo = 0, stateThree = 0;
			double p0 = 0, p1 = 0;
			for(j = 0; j <= 20; j++)
			{
				p0 = 0.000114424219646283*Math.Pow(j,3) - 0.00365168772245993*Math.Pow(j,2) + 0.0470138620958514*j - 0.0582081587701065;
				p1 = (8.76622201717681*Math.Pow(10,-5))*Math.Pow(j,3) - 0.00326127035736112*Math.Pow(j,2) + 0.0240128947579489*j + 0.163655785412321;

				if(p0 <= m)
				{
					stateOne = j;
				}

				if(p0 <= m && p1 <= m)
				{
					stateTwo = j;
				}

				if(p0 <= m && p1 <= m && weight_2 <= m)
				{
					stateThree = j;
				}

//				print(j +" -> " + (p0 + p1 /*+ weight_2*/) + " :" + a+"\n");
//				print(j + " -> " + p0+"; "+p1+"\n");
			}

			print("state 1 -> "+stateOne+"; state 2 -> "+stateTwo+"; state 3 -> "+stateThree+"\n\n");

			if(stateThree >= stateOne && stateThree >= stateTwo)
			{
				// Use State Three
//				print("state 3 with " + stateThree + "\n");
				result = "state 3 with " + stateThree + "% increase\n";
			}
			else if(stateTwo >= stateOne && stateTwo > stateThree)
			{
				// Use State Two
//				print("state 2 with " + stateTwo + "\n");
				result = "state 2 with " + stateTwo + "% increase\n";
			}
			else if(stateOne > stateTwo && stateOne > stateThree)
			{
				// Use State One
//				print("state 1 with " + stateOne + "\n");
				result = "state 1 with " + stateOne + "% increase\n";
			}

			print(result);

//			double l_std = StandardDeviation(left_list);
//			double r_std = StandardDeviation(right_list);
//
//			List<double> right = right_list;
//			List<double> left = left_list;
//
//			right.Sort();
//			left.Sort();
//
//			double left_median = 2000;
//			double right_median = 2000;
//
//			int r_count = right.Count;
//			int l_count = left.Count;
//
//			if(r_count > 0 )
//			{
//				if(r_count % 2 == 0)
//				{
//					double a = right[r_count / 2 - 1];
//					double b = right[r_count / 2];
//					right_median = (a + b) / 2;
//				}
//				else
//				{
//					right_median = right[r_count / 2];
//				}
//
//				print("right: std = " + r_std + "; median = " + right_median + "\n");
//			}
//
//			if(l_count > 0 )
//			{
//				if(l_count % 2 == 0)
//				{
//					double a = left[l_count / 2 - 1];
//					double b = left[l_count / 2];
//					left_median = (a + b) / 2;
//				}
//				else
//				{
//					left_median = left[l_count / 2];
//				}
//
//				print("left: std = " + l_std + "; median = " + left_median + "\n");
//			}

		}
	}
}

[Serializable]
class UserData
{
//	public List<double> LDA_Output;
//	public List<int> StateOut;
//	public List<int> Classifier;
	public List<double> LeftList;
	public List<double> RighttList;
}
