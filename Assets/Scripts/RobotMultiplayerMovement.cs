using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotMultiplayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    Vector3 positionTarget;
    Vector3 rotationTarget;
    int tileSize = 5;
    public float movementSpeed = 5.0f;
    float movementGear;
    float rotateGear;
    bool startedMove = false;
    public bool startedRotation = false;
    float turnRate = 1.0f;

    public override void OnNetworkSpawn()
    {
        SetGear(Gear.First);

        if (IsOwner)
            GameObject.Find("Main Camera").GetComponent<CameraMultiplayer>().SetLocalPlayer(transform);

    }

    // Update is called once per frame
    void Update()
    {
        //Local player owns the object
        if (IsOwner)
        {
            if (Input.GetKeyDown("w"))
                MoveForward();
            if (Input.GetKeyDown("s"))
                MoveBackwards();
            if (Input.GetKeyDown("a"))
                RotateLeft();
            if (Input.GetKeyDown("d"))
                RotateRight();

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


            UpdateNetworkInfoServerRpc(transform.position, transform.rotation);
        }

        //Update model postition and rotation to match network position

        else
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
        }

    }

    [ServerRpc]
    public void UpdateNetworkInfoServerRpc(Vector3 localPosition, Quaternion localRotation)
    {
        networkPosition.Value = localPosition;
        networkRotation.Value = localRotation;
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
