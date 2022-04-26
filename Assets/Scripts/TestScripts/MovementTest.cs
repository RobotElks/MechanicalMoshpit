using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovementTest : MonoBehaviour
{
    public GameObject playerRobot;
    RobotMovement movementScript;
    // Start is called before the first frame update
    void Start()
    {
        playerRobot = GameObject.Find("Robot");
        movementScript = playerRobot.GetComponent<RobotMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w")) {
            movementScript.MoveForward();
        }
        if (Input.GetKeyDown("s"))
            movementScript.MoveBackwards();

        if (Input.GetKeyDown("a"))
            movementScript.RotateLeft();

        if (Input.GetKeyDown("d"))
            movementScript.RotateRight();

        if (Input.GetKeyDown("1"))
            movementScript.SetGear(Gear.First);
        if (Input.GetKeyDown("2"))
            movementScript.SetGear(Gear.Second);
        if (Input.GetKeyDown("3"))
            movementScript.SetGear(Gear.Third);
    }
}
