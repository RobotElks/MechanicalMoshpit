using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class RobotMultiplayerInstructionScript : NetworkBehaviour
{

    Queue<Instructions> instructionsQueue = new Queue<Instructions>();
    bool isExecuting = false;
    RobotMultiplayerMovement movementScript;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<RobotMultiplayerMovement>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //If the program is executing, and the robot is done with last instruction, run next instruction and place it last in que
            if (isExecuting && instructionsQueue.Count > 0 && !movementScript.IsDoingInstruction())
            {
                Instructions instruction = instructionsQueue.Dequeue();
                switch (instruction)
                {
                    case Instructions.MoveForward:
                        movementScript.MoveForward();
                        break;
                    case Instructions.MoveBackward:
                        movementScript.MoveBackwards();
                        break;
                    case Instructions.RotateLeft:
                        movementScript.RotateLeft();
                        break;
                    case Instructions.RotateRight:
                        movementScript.RotateRight();
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

                instructionsQueue.Enqueue(instruction);
            }


            //DEBUG
            if (Input.GetKeyDown("space"))
            {
                Queue<Instructions> instructions = new Queue<Instructions>();
                instructions.Enqueue(Instructions.MoveForward);
                instructions.Enqueue(Instructions.RotateRight);
                instructions.Enqueue(Instructions.MoveForward);
                instructions.Enqueue(Instructions.ThirdGear);

                instructions.Enqueue(Instructions.RotateLeft);
                instructions.Enqueue(Instructions.MoveBackward);

                instructions.Enqueue(Instructions.RotateRight);
                instructions.Enqueue(Instructions.MoveBackward);
                instructions.Enqueue(Instructions.RotateLeft);
                instructions.Enqueue(Instructions.FirstGear);

                LoadInstructions(instructions);
                StartExecute();
            }

            if (Input.GetKeyDown(KeyCode.Return))
                StopExecute();
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

}
