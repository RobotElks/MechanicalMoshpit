using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class MultiplayerDetectTarget : NetworkBehaviour
{
    //public float range;
    RobotList robotList;
    CannonBehavior cannonScript;
    Dead deadScript;

    RobotRoundsHandler roundsHandlerScript;

    RaycastHit hit;
    
    //RobotMovement MovementScript;
    //private bool reload = true;
    //public float fireRate = 0.5f;
    //private float nextFire = 0.0f;
    private float nextShotTime = 0.0f;
    private float reloadTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        cannonScript = this.GetComponent<CannonBehavior>();
        //MovementScript = this.GetComponent<RobotMovement>();

        //CheckIfTargetInScope();
    }

    public override void OnNetworkSpawn()
    {
        robotList = GameObject.Find("RobotList").GetComponent<RobotList>();
        robotList.AddRobot(this.gameObject);

        roundsHandlerScript = GetComponent<RobotRoundsHandler>();
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
        if ((angleBetween < 0.8f) && (angleBetween > -0.8f) && CheckForWalls(target)){
            return true;
        }
        return false;
    }

    private bool CheckForWalls(Transform target)
    {
        Vector3 angleToTarget = target.position - transform.position;
        angleToTarget = angleToTarget.normalized;
        if (Physics.Raycast(transform.position, angleToTarget, out hit))
        {
            return hit.collider.tag == "Player";
        }
        return false;
    }

    private void ShootTarget(){

        cannonScript.Shoot();
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("Time: " + Time.time);
        //Debug.Log("reloadTime: " + reloadTime);

        if (IsOwner && roundsHandlerScript.GetCurrentGameState() != GameState.Programming)
        {
            GameObject[] robots = robotList.GetRobots();
            foreach (GameObject robot in robots)
            {
                // robot != this.gameObject && 
                if (robot != this.gameObject && (Time.time > nextShotTime) && CheckIfTargetInScope(robot.transform))
                {
                    //Debug.Log("check laser for robot : " + robot.GetInstanceID());
                    deadScript = robot.GetComponent<Dead>();
                    if (!deadScript.IsDead())
                    {
                        // CALL SERVER TO SHOOT
                        nextShotTime = Time.time + reloadTime;
                        Debug.Log("cooldown : " + nextShotTime);
                        ShootTarget();
                    }
                }
            }
        }
        
    }
}
