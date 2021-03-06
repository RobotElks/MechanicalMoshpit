using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Robots only have first, second and third gear
public enum Gear
{
    None, First, Second, Third
}

public class RobotMovement : MonoBehaviour
{

    Vector3 positionTarget;
    Vector3 rotationTarget;
    public int tileSize;
    public float movementSpeed = 5.0f;
    float movementGear;
    float rotateGear;
    public bool startedMove = false;
    public bool startedRotation = false;
    float turnRate = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //start on first gear
        SetGear(Gear.First);
    }

    // Update is called once per frame
    void Update()
    {
        //Rotation and movement can never occur at the same time
        if (startedRotation)
        {
            //move if distant between rotation target and current rotation coordinates is larger than 0.006
            if ((transform.forward - rotationTarget).magnitude > 0.006f)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, rotationTarget, turnRate * rotateGear * Time.deltaTime, 0.0f));
            }
            //set rotation angle to the target angle and disable rotation
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.RoundToInt(transform.eulerAngles.y), transform.eulerAngles.z);
                startedRotation = false;
            }
        }


        else if (startedMove)
        {
            //if the distance between the position target and the current position is larger than 0.000001, move towards it.
            if ((transform.position - positionTarget).magnitude > 0.000001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionTarget, movementSpeed * movementGear * Time.deltaTime);
            }
            else
            {
                //transform.position = positionTarget;
                startedMove = false;

            }
        }

    }

    public bool IsDoingInstruction()
    {
        return (IsRotating() || IsMoving());
    }

    //Call on function to move robot forward in the direction it is facing
    public void MoveForward()
    {
        if (IsDoingInstruction()) return;
        positionTarget = transform.position + transform.forward * tileSize;
        startedMove = true;
    }
    //Call on function to move robot backwards in the direction it is facing.
    public void MoveBackwards()
    {
        if (IsDoingInstruction()) return;
        positionTarget = transform.position - transform.forward * tileSize;
        startedMove = true;
    }
    //Call on function to return whether robot is moving or not
    public bool IsMoving()
    {
        return startedMove;
    }

    //Call on function to rotate the robot 90 degrees to the left
    public void RotateLeft()
    {
        if (IsDoingInstruction()) return;

        rotationTarget = -transform.right;
        startedRotation = true;
    }

    //Call on function to rotate the robot 90 degrees to the right
    public void RotateRight()
    {
        if (IsDoingInstruction()) return;
        rotationTarget = transform.right;
        startedRotation = true;
    }

    //Call on function to check whether robot is currently rotating
    public bool IsRotating()
    {
        return startedRotation;
    }

    public void SetTileSize(int input)
    {
        tileSize = input;
    }

    //Call on function to set gear to either First, Second or Third.
    public void SetGear(Gear newGear)
    {
        if (IsDoingInstruction()) return;

        switch (newGear)
        {
            case Gear.First:
                movementGear = 1;
                rotateGear = 1;
                break;
            case Gear.Second:
                movementGear = 2;
                rotateGear = 1.5f;
                break;
            case Gear.Third:
                movementGear = 3;
                rotateGear = 2;
                break;
        }

    }
}
