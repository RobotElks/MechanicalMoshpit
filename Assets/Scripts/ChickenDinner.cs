using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChickenDinner : NetworkBehaviour
{
    Dead deadScript;
    [SerializeField] RobotList robotList;
    [SerializeField] GameObject ui;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject readyScreen;
    [SerializeField] GameObject gameRoundsManager;
    GameRoundsManager timer;
    Transform winLose;
    GameObject[] robotArray;

    public void RobotDeath()
    {
        if (IsHost)
            RobotDeathServerRpc();
    }

    [ServerRpc]
    void RobotDeathServerRpc() {
        robotArray = robotList.GetRobots();
        int length = robotArray.Length;
        int dead = 0;

        foreach(GameObject robot in robotArray){
            deadScript = robot.GetComponent<Dead>();
            if (deadScript.IsDead())
                dead++;
        }

        if (dead + 1 == length)
            WinnerStateClientRpc();

    }

    [ClientRpc]
    void WinnerStateClientRpc()
    {
        robotArray = robotList.GetRobots();

        foreach (GameObject robot in robotArray)
        {
            deadScript = robot.GetComponent<Dead>();
            if (deadScript.IsDead())
            {
                if (IsOwner)
                {
                    Loser();
                    break;
                }
            }
            else
            {
                if (IsOwner)
                {
                    Winner();
                    break;

                }
            }

        }
    }

    public void Winner() {
        gameRoundsManager.SetActive(false);
        readyScreen.SetActive(false);
        ui.SetActive(false);
        hud.SetActive(false);
        transform.Find("Canvas/Panel").gameObject.SetActive(true);
        transform.Find("Canvas/Panel/LoserText").gameObject.SetActive(false);
        winLose = transform.Find("Canvas/Panel/WinnerText");
        winLose.gameObject.SetActive(true);
    }

    public void Loser() {
        if(IsOwner) {
            gameRoundsManager.SetActive(false);
            readyScreen.SetActive(false);
            ui.SetActive(false);
            hud.SetActive(false);
            transform.Find("Canvas/Panel").gameObject.SetActive(true);
            transform.Find("Canvas/Panel/WinnerText").gameObject.SetActive(false);
            winLose = transform.Find("Canvas/Panel/LoserText");
            winLose.GetComponent<TextMeshProUGUI>().text += "\n#" + (robotList.GetRobots().Length + 1);
            winLose.gameObject.SetActive(true);
        }
    }
}
