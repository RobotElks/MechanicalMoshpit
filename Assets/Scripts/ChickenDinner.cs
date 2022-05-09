using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChickenDinner : NetworkBehaviour
{
    Dead deadScript;
    [SerializeField] RobotList robotList;
    [SerializeField] GameObject ui;
    GameObject[] robotArray;

    public void robotDeath() {
        robotArray = robotList.GetRobots();
        for (int i = 0; i < robotArray.Length; i++) {
            deadScript = robotArray[i].GetComponent<Dead>();
            if(deadScript.IsDead()) {
                Debug.Log("ROBot DEATH1");
                robotList.RemoveRobot(robotArray[i]);
                if (robotList.GetRobots().Length <= 1) {
                    winner();
                }
            }
        }   
    }

    public void winner() {
        ui.SetActive(false);
    }
    
}
