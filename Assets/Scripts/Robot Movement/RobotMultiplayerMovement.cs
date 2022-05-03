using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotMultiplayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<Instructions> networkInstruction = new NetworkVariable<Instructions>();
    public NetworkVariable<float> networkGear = new NetworkVariable<float>();

    Vector3 positionTarget;
    Vector3 rotationTarget;

    Vector3 leftToPush = Vector3.zero;

    int tileSize = 1;
    public float movementSpeed = 1.0f;
    float pushedSpeed;
    float movementGear;
    float rotateGear;
    bool startedMove = false;
    bool startedRotation = false;
    float turnRate = 1.0f;
    Vector3 lastPosition;
    Vector3 lastRotation;

    public Instructions currentInstruction = Instructions.None;

    public override void OnNetworkSpawn()
    {
        //Set gear to third to calculate pushedSpeed
        SetGear(Gear.Third);
        pushedSpeed = movementSpeed * movementGear * 1.5f;
        SetGear(Gear.First);


        if (IsOwner)
            GameObject.Find("Main Camera").GetComponent<CameraMultiplayer>().SetLocalPlayer(transform);
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(GetRobotMiddle(), transform.forward, Color.red);
        Debug.DrawLine(this.transform.position, this.transform.position + transform.forward, Color.red);

        //Local player owns the object
        if (IsOwner)
        {
            //if (Input.GetKeyDown("w"))
            //    MoveForward();
            //if (Input.GetKeyDown("s"))
            //    MoveBackwards();
            //if (Input.GetKeyDown("a"))
            //    RotateLeft();
            //if (Input.GetKeyDown("d"))
            //    RotateRight();

            //Rotation movement
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
                    lastRotation = new Vector3(transform.eulerAngles.x, Mathf.RoundToInt(transform.eulerAngles.y), transform.eulerAngles.z);
                    transform.eulerAngles = lastRotation;
                    startedRotation = false;
                    currentInstruction = Instructions.None;
                }
            }

            //Driving movement
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
                    lastPosition = transform.position;
                    startedMove = false;
                    currentInstruction = Instructions.None;
                }

            }


            //Push movement
            if (leftToPush.magnitude > 0)
            {
                Vector3 pushDistance = pushedSpeed * leftToPush.normalized * Time.deltaTime;

                if (pushDistance.magnitude > leftToPush.magnitude)
                    pushDistance = leftToPush;

                transform.position += pushDistance;
                positionTarget += pushDistance;

                leftToPush -= pushDistance;
            }



            //Send current local information to the server to update other clients
            UpdateNetworkInfoServerRpc(transform.position, transform.rotation, currentInstruction, movementGear);
        }

        //Update model postition and rotation to match network position
        else
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
            currentInstruction = networkInstruction.Value;
            movementGear = networkGear.Value;
        }

    }

    [ServerRpc]
    public void UpdateNetworkInfoServerRpc(Vector3 localPosition, Quaternion localRotation, Instructions localInstruction, float localGear)
    {
        networkPosition.Value = localPosition;
        networkRotation.Value = localRotation;
        networkInstruction.Value = localInstruction;
        networkGear.Value = localGear;
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    public Vector3 GetLastRotation()
    {
        return lastRotation;
    }

    public Vector3 GetTargetPosition()
    {
        return positionTarget;
    }

    public Vector3 GetTargetRotation()
    {
        return rotationTarget;
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
        currentInstruction = Instructions.MoveForward;
    }
    //Call on function to move robot backwards in the direction it is facing.
    public void MoveBackwards()
    {
        if (IsDoingInstruction()) return;
        positionTarget = transform.position - transform.forward * tileSize;
        startedMove = true;
        currentInstruction = Instructions.MoveBackward;
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
        currentInstruction = Instructions.RotateLeft;

    }

    //Call on function to rotate the robot 90 degrees to the right
    public void RotateRight()
    {
        if (IsDoingInstruction()) return;
        rotationTarget = transform.right;
        startedRotation = true;
        currentInstruction = Instructions.RotateRight;
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

    public float GetGear()
    {
        return movementGear;
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
                movementGear = 1.5f;
                rotateGear = 1.5f;
                break;
            case Gear.Third:
                movementGear = 2f;
                rotateGear = 2;
                break;
        }
    }

    public Vector3 GetRobotMiddle()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        return transform.position - new Vector3(0, boxCollider.size.y / 2, 0);
    }

    public Instructions GetCurrentInstruction()
    {
        return currentInstruction;
    }

    public Vector3 GetMovingDirection()
    {
        if (currentInstruction == Instructions.MoveBackward)
            return -transform.forward;
        else
            return transform.forward;
    }

    public void Push(Vector3 direction)
    {
        direction = direction.normalized;
        direction.x = Mathf.Round(direction.x);
        direction.z = Mathf.Round(direction.z);
        direction.y = 0;

        leftToPush = direction * tileSize;
    }
}
