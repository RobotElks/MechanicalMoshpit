using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;

public class SaveIP : MonoBehaviour
{
    //public GameObject SaveIp;
    public GameObject Maps;
    public TMP_InputField addressInput;
    [SerializeField]
    private string saved;
    [SerializeField]
    private bool isHost;
    [SerializeField]
    private string world;
       // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveIPAddress(){
        saved = addressInput.text;
        isHost = false;
    }

    public string GetSaved(){
        return saved;
    }

    public string GetWorld()
    {
        return world;
    }

    public void SetWorld()
    {
        world = Maps.GetComponent<mapSelection>().GetMap();
    }

    public bool IsHost(){
        return isHost;
    }

    public void SaveOwnIPAddress(){
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                saved = ip.ToString();
                isHost = true;
                return;
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

}
