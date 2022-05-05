using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.IO;
using Unity.Collections;
using TMPro;

public class MultiplayerLevelInfo : NetworkBehaviour
{
    RobotMultiplayerMovement movementScript;
    MultiplayerWorldParse worldScript;
    GameRoundsManager gameRound;

    NetworkVariable<Vector3> spawnPoint = new NetworkVariable<Vector3>();
    
    public void StartCountDown()
    {
        StartCountdownClientRpc();
    }

    public bool SetReady()
    {
        if (!IsOwner) return false;
        SetReadyServerRpc();
        return true;
    }


    public bool SetNotReady()
    {
        Debug.Log("setting notready");
        if (!IsOwner) return false;
        SetNotReadyServerRpc();
        return true;
    }

    [ServerRpc]
    void SetReadyServerRpc() {
        gameRound.Ready.Add(gameObject);
        gameRound.notReady.Remove(gameObject);
        SetReadyClientRpc();
        if(gameRound.Ready.Count > 3 && gameRound.notReady.Count == 0){
            gameRound.StartCountDown();
        }
    }

    [ServerRpc]
    void SetNotReadyServerRpc() {
        Debug.Log("Ta bort");
        gameRound.notReady.Add(gameObject);
        gameRound.Ready.Remove(gameObject);
        SetNotReadyClientRpc();
    }

    [ClientRpc]
    void SetReadyClientRpc() {
        if(!gameRound.Ready.Contains(gameObject)){
            gameRound.Ready.Add(gameObject);
            gameRound.notReady.Remove(gameObject);
        }
    }

    [ClientRpc]
    void SetNotReadyClientRpc() {
        Debug.Log("Ta bort");
        if(!gameRound.notReady.Contains(gameObject)){
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
        gameRound.notReady.Add(gameObject);

        if (NetworkManager.Singleton.IsHost)
        {
            gameRound.startEarlyButton.SetActive(true);
            if (IsOwner)
            {
                worldScript.LoadWorldFromFile();
                
                transform.position = worldScript.GetSpawnAreaPoint();
            }

            //Gets loaded world from host
            else
            {
                string worldString = worldScript.GetWorldString();
                LoadWorldClientRpc(worldString);
                SetSpawnPointClientRpc(worldScript.GetSpawnAreaPoint());
                SetHasStartedClientRpc(gameRound.hasStarted);
            }
        }

    }

    public void StartGame() {
        Vector3 spawnPoint = worldScript.GetSpawnPoint();
        if (IsOwner) {
            Debug.Log("Is Owner");
            transform.position = spawnPoint;
        }
        else {
            //SetSpawnPointClientRpc(spawnPoint);
        }
    }

    [ClientRpc]
    void StartCountdownClientRpc(){
        gameRound.runButton.SetActive(false);
        gameRound.stopButton.SetActive(false);
        gameRound.readyButton.SetActive(false);
        gameRound.countdownText = gameRound.countdown.GetComponent<TextMeshProUGUI>();
        gameRound.timer = gameRound.counterTime;
        gameRound.countdownText.enabled = true;
        gameRound.isCountdowning = true;
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

    [ClientRpc]
    void SetHasStartedClientRpc(bool hasStarted)
    {
        gameRound.hasStarted = hasStarted;
        if(hasStarted){
            SetAsSpectator();
        }
    }

    public void SetTimer(float time){
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


