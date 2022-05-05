using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    HealthPoints healthScript;
    RobotMultiplayerMovement thisRobotMovementScript;

	void Start () {
		healthScript = this.GetComponent<HealthPoints>();
        thisRobotMovementScript = GetComponent<RobotMultiplayerMovement>();

	}


    private void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }

    private void CollisionCheck(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }

        else if (collision.collider.CompareTag("Laser"))
        {
            LaserCollision(collision);
        }
        

    }
    void OnControllerColliderHit(ControllerColliderHit hit){
        Debug.Log("COLLISION2");
        hit.gameObject.transform.position = Vector3.zero;
        if (hit.controller.CompareTag("Player"))
        {
            //PlayerCollision(hit);
        }

        else if (hit.controller.CompareTag("Laser"))
        {
            hit.rigidbody.isKinematic = true;
            //LaserCollision(hit);
        }
        
    }

    private void PlayerCollision(Collision robotCollision)
    {

        if (IsOwner)
        {

            RobotMultiplayerMovement otherRobotMovementScript = robotCollision.gameObject.GetComponent<RobotMultiplayerMovement>();

            if (otherRobotMovementScript.IsMoving() || otherRobotMovementScript.IsPushed())
            {
                Vector3 otherRobotForceOnThis = otherRobotMovementScript.GetForceToMe(transform.position);
                thisRobotMovementScript.Push(otherRobotForceOnThis, (int)otherRobotForceOnThis.magnitude);

            }
        }
    }
    private void LaserCollision(Collision collider){
        healthScript.getHit(1);
        //Debug.Log("COLLISION LASER");
        GameObject laser = collider.gameObject;
        Destroy(laser, 0f);
        
    }
}
