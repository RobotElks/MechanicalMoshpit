using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerHealthBar : NetworkBehaviour
{
    //[SerializeField] RectTransform healthAmount;
    public Slider healthSlider;
    public Slider abovePlayerHealth;
    GameObject programmingInterface;

    // Network variables
    NetworkVariable<int> healthPoints = new NetworkVariable<int>(100);
    NetworkVariable<int> deaths = new NetworkVariable<int>(0);

    // Local variables
    public int localDeaths = 0;
    public int localHealth = 100;
    public int heal = 100;
    public bool changeColorLocal = false;
    public int damageTilePower = 10;


    //Local scripts
    RobotRoundsHandler roundsHandlerScript;
    RobotFlags flagScript;
    Dead deadScript;
    RobotMultiplayerMovement thisRobotMovementScript;
    RobotEnergy energScript;





    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthSlider = GameObject.Find("Hud").transform.Find("HealthBar").GetComponent<Slider>();
        }

        abovePlayerHealth = GetComponentInChildren<Slider>();
        roundsHandlerScript = GetComponentInParent<RobotRoundsHandler>();
        programmingInterface = GameObject.Find("ProgrammingInterface Multiplayer Variant");
        flagScript = GetComponentInParent<RobotFlags>();
        deadScript = GetComponentInParent<Dead>();
        thisRobotMovementScript = GetComponentInParent<RobotMultiplayerMovement>();
        energScript = GetComponentInParent<RobotEnergy>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
            GetHit(25);

        if (!IsOwner)
        {
            localHealth = healthPoints.Value;
            abovePlayerHealth.value = (float)localHealth;
            //healthSlider.value = (float)localHealth;
        }
        //if (IsOwner)
        //{
        //    healthSlider.value = (float)localHealth;
        //}

        //Die on fall
        if (gameObject.transform.position.y < -20) GetHit(100);
    }

    
    public void ReviveRobot()
    {
        localHealth = 100;
        UpdateHealthInfoServerRpc(localHealth);
        abovePlayerHealth.value = (float)localHealth;
        healthSlider.value = (float)localHealth;
        deadScript.SetDeadServerRpc(false);
        energScript.RestoreEnergyFull();

    }

    public int GetDeaths(){
        return deaths.Value;
    }
    public void GetHit(int damageAmount)
    {
        if (IsOwner && roundsHandlerScript.InsideActiveGame() && localHealth > 0)
        {
            if ((localHealth - damageAmount) > 0)
            {
                localHealth = localHealth - damageAmount;
                healthSlider.value = (float)localHealth;
                abovePlayerHealth.value = (float)localHealth;
            }
            else
            {
                localHealth = 0;
                abovePlayerHealth.value = (float)localHealth;
                healthSlider.value = 0f;
                killed();
            }
            UpdateHealthInfoServerRpc(localHealth);
        }
    }

    [ServerRpc]
    public void UpdateHealthInfoServerRpc(int health)
    {
        healthPoints.Value = health;
        abovePlayerHealth.value = (float)health;
        healthSlider.value = (float)health;
    }

    [ServerRpc]
    public void UpdateDeathsInfoServerRpc(int localDeaths)
    {
        deaths.Value = localDeaths;
    }

    public void HealPowerUp()
    {
        if (IsOwner)
        {
            if ((localHealth + heal) < 100)
            {
                localHealth = localHealth + heal;
            }
            else
            {
                localHealth = 100;
            }

            healthSlider.value = (float)localHealth;
            abovePlayerHealth.value = (float)localHealth;
            UpdateHealthInfoServerRpc(localHealth);
            
        }
    }

    public void killed()
    {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            if (c.GetType() == typeof(MultiplayerDetectTarget))
            {
                c.enabled = false;
            }
        }

        healthSlider.value = (float)localHealth;
        abovePlayerHealth.value = (float)localHealth;

        if (IsOwner)
        {
            thisRobotMovementScript.SetAnimation(StateOfAnimation.Death);
            localDeaths += 1;
            UpdateDeathsInfoServerRpc(localDeaths);
            GetComponentInParent<RobotMultiplayerInstructionScript>().StopExecute();
            programmingInterface.SetActive(false);
            flagScript.LoseFlag();
            //robotMovementScript.MoveToSpawnPoints(worldScript.GetSpawnPoint());
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

    public void DamageTile()
    {
        GetHit(damageTilePower);
    }
}
