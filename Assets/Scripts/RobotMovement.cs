using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gear
{
    First, Second, Third
}

public class RobotMovement : MonoBehaviour
{

    Vector3 positionTarget;
    Vector3 rotationTarget;
    public int tileSize;
    float movementSpeed;
    float movementGear;
    float rotateGear;
    public bool startedMove = false;
    public bool startedRotation = false;
    float turnRate = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        SetGear(Gear.First);
    }

    // Update is called once per frame
    void Update()
    {

        if (startedRotation)
        {
            if((transform.forward - rotationTarget).magnitude > 0.006f)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, rotationTarget, turnRate * rotateGear * Time.deltaTime, 0.0f));
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.RoundToInt(transform.eulerAngles.y), transform.eulerAngles.z);
                startedRotation = false;  
            }
        }


        else if (startedMove)
        {
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

    public void MoveForward()
    {
        positionTarget = transform.position + transform.forward * tileSize;
        startedMove = true;
    }

    public void MoveBackwards()
    {
        positionTarget = transform.position - transform.forward * tileSize;
        startedMove = true;
    }

    public bool IsMoving()
    {
        return startedMove;
    }

    public void RotateLeft()
    {
        rotationTarget = -transform.right;
        startedRotation = true;
    }

    public void RotateRight()
    {
        rotationTarget = transform.right;
        startedRotation = true;
    }

    public bool IsRotating()
    {
        return startedRotation;
    }

    public void SetGear(Gear newGear)
    {
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
