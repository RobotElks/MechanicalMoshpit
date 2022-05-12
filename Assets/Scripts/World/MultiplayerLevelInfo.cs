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

    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();
        //gameRound = GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>();
        //gameRound.notReady.Add(gameObject);

        if (IsOwner)
        {
            //GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>().SetOwnRobotLevelScript(this);

            worldScript.CreateWorldParent();
            worldScript.BuildLobby();
            transform.position = worldScript.GetLobbySpawnPoint();
        }

        if (NetworkManager.Singleton.IsHost)
        {

            //gameRound.startEarlyButton.SetActive(true);

            if (IsOwner)
            {
                worldScript.LoadWorldFromInformation();
                worldScript.BuildWorld();
            }

            //Gets loaded world from host
            else
            {
                //SetHasStartedClientRpc(gameRound.hasStarted);
            }
        }
    }




}


