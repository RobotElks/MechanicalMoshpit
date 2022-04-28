using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class MultiplayerManager : MonoBehaviour
{
    public GameObject SaveIPObject;
    public SaveIP SaveIPScript;
    public GameObject NetworkManagerObject;
    UNetTransport Network;
    void Start() 
    {
        SaveIPObject = GameObject.Find("SaveIP");
        SaveIPScript = SaveIPObject.GetComponent<SaveIP>();
        Network = NetworkManagerObject.GetComponent<UNetTransport>();
        if(SaveIPScript.IsHost()){
            StartHost();
        }
        else{
            StartClient();
        }
        Debug.Log(SaveIPScript.GetSaved());

    }
    public void StartHost()
    {
       NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Network.ConnectAddress = SaveIPScript.GetSaved();
        NetworkManager.Singleton.StartClient();
    }
}
