using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotCollision : NetworkBehaviour
{
    PlayerHealthBar playerHealthBarScript;
    RobotMultiplayerMovement thisRobotMovementScript;
    public HealthStationScript healthStationScript;
    public bool onHealthStation = false;
    public bool onEnergyStation = false;
    public bool onConveyorBelt = false;
    public bool onTurnLeft = false;
    public bool onTurnRight = false;
    public bool onDamageTile = false;


	void Start () {
		playerHealthBarScript = this.GetComponentInChildren<PlayerHealthBar>();
        thisRobotMovementScript = GetComponent<RobotMultiplayerMovement>();
	}


    private void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }

    void OnCollisionExit (Collision collision) 
    {
        if (collision.collider.CompareTag("Laser"))
        {
            LaserCollision(collision);
        }

        if (collision.collider.CompareTag("HealthStation"))
        {
            onHealthStation = false;
        }

        else if (collision.collider.CompareTag("EnergyStation"))
        {
            onEnergyStation = false;
        }

        else if (collision.collider.CompareTag("DamageTile"))
        {
            onDamageTile = false;
        }
    }

    private void CollisionCheck(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }

        else if (collision.collider.CompareTag("Wall"))
        {
            WallCollision(collision);
        }
        else if (collision.collider.CompareTag("HealthStation"))
        {
            healthStationScript = collision.collider.GetComponent<HealthStationScript>();
            if(healthStationScript.GetIfAvailable()){
                onHealthStation = true;
            }
            
        }
        else if (collision.collider.CompareTag("EnergyStation"))
        {
            onEnergyStation = true;
        }
        else if (collision.collider.CompareTag("DamageTile"))
        {
            onDamageTile = true;
        }
        else if (collision.collider.CompareTag("ConveyorBelt"))
        {
            onConveyorBelt = true;
            thisRobotMovementScript.SetDirection(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("TurnGearLeft"))
        {
            onTurnLeft = true;
        }
        else if (collision.collider.CompareTag("TurnGearRight"))
        {
            onTurnRight = true;
        }
        

    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit){
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

    private void LaserCollision(Collision collider){
        Destroy(collider.gameObject, 0f);
        playerHealthBarScript.GetHit(10);
        
    }

    private void WallCollision(Collision hitWall){
        if (IsOwner)
        {
            thisRobotMovementScript.MoveTargetPositionBack(1);
        }
    }
    private void TakeDamage(){
        playerHealthBarScript.GetHit(10);
    }

}
