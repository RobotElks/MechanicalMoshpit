using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    RobotMultiplayerMovement thisRobotMovementScript;

    private void Start()
    {
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
}
