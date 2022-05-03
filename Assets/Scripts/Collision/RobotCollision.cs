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

        Vector3 otherForward = otherRobot.transform.forward;

        //Debug.Log("Last position: " + robotMovement.GetLastPosition());
        //Debug.Log("Last rotation: " + robotMovement.GetLastRotation());
        //Debug.Log("Target position: " + robotMovement.GetTargetPosition());
        //Debug.Log("Target rotation: " + robotMovement.GetTargetRotation());
        this.GetComponent<Rigidbody>().isKinematic = true;
        //Debug.Log(otherForward);
        Debug.Log("------------------------------------");

        Vector3 coll = transform.forward + otherForward;
        //Frontal Collision
        if((transform.forward + otherForward).magnitude < 0.001f) {
            //Check if other is moving or not 
        }

        else 
        {
            //List<ContactPoint> contactPoints = new List<ContactPoint>();
            //collision.GetContacts(contactPoints);

            //collision.collider.GetComponent<BoxCollider>().sc

            ContactPoint contact = collision.contacts[0];

            Debug.Log(transform.position);
            Debug.Log(contact.thisCollider) ;
        }

    }
}
