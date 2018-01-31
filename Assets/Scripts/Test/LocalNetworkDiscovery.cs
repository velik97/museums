using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalNetworkDiscovery : NetworkDiscovery
{
    private InputField inputField;

    private string myIp;

    private void Awake()
    {
//        inputField = GameObject.FindObjectOfType<InputField>();
//        
//        inputField.onEndEdit.AddListener(delegate(string msg)
//        {
//            broadcastData = msg;
//        });
        
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress addr in localIPs)
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                myIp = addr.ToString();
            }
        }
    }

    public void StartBroadcasting()
    {
        Initialize();
        StartAsServer();
    }

    public void StartReceiving ()
    {
        Initialize();
        StartAsClient();
    }
    
    public override void OnReceivedBroadcast (string fromAddress, string data)
    {
        Debug.Log(myIp == fromAddress.Split(':').Last() ? "ok" : "not ok");
    }
}
