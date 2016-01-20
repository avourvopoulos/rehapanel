using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WiiDeviceLibrary;

public class WiiBalanceBoard : MonoBehaviour {
	
	// The coordinates for the 'get the box' mini-game.
	float _BoxX, _BoxY;

	private IBalanceBoard _BalanceBoard;
	
	public IBalanceBoard BalanceBoard
	{
		get { return _BalanceBoard; }
		set
		{
			if (_BalanceBoard != value)
			{
				_BalanceBoard = value;
				InitializeBalanceboard();
			}
		}
	}
	
	public WiiBalanceBoard()
	{
		
		_BoxX = 0.0f;
		_BoxY = 0.0f;
	}
	
	private void InitializeBalanceboard()
	{
		
		BalanceBoard.Updated += BalanceBoard_Updated;
	}
	
	void BalanceBoard_Updated(object sender, EventArgs e)
	{
		if (BalanceBoard != null)
		{
			//BalanceBoard.
		}
	}

	// Use this for initialization
	void Start () {
	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
