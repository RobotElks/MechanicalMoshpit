using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCollision : MonoBehaviour
{

    //Might need refactoring later (switch case)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }
    }

    private void PlayerCollision(Collision collision)
    {
        GameObject otherRobot = collision.gameObject;
        RobotMultiplayerMovement robotMovement = otherRobot.GetComponent<RobotMultiplayerMovement>();



        Debug.Log("Last position: " + robotMovement.GetLastPosition());
        Debug.Log("Last rotation: " + robotMovement.GetLastRotation());
        Debug.Log("Target position: " + robotMovement.GetTargetPosition());
        Debug.Log("Target rotation: " + robotMovement.GetTargetRotation());
        this.GetComponent<Rigidbody>().isKinematic = true;
        Debug.Log("------------------------------------");

    }
}
