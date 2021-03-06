using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class RobotMultiplayerInstructionScript : NetworkBehaviour
{

    Queue<Instructions> instructionsQueue = new Queue<Instructions>();
    bool isExecuting = false;
    bool hasRotated = false;
    RobotMultiplayerMovement movementScript;
    RobotCollision collisionScript;
    PlayerHealthBar healthScript;
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();
        collisionScript = GetComponent<RobotCollision>();
        healthScript = GetComponentInChildren<PlayerHealthBar>();

        if(IsOwner)
        {
            GameObject.Find("ProgrammingInterface Multiplayer Variant").GetComponent<ProgramMuiltiplayerRobot>().instructionScript = this;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if(isExecuting && !movementScript.IsDoingInstruction())
            {
                if(collisionScript.onConveyorBelt){
                    movementScript.MoveDirection();
                    //Debug.Log("MOVING FROM BELT: " + movementScript.IsMoving());
                    collisionScript.onConveyorBelt = false;
                    return;
                }
                else if(collisionScript.onTurnLeft && !hasRotated){
                    hasRotated = true;
                    movementScript.RotateLeft();
                    return;
                }
                else if(collisionScript.onTurnRight && !hasRotated){
                    hasRotated = true;
                    movementScript.RotateRight();
                    return;
                }
                
                if (instructionsQueue.Count > 0)
                {
                    if (collisionScript.onDamageTile)
                    {
                        healthScript.DamageTile();
                    }
                    Instructions instruction = instructionsQueue.Dequeue();
                    switch (instruction)
                    {
                        case Instructions.MoveForward:
                            movementScript.MoveForward();
                            hasRotated = false;
                            break;
                        case Instructions.MoveBackward:
                            movementScript.MoveBackwards();
                            hasRotated = false;
                            break;
                        case Instructions.RotateLeft:
                            movementScript.RotateLeft();
                            hasRotated = false;
                            break;
                        case Instructions.RotateRight:
                            movementScript.RotateRight();
                            hasRotated = false;
                            break;
                        case Instructions.FirstGear:
                            movementScript.SetGear(Gear.First);
                            break;
                        case Instructions.SecondGear:
                            movementScript.SetGear(Gear.Second);
                            break;
                        case Instructions.ThirdGear:
                            movementScript.SetGear(Gear.Third);
                            break;
                    }

                }
            }
            //If the program is executing, and the robot is done with last instruction, run next instruction and place it last in que

        }


    }

    /// <summary>
    /// Set program for the robot to run
    /// </summary>
    /// <param name="instructions"></param>
    public void LoadInstructions(Queue<Instructions> instructions)
    {
        instructionsQueue = new Queue<Instructions>(instructions);
    }

    /// <summary>
    /// Returns true if the robot is currently executing its program
    /// </summary>
    /// <returns></returns>
    public bool IsExecuting()
    {
        return isExecuting;
    }

    /// <summary>
    /// Start executing current program
    /// </summary>
    public void StartExecute()
    {
        isExecuting = true;
    }

    /// <summary>
    /// Stop executing current program
    /// </summary>
    public void StopExecute()
    {
        isExecuting = false;
    }

    public void ClearInstructions()
    {
        instructionsQueue.Clear();
    }

}
