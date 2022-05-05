using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class MultiplayerManager : MonoBehaviour
{
    public GameObject SaveIPObject;
    public SaveIP SaveIPScript;
    public GameObject NetworkManagerObject;
    UNetTransport Network;

    public static int TIMEOUT_MSEC = 500;

    void Start()
    {
        SaveIPObject = GameObject.Find("Information");
        SaveIPScript = SaveIPObject.GetComponent<SaveIP>();
        Network = NetworkManagerObject.GetComponent<UNetTransport>();
        if (SaveIPScript.IsHost())
        {
            StartHost();
        }
        else
        {
            StartClient();
        }
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Network.ConnectAddress = SaveIPScript.GetSaved();
        DateTime startTime = DateTime.Now;

        NetworkManager.Singleton.StartClient();

        while (!NetworkManager.Singleton.IsConnectedClient && timeout(startTime))
        {
            //Debug.Log("Waiting... time elapsed : " + DateTime.Now.Subtract(startTime).TotalMilliseconds);

        }
    }

    private bool timeout(DateTime time)
    {
        return DateTime.Now.Subtract(time).TotalMilliseconds < TIMEOUT_MSEC;
    }

    // Information overlay for developing / debugging
    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 40;
        GUILayout.BeginArea(new Rect((Screen.width / 2) - 350, 10, 900, 900));

        //if (SaveIPScript.IsHost())
        //{
        //    GUILayout.Label("Hosting game! IP : " + SaveIPScript.GetSaved(), guiStyle);
        //}
        //else if (!NetworkManager.Singleton.IsConnectedClient)
        //{
        //    GUILayout.Label("Not Connected! IP : " + SaveIPScript.GetSaved(), guiStyle);
        //}
        //else
        //{
        //    GUILayout.Label("Connected to game! IP : " + SaveIPScript.GetSaved(), guiStyle);

        //}

        GUILayout.EndArea();
    }
}
