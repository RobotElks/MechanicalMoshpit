using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class MultiplayerLevelInfo : NetworkBehaviour
{
    RobotMultiplayerMovement movementScript;
    MultiplayerWorldParse worldScript;


    const int MAX_MAP_SEND_SIZE = 5000;

    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();

        if (IsOwner)
        {
            worldScript.CreateWorldParent();
            worldScript.CreateFlag();
            worldScript.BuildLobby();
            worldScript.SetRobotList();
            transform.position = worldScript.GetLobbySpawnPoint();

            if (NetworkManager.Singleton.IsHost)
            {
                worldScript.LoadWorldFromInformation();
                worldScript.BuildWorld();
            }
        }
    }


    public void HostSendWorldStringToClients()
    {
        if (IsHost && IsOwner)
        {
            ClearWorldStringClientRpc();

            string worldString = worldScript.GetWorldString();

            int index = 0;
            int length = worldString.Length;

            while (length > (index + MAX_MAP_SEND_SIZE))
            {
                SendPartOfWorldStringClientRpc(worldString.Substring(index, MAX_MAP_SEND_SIZE));
                index += MAX_MAP_SEND_SIZE;
            }

            SendPartOfWorldStringClientRpc(worldString.Substring(index));


            BuildWorldClientRpc();
        }

    }

    [ClientRpc]
    private void SendPartOfWorldStringClientRpc(string worldStringPart)
    {
        if (!IsHost)
            worldScript.AddToWorldString(worldStringPart);
    }

    [ClientRpc]
    private void ClearWorldStringClientRpc()
    {
        if (!IsHost)
            worldScript.SetWorldString("");
    }

    [ClientRpc]
    private void BuildWorldClientRpc()
    {
        if (!IsHost)
            worldScript.BuildWorld();
    }


    public void HostSendsSpawnPointsToClients()
    {
        if (IsHost && IsOwner)
        {
            Vector3[] spawnPoints = worldScript.GetSpawnPoints();

            GameObject[] robots = GameObject.Find("RobotList").GetComponent<RobotList>().GetRobots();

            foreach (GameObject robot in robots)
            {
                robot.GetComponent<MultiplayerLevelInfo>().SetSpawnPointClientRpc(spawnPoints);
            }

        }
    }

    [ClientRpc]
    private void SetSpawnPointClientRpc(Vector3[] spawnPoints)
    {

        int id = (int)NetworkManager.Singleton.LocalClientId;
        movementScript.MoveToSpawnPoints(spawnPoints[id]);
    }


}


