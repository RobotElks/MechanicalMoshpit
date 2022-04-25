using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    Vector3 positionTarget;
    public int tileSize;
    public float movementSpeed;
    bool startedMove = false;
    public float turnRate = 90.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, 90, 0);
        MoveBackwards();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving())
        {
            if ((transform.position - positionTarget).magnitude > 0.000001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionTarget, movementSpeed * Time.deltaTime);
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
}
