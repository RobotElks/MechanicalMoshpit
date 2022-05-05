using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    RobotMultiplayerMovement thisRobotMovementScript;

    int singleRobotRaycastMask;

    private void Start()
    {
        singleRobotRaycastMask = ~LayerMask.NameToLayer("RobotsRaycast");
        thisRobotMovementScript = GetComponent<RobotMultiplayerMovement>();
    }

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
            Debug.Log("-----------------------------------------------------");

            GameObject otherRobot = robotCollision.gameObject;
            RobotMultiplayerMovement otherRobotMovementScript = otherRobot.GetComponent<RobotMultiplayerMovement>();

            Vector3 otherRobotForceOnThis = otherRobotMovementScript.GetForceToMe(transform.position);

            Debug.Log("OtherMove: " + otherRobotMovementScript.IsMoving());
            Debug.Log("OtherPushed: " + otherRobotMovementScript.IsPushed());
            Debug.Log("Force: " + otherRobotForceOnThis);

            if (otherRobotMovementScript.IsMoving() || otherRobotMovementScript.IsPushed())
            {
                Vector3 otherMovingDir = otherRobotMovementScript.GetMovingDirection();
                Gear otherRobotGear = otherRobotMovementScript.GetGear();


                //Our robot is moving
                if (thisRobotMovementScript.IsMoving() || thisRobotMovementScript.IsPushed())
                {

                    Vector3 thisMovingDir = thisRobotMovementScript.GetMovingDirection();

                    //Got hit in our moving direction
                    if ((thisMovingDir + otherRobotForceOnThis.normalized).magnitude < 0.001f)
                    {
                        //Do priority check
                        //Debug.Log("Heads on");

                        //movementScript.MoveTargetPositionBack((int)otherRobotGear);
                        thisRobotMovementScript.Push(otherRobotForceOnThis, (int)otherRobotForceOnThis.magnitude);
                    }


                    ////Side collision
                    //else
                    //{
                    //    //Change to multiple rays from the robot

                    //    int oldLayer = otherRobot.layer;
                    //    otherRobot.layer = LayerMask.NameToLayer("RobotsRaycast");

                    //    CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

                    //    bool colliderInfront = Physics.Raycast(transform.position - transform.right * capsuleCollider.radius, thisMovingDir, capsuleCollider.radius * 3, singleRobotRaycastMask);

                    //    if (!colliderInfront)
                    //        Physics.Raycast(transform.position + transform.right * capsuleCollider.radius, thisMovingDir, capsuleCollider.radius * 3, singleRobotRaycastMask);


                    //    //Got hit from the side
                    //    if (!colliderInfront)
                    //        thisRobotMovementScript.Push(otherMovingDir, (int)otherRobotGear);



                    //    otherRobot.layer = oldLayer;

                    //}
                }

                //Our robot is not moving
                else
                {
                    thisRobotMovementScript.Push(otherRobotForceOnThis, (int)otherRobotForceOnThis.magnitude);
                }
            }
        }
    }
}
