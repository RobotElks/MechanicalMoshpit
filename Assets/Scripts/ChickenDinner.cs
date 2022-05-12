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

    public void robotDeath() {
        robotArray = robotList.GetRobots();
        for (int i = 0; i < robotArray.Length; i++) {
            deadScript = robotArray[i].GetComponent<Dead>();
            if(deadScript.IsDead()) {
                robotList.RemoveRobot(robotArray[i]);
                if (robotList.GetRobots().Length <= 1 && !IsOwner) {
                    winner();
                }
                else {
                    loser();
                }
            }
        }   
    }

    public void winner() {
        //gameRoundsManager.SetActive(false);
        readyScreen.SetActive(false);
        ui.SetActive(false);
        hud.SetActive(false);
        transform.Find("Canvas/Panel").gameObject.SetActive(true);
        transform.Find("Canvas/Panel/LoserText").gameObject.SetActive(false);
        winLose = transform.Find("Canvas/Panel/WinnerText");
        winLose.gameObject.SetActive(true);
    }

    public void loser() {
        if(IsOwner) {
            //gameRoundsManager.SetActive(false);
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
