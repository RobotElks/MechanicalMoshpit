using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ReadyButtonScript : NetworkBehaviour
{
    RobotRoundsHandler robotScript;
    public RobotList robotList;

    public GameObject startButton;

    TextMeshProUGUI readyText;

    // Start is called before the first frame update
    private void Start()
    {
        if (!IsHost)
            startButton.SetActive(false);

        readyText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ClickReady()
    {


        if (readyText.text.ToLower().Contains("n"))
            readyText.text = "Ready";
        else
            readyText.text = "Not Ready";

        robotScript.ToggleReadyServerRpc();

    }



    public void SetRobotRoundsScript(RobotRoundsHandler robotScript)
    {
        this.robotScript = robotScript;
    }

    public void StartGame()
    {
        if (IsHost)
        {
            GameObject[] robots = robotList.GetRobots();

            foreach (GameObject robot in robots)
            {
                if (!robot.GetComponent<RobotRoundsHandler>().GetIsReady())
                    return;
            }

            robotScript.HostSetGameStateForAll(GameState.Countdown);
        }
    }
}
