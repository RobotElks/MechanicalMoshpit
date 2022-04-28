using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.IO;
using Unity.Collections;

public class MultiplayerLevelInfo : NetworkBehaviour
{
    string levelFile = "Assets/Worlds/level20x20.txt";

    //NetworkVariable<> levelInformation = new NetworkVariable<>();
    public NetworkList<char> networkLevelInformation;
    public NetworkVariable<bool> savedLevel;

    public void Awake()
    {
        networkLevelInformation = new NetworkList<char>();
        savedLevel = new NetworkVariable<bool>(false);
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            if (IsOwner)
            {
                GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>().LoadWorldFromFile();
            }

            else
            {
                string worldString = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>().GetWorldString();
                LoadWorldClientRpc(worldString);
            }
        }

    }

    [ClientRpc]
    void LoadWorldClientRpc(string worldString)
    {
        GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>().SetWorldStringAndParse(worldString);
    }

    


}


