using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotMultiplayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<Instructions> networkInstruction = new NetworkVariable<Instructions>();
    public NetworkVariable<Gear> networkGear = new NetworkVariable<Gear>();

    Vector3 positionTarget;
    Vector3 rotationTarget;

    Vector3 leftToPush = Vector3.zero;

    int tileSize = 1;
    public float movementSpeed = 1.0f;
    float pushedSpeed;
    float movementGear;
    float rotateGear;
    float turnRate = 1.0f;
    Vector3 lastPosition;
    Vector3 lastRotation;

    Gear currentGear = Gear.None;
    Instructions currentInstruction = Instructions.None;

    public override void OnNetworkSpawn()
    {
        //Set gear to third to calculate pushedSpeed
        SetGear(Gear.Third);
        pushedSpeed = movementSpeed * movementGear * 5f;
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
            //Rotation movement
            if (IsRotating())
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
                    currentInstruction = Instructions.None;
                }
            }

            //Driving movement
            else if (IsMoving())
            {
                //if the distance between the position target and the current position is larger than 0.000001, move towards it.
                if ((transform.position - positionTarget).magnitude > 0.000001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, positionTarget, movementSpeed * movementGear * Time.deltaTime);
                }
                else
                {
                    //transform.position = positionTarget;
                    leftToPush = Vector3.zero;
                    lastPosition = transform.position;
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
            UpdateNetworkInfoServerRpc(transform.position, transform.rotation, currentInstruction, currentGear);
        }

        //Update model postition and rotation to match network position
        else
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
            currentInstruction = networkInstruction.Value;
            currentGear = networkGear.Value;
        }

    }

    [ServerRpc]
    public void UpdateNetworkInfoServerRpc(Vector3 localPosition, Quaternion localRotation, Instructions localInstruction, Gear localGear)
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
        currentInstruction = Instructions.MoveForward;
    }
    //Call on function to move robot backwards in the direction it is facing.
    public void MoveBackwards()
    {
        if (IsDoingInstruction()) return;
        positionTarget = transform.position - transform.forward * tileSize;
        currentInstruction = Instructions.MoveBackward;
    }
    //Call on function to return whether robot is moving or not
    public bool IsMoving()
    {
        return currentInstruction == Instructions.MoveForward || currentInstruction == Instructions.MoveBackward;
    }

    //Call on function to rotate the robot 90 degrees to the left
    public void RotateLeft()
    {
        if (IsDoingInstruction()) return;

        rotationTarget = -transform.right;
        currentInstruction = Instructions.RotateLeft;

    }

    //Call on function to rotate the robot 90 degrees to the right
    public void RotateRight()
    {
        if (IsDoingInstruction()) return;
        rotationTarget = transform.right;
        currentInstruction = Instructions.RotateRight;
    }

    //Call on function to check whether robot is currently rotating
    public bool IsRotating()
    {
        return currentInstruction == Instructions.RotateLeft || currentInstruction == Instructions.RotateRight;
    }

    public void SetTileSize(int input)
    {
        tileSize = input;
    }

    public Gear GetGear()
    {
        return currentGear;
    }

    //Call on function to set gear to either First, Second or Third.
    public void SetGear(Gear newGear)
    {
        if (IsDoingInstruction()) return;

        currentGear = newGear;

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
        //else if (currentInstruction == Instructions.MoveForward)
        //    return transform.forward;
        else
            return transform.forward;
    }

    public void Push(Vector3 direction, int numOfTiles)
    {
        direction.y = 0;
        direction = direction.normalized;
        

        //leftToPush = direction * tileSize;

        transform.position += direction * tileSize * numOfTiles;
    }

    public void ResetToLastPosition()
    {
        positionTarget = lastPosition;
    }

    public void SetSpawn(Vector3 point)
    {
        transform.position = point;
        lastPosition = point;
    }

    public void MoveTargetPositionBack(int numOfTiles)
    {
        positionTarget -= GetMovingDirection() * tileSize * numOfTiles;
    }

    private void SetTargetPosition(Vector3 newTargetPostion)
    {

    }


}
