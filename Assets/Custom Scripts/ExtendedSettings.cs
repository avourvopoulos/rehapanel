using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

public class ExtendedSettings : Form {

//  The NotifyIcon object
private System.Windows.Forms.NotifyIcon notifyIcon1;
	
	void Start()
	{
//		this.notifyIcon1.Icon =((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
	}


	private void TrayMinimizerForm_Resize(object sender, EventArgs e)
	{
	     notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
	     notifyIcon1.BalloonTipText = "You have successfully minimized your form.";
	
	     if (FormWindowState.Minimized == this.WindowState)
	     {
	          notifyIcon1.Visible = true;
	          notifyIcon1.ShowBalloonTip(500);
	          this.Hide();    
	     }
	     else if (FormWindowState.Normal == this.WindowState)
	     {
	          notifyIcon1.Visible = false;
	     }
	}
	
	private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
	     this.Show();
	     this.WindowState = FormWindowState.Normal;
	}
	
}
