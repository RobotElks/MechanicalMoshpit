using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.UI;

public class MultiplayerDetectTarget : NetworkBehaviour
{
    //public float range;
    RobotList robotList;
    CannonBehavior cannonScript;
    Dead deadScript;
    NetworkVariable<int> shotsFired = new NetworkVariable<int>(0);
    RobotRoundsHandler roundsHandlerScript;
    RaycastHit hit;
    Slider reloadSlider;

    //RobotMovement MovementScript;
    //private bool reload = true;
    //public float fireRate = 0.5f;
    //private float nextFire = 0.0f;
    public int localShotsFired;
    private float nextShotTime = 0.0f;
    private float reloadTime = 5f;
    private float detectionDistance = 30f;
    public LayerMask maskRobots;

    // Start is called before the first frame update
    void Start()
    {
        cannonScript = this.GetComponent<CannonBehavior>();
        //MovementScript = this.GetComponent<RobotMovement>();

        //CheckIfTargetInScope();
    }

    public override void OnNetworkSpawn()
    {
        robotList = GameObject.Find("RobotList").GetComponent<RobotList>();
        robotList.AddRobot(this.gameObject);

        deadScript = GetComponent<Dead>();
        roundsHandlerScript = GetComponent<RobotRoundsHandler>();

        if (IsOwner)
            reloadSlider = GameObject.Find("Hud").transform.Find("ReloadBar").GetComponent<Slider>();

    }

    public override void OnNetworkDespawn()
    {
        robotList.RemoveRobot(this.gameObject);
    }



    private bool CheckIfTargetInScope()
    {
        Vector3 startPoint = transform.position + new Vector3(0, 0.5f, 0);
        //Debug.DrawRay(startPoint, transform.forward, Color.red, 0.3f);
        if (Physics.Raycast(startPoint, transform.forward, out hit, detectionDistance))
        {
            if (hit.collider.tag == "Player")
                return !hit.collider.gameObject.GetComponent<Dead>().IsDead();
        }
        return false;
    }

    [ServerRpc]
    public void UpdateShotsFiredInfoServerRpc(int localShots)
    {
        shotsFired.Value = localShots;
    }

    private void ShootTarget()
    {
        localShotsFired += 1;
        nextShotTime = Time.time + reloadTime;
        UpdateShotsFiredInfoServerRpc(localShotsFired);
        cannonScript.Shoot();
        reloadSlider.gameObject.SetActive(true);
    }

    public int GetShotsFired()
    {
        return shotsFired.Value;
    }

    // Update is called once per frame
    void Update()
    {
        Fire();

        GameState state = roundsHandlerScript.GetCurrentGameState();
        if (IsOwner)
        {
            reloadSlider.value = (nextShotTime - Time.time) / reloadTime;
            if (Time.time > nextShotTime) reloadSlider.gameObject.SetActive(false);

            if ((Time.time > nextShotTime) && CheckIfTargetInScope() && state != GameState.Programming && state != GameState.Countdown)
            {
                // CALL SERVER TO SHOOT
                nextShotTime = Time.time + reloadTime;
                ShootTarget();
            }
        }

    }

    private KeyCode[] sequence = new KeyCode[]{
    KeyCode.F,
    KeyCode.I,
    KeyCode.R,
    KeyCode.E};
    private int sequenceIndex;

    private void Fire()
    {
        if (Input.GetKeyDown(sequence[sequenceIndex]))
        {
            if (++sequenceIndex == sequence.Length)
            {
                sequenceIndex = 0;
                ShootTarget();
            }
        }
        else if (Input.anyKeyDown) sequenceIndex = 0;
    }


}
