using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RobotFlags : NetworkBehaviour
{
    MultiplayerWorldParse worldScript;
    RobotCollision collisionScript;
    RobotRoundsHandler roundsScript;
    Dead deadScript;

    public Slider flagSlider;

    NetworkVariable<int> flagCount = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();
        collisionScript = GetComponent<RobotCollision>();
        roundsScript = GetComponent<RobotRoundsHandler>();
        deadScript = GetComponent<Dead>();

        flagCount.OnValueChanged += FlagCountChange;
    }

    private void FlagCountChange(int oldInt, int newInt)
    {
        flagSlider.value = newInt;
    }

    public bool CaptureFlag()
    {
        if (collisionScript.onFlagTile && !deadScript.IsDead())
        {
            IncreaseFlagCountServerRpc();
            MoveFlagServerRPC();

            if (flagCount.Value == flagSlider.maxValue)
            {
                roundsScript.SetGameStateForAllServerRpc(GameState.GameOver);
                return true;
            }


        }

        return false;
    }

    public int GetFlags()
    {
        return flagCount.Value;
    }

    public bool HasWon()
    {
        return flagCount.Value == flagSlider.maxValue;

    }

    public void LoseFlag()
    {
        DecreaseFlagCountServerRpc();
    }

    [ServerRpc]
    private void IncreaseFlagCountServerRpc()
    {
        if (flagCount.Value < flagSlider.maxValue)
            flagCount.Value++;

    }

    [ServerRpc]
    private void DecreaseFlagCountServerRpc()
    {
        if (flagCount.Value > 0)
            flagCount.Value--;
    }

    [ServerRpc]
    private void MoveFlagServerRPC()
    {
        worldScript.RandomFlagPosition();
        Vector3 newFlagPos = worldScript.GetFlagPosition();
        SetFlagPositionClientRpc(newFlagPos);
    }

    [ClientRpc]
    private void SetFlagPositionClientRpc(Vector3 newFlagPos)
    {
        worldScript.SetFlagPosition(newFlagPos);
    }


    private KeyCode[] sequence = new KeyCode[]{
    KeyCode.W,
    KeyCode.I,
    KeyCode.N};
    private int sequenceIndex;

    private void Win()
    {
        if (Input.GetKeyDown(sequence[sequenceIndex]))
        {
            if (++sequenceIndex == sequence.Length)
            {
                sequenceIndex = 0;
                IncreaseFlagCountServerRpc();
                MoveFlagServerRPC();

                if (flagCount.Value == flagSlider.maxValue)
                {
                    roundsScript.SetGameStateForAllServerRpc(GameState.GameOver);
                }
            }
        }
        else if (Input.anyKeyDown) sequenceIndex = 0;
    }
    private KeyCode[] sequence2 = new KeyCode[]{
    KeyCode.H,
    KeyCode.A,
    KeyCode.C,
    KeyCode.K};
    private int sequenceIndex2;


    private void Hack()
    {
        if (Input.GetKeyDown(sequence2[sequenceIndex2]))
        {
            if (++sequenceIndex2 == sequence2.Length)
            {
                sequenceIndex2 = 0;
                MoveFlagServerRPC();
            }
        }
        else if (Input.anyKeyDown) sequenceIndex2 = 0;
    }

    private void Update()
    {
        Win();
        Hack();
    }

}
