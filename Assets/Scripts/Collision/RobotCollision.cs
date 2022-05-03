using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCollision : MonoBehaviour
{
    //CannonBehavior CannonScript;
    //CannonScript = robot.GetComponent<CannonBehavior>();
    //Might need refactoring later (switch case)
    private void OnCollisionEnter(Collision collision)
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
        Debug.Log("COLLISION LASER");
        GameObject laser = collider.gameObject;
        Destroy(laser, 0.1f);
    }
}
