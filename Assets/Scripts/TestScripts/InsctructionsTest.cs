using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsctructionsTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject robot;
    RobotInstructionScript instructionScript;
    void Start()
    {
        instructionScript = robot.GetComponent<RobotInstructionScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Test program to run on robot
        if(Input.GetKeyDown("space"))
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

            instructionScript.LoadInstructions(instructions);
            instructionScript.StartExecute();
        }

        if (Input.GetKeyDown(KeyCode.Return))
            instructionScript.StopExecute();


    }
}
