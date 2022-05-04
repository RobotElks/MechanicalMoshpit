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
        movementScript = GetComponent<RobotMultiplayerMovement>();
    }


    //Might need refactoring later (switch case)
    // Does this risk mulltiple detections for one collision?
    private void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        //CollisionCheck(collision);
    }

    private void CollisionCheck(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }
    }

    private void PlayerCollision(Collision robotCollision)
    {
        if (IsOwner)
        {
            GameObject otherRobot = robotCollision.gameObject;
            RobotMultiplayerMovement otherRobotMovementScript = otherRobot.GetComponent<RobotMultiplayerMovement>();

            //this.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 otherMovingDir = otherRobotMovementScript.GetMovingDirection();
            Gear otherRobotGear = otherRobotMovementScript.GetGear();


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
                        //Debug.Log("Heads on");

                        movementScript.MoveTargetPositionBack((int)otherRobotGear);
                        movementScript.Push(otherMovingDir, (int)otherRobotGear);

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
                            movementScript.Push(otherMovingDir, (int)otherRobotGear);



                        otherRobot.layer = oldLayer;

                    }
                }

                //Our robot is not moving
                else
                {
                    movementScript.Push(otherMovingDir, (int)otherRobotGear);
                }
            }

        }
    }
}
