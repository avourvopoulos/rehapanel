using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPClient : MonoBehaviour
{
    int remotePort = 19784;
    UdpClient receiver;
    IPEndPoint receiveIPGroup;

    void Start()
    {
        StartReceivingIP();

        //InvokeRepeating("receive", 0, 2f);
    }

    //void receive()
    //{
    //    receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
    //}

    public void StartReceivingIP()
    {
        try {
            if (receiver == null) {
                receiver = new UdpClient (remotePort);
                receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
            }
        } catch (SocketException e) {
            Debug.Log (e.Message);
        }
    }
    private void ReceiveData(IAsyncResult result)
    {
        receiveIPGroup = new IPEndPoint(IPAddress.Any, remotePort);
        byte[] received;
        if (receiver != null)
        {
            received = receiver.EndReceive(result, ref receiveIPGroup);
        }
        else
        {
            return;
        }
        receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
        string receivedString = Encoding.ASCII.GetString(received);
        print(receivedString); 
    }
}
