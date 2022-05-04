using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    RobotMultiplayerMovement movementScript;

    int singleRobotRaycastMask;

    private void Start()
    {
        singleRobotRaycastMask = ~LayerMask.NameToLayer("RobotsRaycast");
    }



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

            //this.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 otherMovingDir = otherRobotMovementScript.GetMovingDirection();


            if (otherRobotMovementScript.IsMoving())
            {
                //Our robot is moving
                if (movementScript.IsMoving())
                {
                    Vector3 movingDir = movementScript.GetMovingDirection();

                    //Heads on collision
                    if ((movingDir + otherMovingDir).magnitude < 0.001f)
                    {
                        //Do priority check
                        Debug.Log("Heads on");
                        float gearDiff = movementScript.GetGear() - otherRobotMovementScript.GetGear();
                        if (gearDiff == 0) movementScript.ResetToLastPosition();
                        else if (gearDiff < 0) movementScript.Push(otherMovingDir);
                    }


                    //Side collision
                    else
                    {
                        //Change to multiple rays from the robot

                        int oldLayer = otherRobot.layer;
                        otherRobot.layer = LayerMask.NameToLayer("RobotsRaycast");

                        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

                        bool colliderInfront = Physics.Raycast(transform.position - transform.right * capsuleCollider.radius, movingDir, capsuleCollider.radius * 3, singleRobotRaycastMask);

                        if (!colliderInfront)
                            Physics.Raycast(transform.position + transform.right * capsuleCollider.radius, movingDir, capsuleCollider.radius * 3, singleRobotRaycastMask);


                        //Got hit from the side
                        if (!colliderInfront)
                            movementScript.Push(otherMovingDir);


                        otherRobot.layer = oldLayer;

                    }
                }

                //Our robot is not moving
                else
                {
                    movementScript.Push(otherMovingDir);
                }
            }

        }
    }
}
