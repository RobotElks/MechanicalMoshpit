using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class MultiplayerDetectTarget : NetworkBehaviour
{
    //public float range;
    RobotList robotList;
    CannonBehavior CannonScript;
    Dead deadScript;
    //RobotMovement MovementScript;
    //private bool reload = true;
    //public float fireRate = 0.5f;
    //private float nextFire = 0.0f;
    private float nextShotTime = 0.0f;
    private float reloadTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        CannonScript = this.GetComponent<CannonBehavior>();
        //MovementScript = this.GetComponent<RobotMovement>();

        //CheckIfTargetInScope();
    }

    public override void OnNetworkSpawn()
    {
        robotList = GameObject.Find("RobotList").GetComponent<RobotList>();
        robotList.AddRobot(this.gameObject);

    }

    public override void OnNetworkDespawn()
    {
        robotList.RemoveRobot(this.gameObject);
    }

    // private float CalculateDistance(){
    //     float distance = (target.position - transform.position).magnitude;
    //     return distance;
    // }

    private float CalculateAngle(Transform target){
        Vector3 targetPos = target.position - transform.position;
        float angleBetween = Vector3.Angle(transform.forward, targetPos);
        return angleBetween;
    }

    private bool CheckIfTargetInScope(Transform target){
        //float distance = CalculateDistance();
        float angleBetween = CalculateAngle(target);
        if ((angleBetween < 3f) && (angleBetween > -3f)){
            return true;
        }
        
        return false;
    }

    private void ShootTarget(){
        
        CannonScript.shoot();
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log("Time: " + Time.time);
        //Debug.Log("reloadTime: " + reloadTime);
        GameObject[] robots = robotList.GetRobots();

        foreach(GameObject robot in robots){
            if (robot != this.gameObject && (Time.time > nextShotTime) && CheckIfTargetInScope(robot.transform)) {
                deadScript = robot.GetComponent<Dead>();
                if (!deadScript.IsDead()) {
                    nextShotTime = Time.time + reloadTime; 
                    robot.GetComponent<Rigidbody>().WakeUp();
                    Debug.Log(robot.GetComponent<Rigidbody>().IsSleeping());
                    ShootTarget();
                }
            }
        }
        
        
    }
}
