using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Threading;

public class Emulator : MonoBehaviour {	
	
	[DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
	
	[DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
	
	[DllImport("user32.dll")]
    static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public SendInputEventType type;
        public MouseKeybdhardwareInputUnion mkhi;
	    public uint Type;
		//public MouseKeybdhardwareInput Data;
    }
    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdhardwareInputUnion
    {
        [FieldOffset(0)]
        public MouseInputData mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }
    struct MouseInputData
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [Flags]
    enum MouseEventFlags : uint
    {
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_MIDDLEDOWN = 0x0020,
        MOUSEEVENTF_MIDDLEUP = 0x0040,
        MOUSEEVENTF_XDOWN = 0x0080,
        MOUSEEVENTF_XUP = 0x0100,
        MOUSEEVENTF_WHEEL = 0x0800,
        MOUSEEVENTF_VIRTUALDESK = 0x4000,
        MOUSEEVENTF_ABSOLUTE = 0x8000
    }
    enum SendInputEventType : int
    {
        InputMouse,
        InputKeyboard,
        InputHardware
    }
	
	public const int MOUSEEVENTF_LEFTDOWN = 0x02;
	public const int MOUSEEVENTF_LEFTUP = 0x04;
	public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
	public const int MOUSEEVENTF_RIGHTUP = 0x10;
	
    public static void MoveMouse(int x, int y)
    {
		SetCursorPos(x,y);
	//	print("x: "+x +" y: "+y);
		
    }

	
	//This simulates a left mouse click
public static void LeftMouseClick(int xpos, int ypos)
{
//    SetCursorPos(xpos, ypos);
    mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
	Thread.Sleep(200);
    mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
		Debug.Log("LeftMouseClick");
}
	
	//This simulates a right mouse click
public static void RightMouseClick(int xpos, int ypos)
{
//    SetCursorPos(xpos, ypos);
    mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
	Thread.Sleep(200);
    mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
		Debug.Log("RightMouseClick");
}
	

	//Click Keyboard Keys
	public static void SendKeyPress(KeyCode keyCode)
	{
    INPUT input = new INPUT {
        Type = 1
    };
        input.mkhi.ki = new KEYBDINPUT() {
        wVk = (ushort)keyCode,
        wScan = 0,
        dwFlags = 0,
        time = 0,
        dwExtraInfo = IntPtr.Zero,
    };

    INPUT input2 = new INPUT {
        Type = 1
    };
    input2.mkhi.ki = new KEYBDINPUT() {
        wVk = (ushort)keyCode,
        wScan = 0,
        dwFlags = 2,
        time = 0,
        dwExtraInfo = IntPtr.Zero
    };
          
	}
	

}
