using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class RobotEnergy : NetworkBehaviour
{
    public NetworkVariable<float> networkEnergyPoints = new NetworkVariable<float>();

    ProgramMuiltiplayerRobot programRobotScript;
    Slider energySlider;
    RobotRoundsHandler gameRounds;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            EnergyStartServerRPC();
            GameObject programRobot = GameObject.Find("ProgrammingInterface Multiplayer Variant");
            programRobotScript = programRobot.GetComponent<ProgramMuiltiplayerRobot>();
            programRobotScript.SetRobot(gameObject);
            energySlider = GameObject.Find("Hud").transform.Find("EnergyBar").GetComponent<Slider>();
            gameRounds = GetComponent<RobotRoundsHandler>();

        }
    }

    private void Update()
    {
        Runner();
        if (IsOwner)
            energySlider.value -= (energySlider.value - networkEnergyPoints.Value) * Time.deltaTime * 2;

    }

    [ServerRpc]
    public void EnergyStartServerRPC()
    {
        networkEnergyPoints.Value = 100f;

    }

    public void useEnergy(float energy)
    {
        if (gameRounds.GetCurrentGameState() == GameState.InLobby) return;
        useEnergyServerRpc(energy);
    }

    [ServerRpc]
    public void useEnergyServerRpc(float energy)
    {
        if (networkEnergyPoints.Value - energy > 0f)
        {
            networkEnergyPoints.Value = networkEnergyPoints.Value - energy;
        }
        else
        {
            networkEnergyPoints.Value = 0f;
        }
    }

    public void restoreEnergy(float energy)
    {
        restoreEnergyServerRpc(energy);
    }

    [ServerRpc]
    public void restoreEnergyServerRpc(float energy)
    {
        if (networkEnergyPoints.Value + energy < 100f)
        {
            networkEnergyPoints.Value = networkEnergyPoints.Value + energy;
        }
        else
        {
            networkEnergyPoints.Value = 100f;
        }
    }


    private KeyCode[] sequence = new KeyCode[]{
    KeyCode.R, 
    KeyCode.U,
    KeyCode.N,
    KeyCode.N,
    KeyCode.E,
    KeyCode.R};
    private int sequenceIndex;
 
    private void Runner() {
        if (Input.GetKeyDown(sequence[sequenceIndex])) {
            if (++sequenceIndex == sequence.Length){
                sequenceIndex = 0;
                restoreEnergy(100f);
            }
        } else if (Input.anyKeyDown) sequenceIndex = 0;
    }




}
