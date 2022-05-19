using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectTarget : MonoBehaviour
{
    //public float range;
    public Transform target;
    CannonBehavior CannonScript;
    RobotMovement MovementScript;
    Dead deadScript;
    //private bool reload = true;
    //public float fireRate = 0.5f;
    //private float nextFire = 0.0f;
    private float nextShotTime = 0.0f;
    private float reloadTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        CannonScript = this.GetComponent<CannonBehavior>();
        MovementScript = this.GetComponent<RobotMovement>();

        CheckIfTargetInScope();
    }

    private float CalculateDistance(){
        float distance = (target.position - transform.position).magnitude;
        return distance;
    }
    private float CalculateAngle(){
        Vector3 targetPos = target.position - transform.position;
        float angleBetween = Vector3.Angle(transform.forward, targetPos);
        return angleBetween;
    }

    private bool CheckIfTargetInScope(){
        //float distance = CalculateDistance();
        float angleBetween = CalculateAngle();
        if(/*(distance <= range) && */(MovementScript.startedRotation != true)){
            if ((angleBetween < 3f) && (angleBetween > -3f)){
                return true;
            }
        }
        return false;
    }

    private void ShootTarget(){
        deadScript = GetComponent<Dead>();
        if (!deadScript.IsDead())
            CannonScript.Shoot();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time > nextShotTime) && CheckIfTargetInScope()) {
            Debug.Log("But this?");
            nextShotTime = Time.time + reloadTime; 
            ShootTarget();
            // execute block of code here
        }
        
        
    }
}
