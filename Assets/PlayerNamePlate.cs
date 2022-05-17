using TMPro;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;


public class PlayerNamePlate : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerNetworkName = new NetworkVariable<FixedString32Bytes>();

    private bool overlaySet = false;
    public SaveIP informationScript;

    public override void OnNetworkSpawn()
    {
        if(IsHost)
        {
            //SaveIP information = GameObject.Find("Information").GetComponent<SaveIP>();

        }

        if (IsServer)
        {
            //playerNetworkName.Value = $"Player {OwnerClientId+1}";
            playerNetworkName.Value = informationScript.PlayerName();
            Debug.Log("Sista" + playerNetworkName.Value);
        }
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playerNetworkName.Value.ToString();
    }

    private void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value.ToString()))
        {
            SetOverlay();
            overlaySet = true;
        }
    }
}
