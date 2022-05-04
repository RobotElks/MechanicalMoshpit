using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class ProgramMuiltiplayerRobot : MonoBehaviour
{
    Queue<Instructions> instructionsQueue = new Queue<Instructions>();

    public Slider gear;
    private float gearValue;
    public TextMeshProUGUI textInstructions;

    public Button removeButton;
    public Button runButton;
    public Button stopButton;

    public Button moveForwardButton;
    string moveForwardString = "Move Forward ";

    public Button moveBackwardButton;
    string moveBackwardString = "Move Backward ";

    public Button turnRightButton;
    string turnRightString = "Turn Right ";

    public Button turnLeftButton;
    string turnLeftString = "Turn Left ";

    public RobotMultiplayerInstructionScript instructionScript;

    void Start()
    {
        runButton = runButton.GetComponent<Button>();
        runButton.onClick.AddListener(sendProgramToRobot);

        stopButton = stopButton.GetComponent<Button>();
        stopButton.onClick.AddListener(stopProgram);

        removeButton = removeButton.GetComponent<Button>();
        removeButton.onClick.AddListener(removeLastInstruction);

        // moveForwardButton = moveForwardButton.GetComponent<Button>();
        // moveForwardButton.onClick.AddListener(addInstructionToProgram(moveForwardString, gearValue));

        moveForwardButton = moveForwardButton.GetComponent<Button>();
        moveForwardButton.onClick.AddListener(addMoveForwardToProgram);

        moveBackwardButton = moveBackwardButton.GetComponent<Button>();
        moveBackwardButton.onClick.AddListener(addMoveBackwardToProgram);

        turnRightButton = turnRightButton.GetComponent<Button>();
        turnRightButton.onClick.AddListener(addTurnRightToProgram);

        turnLeftButton = turnLeftButton.GetComponent<Button>();
        turnLeftButton.onClick.AddListener(addTurnLeftToProgram);

    }

    void Update()
    {
        gearValue = gear.GetComponent<Slider>().value;
    }
    // void addInstructionToProgram(string instruction, float gearValue)
    // {
    //     textInstructions.text = textInstructions.text + instruction + gearValue + "\n";
    // }

    void addMoveForwardToProgram()
    {
        textInstructions.text = textInstructions.text + moveForwardString + gearValue + "\n";
    }

    void addMoveBackwardToProgram()
    {
        textInstructions.text = textInstructions.text + moveBackwardString + gearValue + "\n";
    }

    void addTurnRightToProgram()
    {
        textInstructions.text = textInstructions.text + turnRightString + gearValue + "\n";
    }

    void addTurnLeftToProgram()
    {
        textInstructions.text = textInstructions.text + turnLeftString + gearValue + "\n";
    }

    void removeLastInstruction()
    {
        string[] seperateInstructions = textInstructions.text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        seperateInstructions = seperateInstructions.SkipLast(1).ToArray();
        string instructionsString = String.Join("\n", seperateInstructions);
        textInstructions.text = instructionsString;
        if (textInstructions.text.Length != 0)
        {
            textInstructions.text = textInstructions.text + "\n";
        }
    }

    /// <summary>
    /// Takes string instruction and return the method for the instrucion for the queue
    /// </summary>
    private Instructions getInstruction(String subInstruction)
    {
        Instructions instruction = default;

        switch (subInstruction)
        {
            case "Forward":
                instruction = Instructions.MoveForward;
                break;
            case "Backward":
                instruction = Instructions.MoveBackward;
                break;
            case "Right":
                instruction = Instructions.RotateRight;
                break;
            case "Left":
                instruction = Instructions.RotateLeft;
                break;
            case "1":
                instruction = Instructions.FirstGear;
                break;
            case "2":
                instruction = Instructions.SecondGear;
                break;
            case "3":
                instruction = Instructions.ThirdGear;
                break;
        }
        return instruction;
    }

    /// <summary>
    /// Enqueue the instructions and load them into the robot and then executes the program
    /// </summary>
    public void sendProgramToRobot()
    {
        instructionsQueue.Clear();
        enqueueProgram();
        instructionScript.LoadInstructions(instructionsQueue);
        instructionScript.StartExecute();
    }

    /// <summary>
    /// Convert the program list into a queue of method instructions
    /// </summary>
    private void enqueueProgram()
    {
        String[] program = textInstructions.text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        foreach (string instruction in program)
        {
            String[] subInstruction = instruction.Split(' ');
            instructionsQueue.Enqueue(getInstruction(subInstruction[2]));
            instructionsQueue.Enqueue(getInstruction(subInstruction[1]));
        }
    }

    public void stopProgram()
    {
        instructionScript.StopExecute();
        instructionsQueue.Clear();
    }


}
