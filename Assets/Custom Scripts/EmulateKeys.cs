using UnityEngine;
using System.Collections;
using WindowsInput;

public class EmulateKeys : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (ReceiveVRPN.isEmulating) {


			if (ReceiveVRPN.lbtn || ReceiveVRPN.lda2) {
				InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
				InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
			}
			
			else if (ReceiveVRPN.rbtn || ReceiveVRPN.lda1) {
				InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
				InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
			}
			else{
				InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
				InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
			}


//			if(ReceiveVRPN.isTraining){
//
//				if (ReceiveVRPN.lbtn) {
////								PressLeft ();
//							InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
//							InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
//						}
//
//				else if (ReceiveVRPN.rbtn) {
////								PressRight ();
//							InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
//							InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
//						}
//						else{
//							InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
//							InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
//						}
//				}//is training
//
//			if(!ReceiveVRPN.isTraining){
//				
//				if (ReceiveVRPN.lda2) {
//					//								PressLeft ();
//					InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
//					InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
//				}
//				
//				else if (ReceiveVRPN.lda1) {
//					//								PressRight ();
//					InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
//					InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
//				}
//				else{
//					InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
//					InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
//				}
//			}//not training


			}
	
	}

	

}
