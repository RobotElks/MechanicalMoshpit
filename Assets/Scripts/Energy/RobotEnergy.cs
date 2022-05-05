using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotEnergy : NetworkBehaviour
{
    public NetworkVariable<float> energyPoints = new NetworkVariable<float>();

    ProgramMuiltiplayerRobot programRobotScript;

    void Start(){
        energyPoints.Value = 100f;
        GameObject programRobot = GameObject.Find("ProgrammingInterface Multiplayer Variant");
        programRobotScript = programRobot.GetComponent<ProgramMuiltiplayerRobot>();
        if(IsOwner){
            programRobotScript.SetRobot(gameObject);
        }
    }

    public void useEnergy(float energy){
        useEnergyServerRpc(energy);
    }

    [ServerRpc]
    public void useEnergyServerRpc(float energy){
        if(energyPoints.Value - energy > 0f){
            energyPoints.Value = energyPoints.Value - energy;
        }
        else{
            energyPoints.Value = 0f;
        }
        Debug.Log(energyPoints.Value);
    }

    public void restoreEnergy(float energy){
        restoreEnergyServerRpc(energy);
        Debug.Log(energyPoints.Value);
    }

    [ServerRpc]
    public void restoreEnergyServerRpc(float energy){
        if(energyPoints.Value + energy < 100f){
            energyPoints.Value = energyPoints.Value + energy;
        }
        else{
            energyPoints.Value = 100f;
        }
    }

}
