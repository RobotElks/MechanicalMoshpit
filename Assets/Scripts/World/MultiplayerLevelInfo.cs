using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.IO;
using Unity.Collections;

public class MultiplayerLevelInfo : NetworkBehaviour
{
    RobotMultiplayerMovement movementScript;
    MultiplayerWorldParse worldScript;

    NetworkVariable<Vector3> spawnPoint = new NetworkVariable<Vector3>();
    

    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();


        if (NetworkManager.Singleton.IsHost)
        {
            if (IsOwner)
            {
                worldScript.LoadWorldFromFile();
                
                transform.position = worldScript.GetSpawnPoint();
            }

            //Gets loaded world from host
            else
            {
                string worldString = worldScript.GetWorldString();
                LoadWorldClientRpc(worldString);

                SetSpawnPointClientRpc(worldScript.GetSpawnPoint());
            }
        }
    }

    [ClientRpc]
    void LoadWorldClientRpc(string worldString)
    {
        worldScript.SetWorldString(worldString);
    }

    [ClientRpc]
    void SetSpawnPointClientRpc(Vector3 point)
    {
        transform.position = point;
    }

    public bool GetIfOwner()
    {
        if (IsOwner)
            return true;
        return false;
    }


    


}


