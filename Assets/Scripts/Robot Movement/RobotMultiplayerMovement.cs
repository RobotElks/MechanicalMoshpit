using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public enum StateOfAnimation {Idle, Forward, Backward, Right, Left, Death, Hit};

public class RobotMultiplayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<Instructions> networkInstruction = new NetworkVariable<Instructions>();
    public NetworkVariable<Gear> networkGear = new NetworkVariable<Gear>();
    public NetworkVariable<Vector3> networkLeftToPush = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> networkAnimation = new NetworkVariable<bool>();
    public NetworkVariable<StateOfAnimation> animationState = new NetworkVariable<StateOfAnimation>(); 

    Rigidbody rb;
    public GameObject direction;
    Animator animator;
    Dead deadScript;

    Vector3 positionTarget;
    Vector3 rotationTarget;
    Vector3 lastValidPosition;
    Vector3 leftToPush = Vector3.zero;

    int tileSize = 1;
    public float movementSpeed = 1.0f;
    float pushedSpeed;
    float movementGear;
    float rotateGear;
    float turnRate = 1.0f;
    public StateOfAnimation localAnimationState = StateOfAnimation.Idle;
    Gear currentGear = Gear.None;
    Slider gearSlider;
    Instructions currentInstruction = Instructions.None;

    public override void OnNetworkSpawn()
    {
        gearSlider = GameObject.Find("ProgrammingInterface Multiplayer Variant").GetComponentInChildren<Slider>();

        //Set gear to third to calculate pushedSpeed
        SetGear(Gear.Third);
        pushedSpeed = movementSpeed * movementGear * 2f;
        SetGear(Gear.First);
        animator = GetComponent<Animator>();
        deadScript = GetComponent<Dead>();
        if (IsOwner)
        {
            GameObject.Find("Main Camera").GetComponent<CameraMultiplayer>().SetLocalPlayer(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("transitionDecider", (int)localAnimationState);

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
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.RoundToInt(transform.eulerAngles.y), transform.eulerAngles.z); ;
                    currentInstruction = Instructions.None;
                    localAnimationState = StateOfAnimation.Idle;

                }
            }

            //Driving movement
            else if (IsMoving())
            {
                //if the distance between the position target and the current position is larger than 0.000001, move towards it.
                Vector3 target = new Vector3(positionTarget.x, transform.position.y, positionTarget.z);
                if ((transform.position - target).magnitude > 0.000001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * movementGear * Time.deltaTime);
                }
                else
                {
                    //transform.position = positionTarget;
                    currentInstruction = Instructions.None;
                    localAnimationState = StateOfAnimation.Idle;
                }

            }
            else if(!deadScript.IsDead())
            {
                localAnimationState = StateOfAnimation.Idle;
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
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = false;
            UpdateNetworkInfoServerRpc(transform.position, transform.rotation, currentInstruction, 
                currentGear, leftToPush, localAnimationState);
            rb.freezeRotation = true;

            if (positionTarget == transform.position)
            {
                lastValidPosition = positionTarget;
                //Debug.Log("New valid position : " + lastValidPosition);
            }
        }

        //Update model postition and rotation to match network position
        else
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
            currentInstruction = networkInstruction.Value;
            currentGear = networkGear.Value;
            leftToPush = networkLeftToPush.Value;
            localAnimationState = animationState.Value;
        }
    }

    [ServerRpc]
    public void UpdateNetworkInfoServerRpc(Vector3 localPosition, Quaternion localRotation, 
        Instructions localInstruction, Gear localGear, Vector3 localLeftToPush, StateOfAnimation AnimationState)
    {
        networkPosition.Value = localPosition;
        networkRotation.Value = localRotation;
        networkInstruction.Value = localInstruction;
        networkGear.Value = localGear;
        networkLeftToPush.Value = localLeftToPush;
        animationState.Value = AnimationState;
    }

    public Vector3 GetTargetPosition()
    {
        return positionTarget;
    }

    public void ResetTargetPosition(Vector3 positionTarget)
    {
        this.leftToPush = Vector3.zero;
        this.positionTarget = positionTarget;
    }

    public Vector3 GetTargetRotation()
    {
        return rotationTarget;
    }
    public bool IsDoingInstruction()
    {
        return (IsRotating() || IsMoving());
    }

    public bool IsPushed()
    {
        return leftToPush.magnitude != 0;
    }

    //Call on function to move robot forward in the direction it is facing
    public void MoveForward()
    {
        if (IsDoingInstruction()) return;
        if(IsOwner)
            localAnimationState = StateOfAnimation.Forward;
        positionTarget = transform.position + transform.forward * tileSize;
        currentInstruction = Instructions.MoveForward;
    }
    //Call on function to move robot backwards in the direction it is facing.
    public void MoveBackwards()
    {
        if (IsDoingInstruction()) return;
        if (IsOwner)
            localAnimationState = StateOfAnimation.Backward;
        positionTarget = transform.position - transform.forward * tileSize;
        currentInstruction = Instructions.MoveBackward;
    }

    public void SetDirection(GameObject Direction)
    {
        direction = Direction;
    }

    public void MoveDirection()
    {
        if (IsDoingInstruction()) return;
        positionTarget = transform.position + direction.transform.forward * tileSize;
        currentInstruction = Instructions.MapMovement;
    }
    //Call on function to return whether robot is moving or not
    public bool IsMoving()
    {
        return currentInstruction == Instructions.MoveForward || currentInstruction == Instructions.MoveBackward || currentInstruction == Instructions.MapMovement;
    }

    //Call on function to rotate the robot 90 degrees to the left
    public void RotateLeft()
    {
        if (IsDoingInstruction()) return;
        if (IsOwner)
            localAnimationState = StateOfAnimation.Left;
        rotationTarget = -transform.right;
        currentInstruction = Instructions.RotateLeft;

    }

    //Call on function to rotate the robot 90 degrees to the right
    public void RotateRight()
    {
        if (IsDoingInstruction()) return;
        if (IsOwner)
            localAnimationState = StateOfAnimation.Right;
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

    public void MoveToSpawnPoints(Vector3 spawnPoint)
    {
        GetComponent<RobotCollision>().Reset();
        leftToPush = Vector3.zero;
        positionTarget = spawnPoint;
        transform.position = spawnPoint;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        SetGear(Gear.First);
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
                gearSlider.value = 1f;
                break;
            case Gear.Second:
                movementGear = 1.5f;
                rotateGear = 1.5f;
                gearSlider.value = 2f;
                break;
            case Gear.Third:
                movementGear = 2f;
                rotateGear = 2;
                gearSlider.value = 3f;
                break;
        }
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

    public void Push(Vector3 direction, int numOfTiles)
    {
        direction.y = 0;
        direction = direction.normalized;
        //Debug.Log("New push force. Direction: " + direction + ", Magnitude: " + numOfTiles);

        transform.position += direction * 0.3f;
        positionTarget += direction * 0.3f;

        leftToPush += direction * tileSize * numOfTiles - direction * 0.3f;
    }

    // Gives less instant pushback than Push()
    public void Wall(Vector3 direction, int numOfTiles)
    {
        direction.y = 0;
        direction = direction.normalized;

        transform.position += direction * 0.2f;
        positionTarget += direction * 0.2f;

        leftToPush += direction * tileSize * numOfTiles - direction * 0.2f;
    }

    public void WallCollisionX()
    {
        // Inverse push-force if pushed, else move back
        if (IsPushed())
        {
            int force = Mathf.CeilToInt(Mathf.Abs(leftToPush.x));
            //Debug.Log("X Force from push : " + force);
            if (leftToPush.x > 0.05)
                Wall(new Vector3(-1, 0, 0), force);
            else if (leftToPush.x < -0.05)
                Wall(new Vector3(1, 0, 0), force);
            if (IsMoving())
            {
                // some sort of hotfix to prevent wall walking
                MoveBackPosition();
            }
        }
        else
        {
            int movingDir = (int)GetMovingDirection().x;
            //Debug.Log("X movingDir : " + movingDir);
            int force = Mathf.Abs(movingDir);
            if (force > 0.05)
                MoveBackPosition();
        }
    }

    public void WallCollisionZ()
    {
        // Inverse push-force if pushed, else move back
        if (IsPushed())
        {
            int force = Mathf.CeilToInt(Mathf.Abs(leftToPush.z));
            //Debug.Log("Z Force from push : " + force);
            if (leftToPush.z > 0.05)
                Wall(new Vector3(0, 0, -1), force);
            else if (leftToPush.z < -0.05)
                Wall(new Vector3(0, 0, 1), force);

            if (IsMoving())
            {
                // some sort of hotfix to prevent wall walking
                MoveBackPosition();
            }
        }
        else
        {
            int movingDir = (int)GetMovingDirection().z;
            //Debug.Log("Z movingDir : " + movingDir);
            int force = Mathf.Abs(movingDir);
            if (force > 0.05)
                MoveBackPosition();
        }
    }

    public void MoveBackPosition()
    {
        positionTarget -= GetMovingDirection() * tileSize;
    }

    public Vector3 GetForceToMe(Vector3 myPosition)
    {
        Vector3 posDiff = (myPosition - transform.position);
        Vector3 movDir = GetMovingDirection();
        posDiff.y = 0;
        posDiff = posDiff.normalized;
        float max = Mathf.Max(Mathf.Abs(posDiff.x), Mathf.Abs(posDiff.z));

        if (Mathf.Abs(posDiff.x) == max)
        {
            posDiff.x = Mathf.Sign(posDiff.x);
            posDiff.z = 0;
        }
        else
        {
            posDiff.z = Mathf.Sign(posDiff.z);
            posDiff.x = 0;
        }

        Vector3 force = Vector3.zero;

        if (IsPushed() && Vector3.Dot(leftToPush.normalized, posDiff) > 0)
        {
            force = posDiff;

            if (IsMoving() && Vector3.Dot(movDir, force) > 0.95f)
                force *= (int)currentGear;

        }

        else if (IsMoving() && Vector3.Dot(movDir, posDiff) > 0.95f)
            force = movDir * (int)currentGear;
        else if (GetCurrentInstruction() == Instructions.MapMovement)
            force = posDiff;

        return force;
    }

    public void SetAnimation(StateOfAnimation newState)
    {
        this.localAnimationState = newState;
    }
    
    public StateOfAnimation GetAnimation()
    {
        return localAnimationState;
    }
}
