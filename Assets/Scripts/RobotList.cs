using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotList : MonoBehaviour
{
   public List<GameObject> robotList = new List<GameObject>();
   public List<GameObject> deadRobotList = new List<GameObject>();


    public void AddDeadRobot(GameObject robot) {
        deadRobotList.Add(robot);
    }
    public void RemoveDeadRobot(GameObject robot) {
        deadRobotList.Remove(robot);
    }

    public GameObject[] GetDeadRobots(){
        return deadRobotList.ToArray();
    }
    
    public void AddRobot(GameObject robot){
        robotList.Add(robot);
    }

    public void RemoveRobot(GameObject robot)
    {
        robotList.Remove(robot);
    }

    public GameObject[] GetRobots(){
        return robotList.ToArray();
    }
}
