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
        thisRobotMovementScript = this.GetComponent<RobotMultiplayerMovement>();
	}


    private void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Laser"))
        {
            LaserCollision(collider);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        //if (collider.CompareTag("wallX"))
        //{
        //    WallCollision(0);
        //}
        //else if (collider.CompareTag("wallZ"))
        //{
        //    WallCollision(1);
        //}
    }


    void OnCollisionExit (Collision collision) 
    {
        //if (collision.collider.CompareTag("Laser"))
        //{
        //    LaserCollision(collision);
        //}

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
        else if (collision.collider.CompareTag("TurnGearLeft"))
        {
            onTurnLeft = false;
        }
        else if (collision.collider.CompareTag("TurnGearRight"))
        {
            onTurnRight = false;
        }
    }

    private void CollisionCheck(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCollision(collision);
        }
        else if (collision.collider.CompareTag("wallX"))
        {
            WallCollision(0);
        }
        else if (collision.collider.CompareTag("wallZ"))
        {
            WallCollision(1);
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
            //Debug.Log("Robots moving direction : " + thisRobotMovementScript.GetMovingDirection());
            RobotMultiplayerMovement otherRobotMovementScript = robotCollision.gameObject.GetComponent<RobotMultiplayerMovement>();

            if (otherRobotMovementScript.IsMoving() || otherRobotMovementScript.IsPushed())
            {
                Vector3 otherRobotForceOnThis = otherRobotMovementScript.GetForceToMe(transform.position);
                thisRobotMovementScript.Push(otherRobotForceOnThis, (int)otherRobotForceOnThis.magnitude);
                //Debug.Log("Other robots force on this robot : " + otherRobotForceOnThis);
                if (otherRobotForceOnThis == Vector3.zero && thisRobotMovementScript.IsMoving())
                    if(WallOnOtherSide())
                        thisRobotMovementScript.Wall(-thisRobotMovementScript.GetMovingDirection(), 1);
            }
            else if (thisRobotMovementScript.IsMoving())
                if (WallOnOtherSide())
                    thisRobotMovementScript.Wall(-thisRobotMovementScript.GetMovingDirection(), 1);
        }
    }

    private void LaserCollision(Collider collider)
    {
        Destroy(collider.gameObject, 0f);
        playerHealthBarScript.GetHit(10);
        this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*200);

    }

    private void WallCollision(int wall){
        if (IsOwner)
        {
            // 0 := Wall X, 1 := Wall Z 
            if(wall == 1)
                thisRobotMovementScript.WallCollisionX();
            else
                thisRobotMovementScript.WallCollisionZ();

        }
    }
    private void TakeDamage(){
        playerHealthBarScript.GetHit(10);
    }

    public void Reset()
    {
        onConveyorBelt = false;
        onDamageTile = false;
        onEnergyStation = false;
        onHealthStation = false;
        onTurnLeft = false;
        onTurnRight = false;
        
    }

    // raycast above robots to check if wall is on other side (less than 1,5 tiles away) of pushed robot
    public bool WallOnOtherSide()
    {
        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        Vector3 dir = thisRobotMovementScript.GetMovingDirection();
        //Debug.Log("Raycast. Pos : " + pos + ", dir : " + dir);
        RaycastHit hit;
        if (Physics.Raycast(pos, dir, out hit))
        {
            //Debug.Log("Hit info. Hit tag : " + hit.collider.tag + ", Hit distance : " + hit.distance);
            // A WALL IS A TILE A WAY (BUT NOT NEXT TO ROBOT)
            return ((hit.collider.tag == "wallX" || hit.collider.tag == "wallZ") && (hit.distance > 0.8 && hit.distance < 1.5f));
        }
        else
            return false;
    }

}
