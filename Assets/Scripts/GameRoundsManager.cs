using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameRoundsManager : MonoBehaviour
{
    public float programmingTime = 10f;
    public float executionTime = 10f;
    public int counterTime = 5;
    public float finishedProgrammingTime = 5f;

    public bool isProgramming = false;
    public bool isExecuting = false;
    public bool isCountdowning = false;
    public bool hasStarted = false;

    public float timer;

    MultiplayerWorldParse worldScript;
    MultiplayerLevelInfo levelInfo;

    public GameObject countdown;
    public TextMeshProUGUI countdownText;
    public GameObject robot;
    public GameObject readyButton;
    public GameObject readyButtonText;
    public GameObject startEarlyButton;
    public GameObject programmingInterface;
    public GameObject finnishedButton;
    public GameObject runButton;
    public GameObject stopButton;


    private bool isReady = false;

    public List<GameObject> notReady = new List<GameObject>();
    public List<GameObject> Ready = new List<GameObject>();

    public void ToggleReady()
    {
        if(isReady)
        {
            NotReadyForGame();
            readyButtonText.GetComponent<TextMeshProUGUI>().text = "Ready";
            isReady = false;
        }
        else{
            ReadyForGame();
            readyButtonText.GetComponent<TextMeshProUGUI>().text = "Not Ready";
            isReady = true; 
        }

    }

    public void ReadyForGame()
    {
        foreach (GameObject robot in notReady)
        {
            levelInfo = robot.GetComponent<MultiplayerLevelInfo>();
            if(levelInfo.SetReady()) break;
        }  
    }

    public void NotReadyForGame()
    {
        Debug.Log("Set not ready plz");
        foreach (GameObject robot in Ready)
        {
            Debug.Log("a robot in ready");
            levelInfo = robot.GetComponent<MultiplayerLevelInfo>();
            if(levelInfo.SetNotReady()) break;
        }  
    }

    public void StartEarly()
    {
        if(notReady.Count == 0){
            StartCountDown();
            startEarlyButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject robot in robots)
        {
            levelInfo = robot.GetComponent<MultiplayerLevelInfo>();
            levelInfo.StartGame();
        }
        StartProgrammingPhase();
        
    }

    public void StartCountDown() {
        hasStarted = true;
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject robot in robots)
        {
            levelInfo = robot.GetComponent<MultiplayerLevelInfo>();
            levelInfo.StartCountDown();
        }
    }

    public void TimerSet(float time){
        timer = time;
        finnishedButton.SetActive(false);
    }

    private void StartProgrammingPhase()
    {
        timer = programmingTime;
        programmingInterface.SetActive(true);
        finnishedButton.SetActive(true);
        programmingInterface.GetComponent<ProgramMuiltiplayerRobot>().textInstructions.text = "";
        programmingInterface.GetComponent<ProgramMuiltiplayerRobot>().stopProgram();
        countdownText.enabled = true;
        isProgramming = true;
    }

    private void StartExecutionPhase()
    {
        timer = executionTime;
        programmingInterface.GetComponent<ProgramMuiltiplayerRobot>().sendProgramToRobot();
        programmingInterface.SetActive(false);
        countdownText.enabled = true;
        isExecuting = true;
    }

    public void FinishedProgramming(){
        programmingInterface.SetActive(false);
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject robot in robots)
        {
            levelInfo = robot.GetComponent<MultiplayerLevelInfo>();
            levelInfo.SetTimer(finishedProgrammingTime);
        }
    }


    void Start()
    {
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();

    }

    // Update is called once per frame
    void Update()
    {
        if(isCountdowning)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("0");
            if (timer <= 0.0f) {
                isCountdowning = false;
                readyButton.SetActive(false);
                countdownText.enabled = false;
                StartGame();
                finnishedButton.SetActive(true);
            }
        }

        if(isProgramming)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("0");
            if (timer <= 0.0f) {
                isProgramming = false;
                readyButton.SetActive(false);
                countdownText.enabled = false;
                StartExecutionPhase();
            }
        }

        if(isExecuting)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("0");
            if (timer <= 0.0f) {
                isExecuting = false;
                readyButton.SetActive(false);
                countdownText.enabled = false;
                StartProgrammingPhase();
            }
        }
    }
            
}
