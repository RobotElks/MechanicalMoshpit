using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotList : MonoBehaviour
{
   public List<GameObject> robotList = new List<GameObject>();

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
