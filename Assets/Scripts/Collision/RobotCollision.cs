using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCollision : MonoBehaviour
{
    RobotMultiplayerMovement movementScript;



    //Might need refactoring later (switch case)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }

        movementScript = GetComponent<RobotMultiplayerMovement>();
    }

    private void PlayerCollision(Collision robotCollision)
    {
        GameObject otherRobot = robotCollision.gameObject;
        RobotMultiplayerMovement otherRobotMovementScript = otherRobot.GetComponent<RobotMultiplayerMovement>();
        //Vector3 otherForward = otherRobot.transform.forward;



        //this.GetComponent<Rigidbody>().isKinematic = true;

        //Our robot is moving
        if (movementScript.IsMoving())
        {
            Vector3 movingDir = movementScript.GetMovingDirection();
            Vector3 otherMovingDir = otherRobotMovementScript.GetMovingDirection();


            //Heads on collision
            if ((movingDir + otherMovingDir).magnitude < 0.001f)
            {
                //Do priority check
                Debug.Log(this.name + " do priority check");
            }


            //Side collision
            else
            {
                //Change to multiple rays from the robot
                CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
                RaycastHit rayHit;
                bool colliderInfront = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, capsuleCollider.radius * 3);

                if (colliderInfront && rayHit.collider.gameObject != robotCollision.collider.gameObject)
                    colliderInfront = false;

                //Got hit from the side
                if (!colliderInfront)
                {
                    Debug.Log(this.name + " got hit from the side");

                    Pushed(otherRobot);
                }
            }
        }

        //Our robot is not moving
        else
        {
            Debug.Log(this.name + " is not moving");

            Pushed(otherRobot);
        }


    }

    private void Pushed(GameObject pusher)
    {

    }
}
