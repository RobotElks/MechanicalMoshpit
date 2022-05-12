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
    ChickenDinner chickenDinner;
    public int heal = 50;
    public int damageTilePower = 20;
    Slider healthSlider;

    GameRoundsManager roundsManager;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            healthSlider = GameObject.Find("Hud").transform.Find("HealthBar").GetComponent<Slider>();

        roundsManager = GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>();
        chickenDinner = GameObject.Find("ChickenDinner").GetComponent<ChickenDinner>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
            getHit(25);
        //healthSlider.value -= (healthSlider.value - (float)localHealth) * Time.deltaTime * 2;

        if (IsOwner)
            healthSlider.value = (float)localHealth;
        if(gameObject.transform.position.y < -50) getHit(100);

    }


    public void getHit(int damage)
    {   
        
            if (IsOwner)
            {
                Debug.Log(NetworkManager.Singleton.LocalClientId);
                if ((localHealth - damage) > 0)
                {
                    localHealth = localHealth - damage;
                }
                else
                {
                    localHealth = 0;
                    killed();
                }
                UpdateHealthInfoServerRpc(localHealth);
            }
            //else
            //{
            //    localHealth = healthPoints.Value;
            //}
        
    }

    public void DamageTile(){
        getHit(damageTilePower);
    }

    public void healPowerUp()
    {
        if (IsOwner)
        {
            if ((healthPoints.Value + heal) < 100)
            {
                healthPoints.Value = healthPoints.Value + heal;
            }
            else
            {
                healthPoints.Value = 100;
            }
            localHealth = healthPoints.Value;
            UpdateHealthInfoServerRpc(localHealth);
        }
    }

    public void killed()
    {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            if (c.GetType() != typeof(HealthPoints))
            {
                c.enabled = false;
            }
        }

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (!IsHost)
        {
            if (!NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Dead>(out var dead))
                return;
            dead.SetDeadServerRpc(true);
        }
        else
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
                return;
            if (!networkClient.PlayerObject.TryGetComponent<Dead>(out var dead))
                return;
            dead.SetDeadServerRpc(true);
        }

    }

    [ServerRpc]
    public void UpdateHealthInfoServerRpc(int health)
    {
        healthPoints.Value = health;
    }


}
