using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
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
        if (IsOwner)
        {
            GameObject otherRobot = robotCollision.gameObject;
            RobotMultiplayerMovement otherRobotMovementScript = otherRobot.GetComponent<RobotMultiplayerMovement>();
            //Vector3 otherForward = otherRobot.transform.forward;



            //this.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 otherMovingDir = otherRobotMovementScript.GetMovingDirection();

            //Our robot is moving
            if (movementScript.IsMoving())
            {
                Vector3 movingDir = movementScript.GetMovingDirection();

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
                    bool colliderInfront = Physics.Raycast(transform.position, movingDir, out rayHit, capsuleCollider.radius * 3);

                    if (colliderInfront && rayHit.collider.gameObject != robotCollision.collider.gameObject)
                        colliderInfront = false;

                    //Got hit from the side
                    if (!colliderInfront)
                    {

                        movementScript.Push(otherMovingDir);
                    }

                }
            }

            //Our robot is not moving
            else
            {
                Debug.Log(this.name + " is not moving");

                movementScript.Push(otherMovingDir);

            }

        }
    }
}
