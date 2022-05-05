using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthPoints : NetworkBehaviour
{
    public bool changeColorLocal = false;
    NetworkVariable<int> healthPoints = new NetworkVariable<int>();
    NetworkVariable<bool> changeColor = new NetworkVariable<bool>();
    public int localHealth = 100;
    RobotList robotList;
    ParticleSystem smoke;

    Slider healthSlider;

    void Start()
    {
        if(IsOwner)
        healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
    }

    void Update() {
        //if (localHealth > 0)
        //getHit(25);
        //healthSlider.value -= (healthSlider.value - (float)localHealth) * Time.deltaTime * 2;
        healthSlider.value = (float)localHealth;

    }


    public void getHit(int damage) { 
        
        if(IsOwner) {
            if((localHealth - damage) > 0){
                localHealth = localHealth - damage;
            }
            else{
                localHealth = 0;
                Debug.Log("Killed");
                killed();
            }
            UpdateHealthInfoServerRpc(localHealth);
        }
        else {
            localHealth = healthPoints.Value;
        }
    }


    public void healPowerUp(int heal){
        if((healthPoints.Value + heal) < 100){
            healthPoints.Value = healthPoints.Value + heal;
        }
        else{
            healthPoints.Value = 100;
        }
    }

    public void killed() {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            if (c.GetType() != typeof(HealthPoints)) 
            {
                c.enabled = false;
            }
        }

        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log("1");
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
            return;
        Debug.Log("2");
        if(!networkClient.PlayerObject.TryGetComponent<Dead>(out var dead))
            return;
        Debug.Log("3");
        dead.SetDeadServerRpc(true);
        Debug.Log("4");


    }

    [ServerRpc]
    public void UpdateHealthInfoServerRpc(int health)
    {
        healthPoints.Value = health;
    }


}
