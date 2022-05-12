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
    GameRoundsManager gameRound;
    RobotCollision collisionScript;
    RobotMultiplayerInstructionScript instructionScript;

    [ServerRpc]
    public void StartCountDownServerRPC()
    {
        debugClientRpc("start server cd");

        Vector3[] sp = worldScript.GetSpawnPoints();

        //int MAX_MAP_SIZE = 5000;

        GameObject[] robots = GameObject.Find("RobotList").GetComponent<RobotList>().GetRobots();
        foreach (GameObject robot in robots)
        {
            debugClientRpc("robot for each");

            //string braveNewWorld = worldScript.GetWorldString();
            //int index = 0;
          
            //int length = braveNewWorld.Length;
            //Debug.Log("Map string length: " + length);
            //Debug.Log("index + 5000 : " + (int)(index + MAX_MAP_SIZE));
            //while (length > (int)(index + MAX_MAP_SIZE))
            //{
            //    Debug.Log("index : " + index);

            //    // map needs to be split over the network
            //    robot.GetComponent<MultiplayerLevelInfo>().getWorldClientRpc(braveNewWorld.Substring(index, MAX_MAP_SIZE));
            //    index += MAX_MAP_SIZE;

            //}

            //robot.GetComponent<MultiplayerLevelInfo>().getWorldClientRpc(braveNewWorld.Substring(index, length-index));
            robot.GetComponent<MultiplayerLevelInfo>().StartCountdownClientRpc(sp);


        }

    }

    [ClientRpc]
    void debugClientRpc(string msg)
    {
        Debug.Log("debug : " + msg);
    }

    public bool SetReady()
    {
        if (!IsOwner) return false;
        SetReadyServerRpc();
        return true;
    }


    public bool SetNotReady()
    {
        if (!IsOwner) return false;
        SetNotReadyServerRpc();
        return true;
    }

    [ServerRpc]
    void SetReadyServerRpc()
    {
        gameRound.Ready.Add(gameObject);
        gameRound.notReady.Remove(gameObject);
        SetReadyClientRpc();
        //if (gameRound.Ready.Count > 3 && gameRound.notReady.Count == 0)
        //{
        //    gameRound.StartCountDown();
        //}
    }

    [ServerRpc]
    void SetNotReadyServerRpc()
    {
        gameRound.notReady.Add(gameObject);
        gameRound.Ready.Remove(gameObject);
        SetNotReadyClientRpc();
    }

    [ClientRpc]
    void SetReadyClientRpc()
    {
        if (!gameRound.Ready.Contains(gameObject))
        {
            gameRound.Ready.Add(gameObject);
            gameRound.notReady.Remove(gameObject);
        }
    }

    [ClientRpc]
    void SetNotReadyClientRpc()
    {
        if (!gameRound.notReady.Contains(gameObject))
        {
            gameRound.notReady.Add(gameObject);
            gameRound.Ready.Remove(gameObject);
        }

    }

    public void SetAsSpectator()
    {
        transform.position = new Vector3(10f, 5f, 0f);
        gameObject.SetActive(false);
        gameRound.readyButton.SetActive(false);
        gameRound.programmingInterface.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        gameRound.Ready.Remove(gameObject);
        gameRound.notReady.Remove(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();
        gameRound = GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>();
        instructionScript = GetComponent<RobotMultiplayerInstructionScript>();
        collisionScript = GetComponent<RobotCollision>();
        gameRound.notReady.Add(gameObject);

        if (IsOwner)
        {
            GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>().SetOwnRobotLevelScript(this);

            worldScript.CreateWorldParent();
            worldScript.BuildLobby();
            transform.position = worldScript.GetLobbySpawnPoint();
            SendWorldServerRpc();

        }

        if (NetworkManager.Singleton.IsHost)
        {

            gameRound.startEarlyButton.SetActive(true);

            if (IsOwner)
            {
                worldScript.LoadWorldFromInformation();
                worldScript.BuildWorld();
            }

            //Gets loaded world from host
            else
            {
                SetHasStartedClientRpc(gameRound.hasStarted);
            }
        }
    }

    [ServerRpc]
    void SendWorldServerRpc()
    {
        // SEND OVER GAME MAP TO CLIENTS
        int MAX_MAP_SIZE = 5000;
        string braveNewWorld = worldScript.GetWorldString();
        int index = 0;

        int length = braveNewWorld.Length;
        Debug.Log("Map string length: " + length);
        Debug.Log("index + 5000 : " + (int)(index + MAX_MAP_SIZE));
        while (length > (int)(index + MAX_MAP_SIZE))
        {
            Debug.Log("index : " + index);

            // map needs to be split over the network
            GetWorldClientRpc(braveNewWorld.Substring(index, MAX_MAP_SIZE));
            index += MAX_MAP_SIZE;

        }

        GetWorldClientRpc(braveNewWorld.Substring(index, length - index));
    }

    public void StartGame()
    {

    }

    [ClientRpc]
    void GetWorldClientRpc(string part)
    {
        Debug.Log("Update World String");
        if (IsOwner)
            worldScript.BuildWorldString(part);
        //worldScript.SetWorldString(worldString);

    }

    [ClientRpc]
    void StartCountdownClientRpc(Vector3[] spawnPoints)
    {
        if (IsOwner)
        {
            Debug.Log("Start Count Down");
            //worldScript.SetWorldString(worldString);
            instructionScript.ClearInstructions();
            gameRound.runButton.SetActive(false);
            gameRound.stopButton.SetActive(false);
            gameRound.readyButton.SetActive(false);
            gameRound.countdownText = gameRound.countdown.GetComponent<TextMeshProUGUI>();
            gameRound.timer = gameRound.counterTime;
            gameRound.countdownText.enabled = true;
            gameRound.isCountdowning = true;

            if (!IsHost)
                worldScript.BuildWorld();

            int id = (int)NetworkManager.Singleton.LocalClientId;
            movementScript.ResetTargetPosition(spawnPoints[id]);

            transform.position = spawnPoints[id];
            collisionScript.onConveyorBelt = false;
        }
    }

    [ClientRpc]
    void SetHasStartedClientRpc(bool hasStarted)
    {
        gameRound.hasStarted = hasStarted;
        if (hasStarted)
        {
            SetAsSpectator();
        }
    }

    public void SetTimer(float time)
    {
        if (IsOwner)
            SetTimerServerRpc(time);
    }

    [ServerRpc]
    void SetTimerServerRpc(float time)
    {
        gameRound.TimerSet(time);
        SetTimerClientRpc(time);
    }

    [ClientRpc]
    void SetTimerClientRpc(float time)
    {
        gameRound.TimerSet(time);
    }





}


