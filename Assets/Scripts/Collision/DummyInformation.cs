using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;

public class DummyInformation : MonoBehaviour
{
    [SerializeField]
    private string IP;
    [SerializeField]
    private bool isHost;
    [SerializeField]
    private string world;

    public void Start()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                IP = ip.ToString();
            }
        }
    }
}
