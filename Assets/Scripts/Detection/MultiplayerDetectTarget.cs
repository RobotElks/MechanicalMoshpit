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

        deadScript = GetComponent<Dead>();
        roundsHandlerScript = GetComponent<RobotRoundsHandler>();
    }

    public override void OnNetworkDespawn()
    {
        robotList.RemoveRobot(this.gameObject);
    }



    private bool CheckIfTargetInScope(){
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit))
        {

            if(hit.collider.tag == "Player")
            return !hit.collider.gameObject.GetComponent<Dead>().IsDead();
        }
        return false;
    }

    private void ShootTarget(){

        cannonScript.Shoot();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner && roundsHandlerScript.GetCurrentGameState() != GameState.Programming)
        {
                if ((Time.time > nextShotTime) && CheckIfTargetInScope())
                {
                    // CALL SERVER TO SHOOT
                    nextShotTime = Time.time + reloadTime;
                    ShootTarget();
                }
        }
        
    }
}
