using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Dead : NetworkBehaviour 
{
    RobotList robotList;
    ChickenDinner chickenDinner;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private Renderer colorBase;
    [SerializeField] private Renderer colorTower;

    private NetworkVariable<bool> dead = new NetworkVariable<bool>();

    [ServerRpc]
    public void SetDeadServerRpc(bool isDead) {
        dead.Value = isDead;
    }

    public bool IsDead() {
        return dead.Value;
    }
    
    private void OnEnable() {
        dead.OnValueChanged += ChangeColor;

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

        ChickenDinner chickenDinner = GameObject.Find("ChickenDinner").GetComponent<ChickenDinner>();
        chickenDinner.RobotDeath();
    }

}