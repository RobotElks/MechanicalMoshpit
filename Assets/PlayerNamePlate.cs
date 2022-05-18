using TMPro;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;


public class PlayerNamePlate : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerNetworkName = new NetworkVariable<FixedString32Bytes>();

    private bool overlaySet = false;
    //public SaveIP informationScript;
    private string localPlayerName;
    private SaveIP infoScript;

    public override void OnNetworkSpawn()
    {
        infoScript = GameObject.Find("Information").GetComponent<SaveIP>();

        if (IsOwner)
        {
            playerNetworkName.Value = $"Player {OwnerClientId+1}";
            if(infoScript.PlayerName() != "")
            {
                localPlayerName = infoScript.PlayerName();
                SetServerNameServerRpc(localPlayerName);
            }
        }
    }

    [ServerRpc]
    private void SetServerNameServerRpc(string name) {
        playerNetworkName.Value = name;
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
