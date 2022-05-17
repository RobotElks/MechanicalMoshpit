using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Instructions to run on robots
public enum Instructions
{
    MoveForward, MoveBackward, RotateLeft, RotateRight, FirstGear, SecondGear, ThirdGear, None, MapMovement
}

public class RobotInstructionScript : MonoBehaviour
{

    Queue<Instructions> instructionsQueue = new Queue<Instructions>();
    bool isExecuting = false;
    RobotMovement movementScript;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<RobotMovement>();
    }

    // Update is called once per frame
    void Update()
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
