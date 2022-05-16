using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Dead : NetworkBehaviour 
{
    RobotList robotList;
    ChickenDinner chickenDinner;
    Spectator spectatorScript;
    GameObject spectator;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private Renderer colorBase;
    [SerializeField] private Renderer colorTower;

    private NetworkVariable<bool> dead = new NetworkVariable<bool>();

    public override void OnNetworkSpawn() {
        spectator = GameObject.Find("Spectator");
        
        //spectator.SetActive(false);
    }

    private IEnumerator Wait(int seconds) {
        ChickenDinner chickenDinner = GameObject.Find("ChickenDinner").GetComponent<ChickenDinner>();
        chickenDinner.robotDeath();

        yield return new WaitForSeconds(seconds);

        chickenDinner.gameObject.SetActive(false);

        spectator.SetActive(true);
        spectatorScript = spectator.GetComponent<Spectator>();
        spectatorScript.SetSpectateOnDeath();
    }

    [ServerRpc]
    public void SetDeadServerRpc(bool isDead) {
        Debug.Log("UPDATE DEATH");
        dead.Value = isDead;
    }

    public bool IsDead() {
        return dead.Value;
    }
    
    private void OnEnable() {
        dead.OnValueChanged +=ChangeColor;

    }
    private void onDisable() {
        dead.OnValueChanged -= ChangeColor;
    }

    private void ChangeColor(bool oldBool, bool newBool) {
        if (!IsClient) return;


        colorBase.material.SetColor("_Color", Color.black);
        colorTower.material.SetColor("_Color", Color.black);
        var em = smoke.emission;
        em.enabled = true;
        

        StartCoroutine(Wait(5));

        
    }



}