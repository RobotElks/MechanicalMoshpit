using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;

public class SaveIP : NetworkBehaviour
{
    //public GameObject SaveIp;
    public GameObject Maps;
    public TMP_InputField addressInput;
    [SerializeField]
    private string saved;
    [SerializeField]
    private bool isHost;
    [SerializeField]
    private string worldString;

    public TMP_InputField inputPlayerName;
    public TMP_InputField inputHostName;
    private string savedPlayerName;

       // Start is called before the first frame update
    void Start()
    {
        GameObject[] informations = GameObject.FindGameObjectsWithTag("Information");
        foreach(GameObject info in informations)
        {
            if (this.gameObject != info)
            {
                saved = info.GetComponent<SaveIP>().GetSaved();
                GameObject.Destroy(info);
                addressInput.text = saved;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void SavePlayerName()
    {
        savedPlayerName = inputPlayerName.text;
    }
    public void SaveHostName()
    {
        savedPlayerName = inputHostName.text;
    }


    public string PlayerName()
    {
        return savedPlayerName;
    }

    public void SaveIPAddress(){
        saved = addressInput.text;
        isHost = false;
    }

    public void SaveIPAddress(string newIP)
    {
        saved = newIP;
        isHost = false;
    }

    public string GetSaved(){
        return saved;
    }

    public string GetWorldString()
    {
        return worldString;
    }

    public void SetWorldString(string worldString)
    {
        this.worldString = worldString;

    }

    public bool IsThisHost(){
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
