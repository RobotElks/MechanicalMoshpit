using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spectator : NetworkBehaviour
{

    [SerializeField] RobotList robotList;
    [SerializeField] GameRoundsManager gameRoundsManager;
    [SerializeField] Dead deadScript;
    CameraMultiplayer cameraScript;
    Transform cameraPos;
    private int chosenPlayer = 0;
    private bool assignDead = false;

    public override void OnNetworkSpawn()
    {
        if(gameRoundsManager.hasStarted)
            joinLate();
    }

    public void SetSpectateOnDeath() {
        for (int i = 0; i < robotList.GetDeadRobots().Length; i++) {
            if (IsOwner) {
                deadScript = robotList.GetDeadRobots()[i].GetComponent<Dead>();
            }
        }
           
        if(IsOwner && deadScript.IsDead()) {
            GameObject.Find("Main Camera").GetComponent<CameraMultiplayer>().SetLocalPlayer(robotList.GetRobots()[chosenPlayer].transform);
        }
    }

    public void joinLate() {
        if (IsOwner) {
            gameObject.SetActive(false);
            deadScript.SetDeadServerRpc(true);
        }
    }

    void Update() {
        if (!assignDead) {
            for(int i = 0; i < robotList.GetRobots().Length; i++){
                if(IsOwner) {
                    deadScript = robotList.GetRobots()[i].GetComponent<Dead>();
                    assignDead = true;
                }
            }
        }

        if(gameRoundsManager.hasStarted)

        if(IsOwner && deadScript.IsDead()) {
            if(Input.GetKeyDown("a") && chosenPlayer >= 0) {
                if(chosenPlayer == 0)
                    chosenPlayer = robotList.GetRobots().Length - 1;
                else
                    chosenPlayer -= 1;
                SetSpectateOnDeath();
            }

            else if(Input.GetKeyDown("d") && chosenPlayer <= (robotList.GetRobots().Length - 1)) {
                if(chosenPlayer == robotList.GetRobots().Length - 1)
                    chosenPlayer = 0;
                else
                    chosenPlayer += 1;
                SetSpectateOnDeath();
            }
        }
    }
}
