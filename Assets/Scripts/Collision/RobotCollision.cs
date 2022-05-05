using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    HealthPoints healthScript;
	void Start () {
		healthScript = this.GetComponent<HealthPoints>();		
	}

    //Might need refactoring later (switch case)
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION");
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

    private void PlayerCollision(Collision collision)
    {
        GameObject otherRobot = collision.gameObject;
        RobotMultiplayerMovement robotMovement = otherRobot.GetComponent<RobotMultiplayerMovement>();

        Vector3 otherForward = otherRobot.transform.forward;

        //this.GetComponent<Rigidbody>().isKinematic = true;

        //Frontal Collision
        if ((transform.forward + otherForward).magnitude < 0.001f)
        {

            //Check if other is moving or not 
        }

        else
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            RaycastHit hit;

            if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, boxCollider.size.x))
            {
                transform.position += otherForward * 2;

                Debug.Log("Got hit from the side " + this.name);
            }
        }

    }
    private void LaserCollision(Collision collider){
        healthScript.getHit(25);
        //Debug.Log("COLLISION LASER");
        GameObject laser = collider.gameObject;
        Destroy(laser, 0f);
        
    }
}
