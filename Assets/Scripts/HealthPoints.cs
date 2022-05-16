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
    //NetworkVariable<float> abovePlayerHealth = new NetworkVariable<float>();
    public int localHealth = 100;
    RobotList robotList;
    ParticleSystem smoke;
    ChickenDinner chickenDinner;
    public int heal = 50;
    public Slider healthSlider;
    public Slider abovePlayerHealth;
    public int damageTilePower = 20;

    GameRoundsManager roundsManager;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthSlider = GameObject.Find("Hud").transform.Find("HealthBar").GetComponent<Slider>();
        }

        abovePlayerHealth = this.GetComponentInChildren<Slider>();

        //roundsManager = GameObject.Find("GameRoundsManager").GetComponent<GameRoundsManager>();
        chickenDinner = GameObject.Find("ChickenDinner").GetComponent<ChickenDinner>();
    }

    void Update()
    {

        abovePlayerHealth.value = (float)localHealth;
        if (Input.GetKeyDown("space"))
            getHit(25);
        //healthSlider.value -= (healthSlider.value - (float)localHealth) * Time.deltaTime * 2;
        /*
                if (IsOwner)
                {
                    healthSlider.value = (float)localHealth;

                    abovePlayerHealth.value = (float)localHealth;
                }
        */
        if (gameObject.transform.position.y < -50) getHit(100);

    }


    public void getHit(int damage)
    {

        if (IsOwner)
        {
            if ((localHealth - damage) > 0)
            {
                localHealth = localHealth - damage;
                healthSlider.value = (float)localHealth;
                abovePlayerHealth.value = (float)localHealth;
            }
            else
            {
                localHealth = 0;
                healthSlider.value = 0f;
                abovePlayerHealth.value = 0f;
                killed();
            }
            UpdateHealthInfoServerRpc(localHealth);

        }
        else
        {
            localHealth = healthPoints.Value;
            Debug.Log("Healthpoints: " + healthPoints.Value);
            healthSlider.value = (float)healthPoints.Value;
            abovePlayerHealth.value = (float)healthPoints.Value;
        }


    }

    public void DamageTile()
    {
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
        healthSlider.value = (float)health;
        abovePlayerHealth.value = (float)health;
    }

}
