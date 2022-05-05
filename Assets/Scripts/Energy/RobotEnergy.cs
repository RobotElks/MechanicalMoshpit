using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RobotEnergy : NetworkBehaviour
{
    public NetworkVariable<float> networkEnergyPoints = new NetworkVariable<float>();

    ProgramMuiltiplayerRobot programRobotScript;

    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            EnergyStartServerRPC();
            GameObject programRobot = GameObject.Find("ProgrammingInterface Multiplayer Variant");
            programRobotScript = programRobot.GetComponent<ProgramMuiltiplayerRobot>();
            programRobotScript.SetRobot(gameObject);
        }
    }

    [ServerRpc]
    public void EnergyStartServerRPC()
    {
        networkEnergyPoints.Value = 100f;

    }

    public void useEnergy(float energy){
        useEnergyServerRpc(energy);
    }

    [ServerRpc]
    public void useEnergyServerRpc(float energy){
        if(networkEnergyPoints.Value - energy > 0f){
            networkEnergyPoints.Value = networkEnergyPoints.Value - energy;
        }
        else{
            networkEnergyPoints.Value = 0f;
        }
    }

    public void restoreEnergy(float energy){
        restoreEnergyServerRpc(energy);
    }

    [ServerRpc]
    public void restoreEnergyServerRpc(float energy){
        if(networkEnergyPoints.Value + energy < 100f){
            networkEnergyPoints.Value = networkEnergyPoints.Value + energy;
        }
        else{
            networkEnergyPoints.Value = 100f;
        }
    }

}
