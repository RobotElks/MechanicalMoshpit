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
    public int tileSize;
    public float movementSpeed;
    public float gear;
    bool startedMove = false;
    public float turnRate = 90.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, 90, 0);
        MoveBackwards();
        //SetGear(Gear.First);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving())
        {
            if ((transform.position - positionTarget).magnitude > 0.000001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionTarget, movementSpeed * gear * Time.deltaTime);
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
       
    }

    public void RotateRight()
    {
        
    }

    public bool IsRotating()
    {
        return true;
    }

    public void SetGear(Gear newGear)
    {
        switch (newGear)
        {
            case Gear.First:
                gear = 1;
                break;
            case Gear.Second:
                gear = 2;
                break;
            case Gear.Third:
                gear = 3;
                break;
        }

    }
}
