using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNamePlate : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerNetworkName = new NetworkVariable<FixedString32Bytes>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerNetworkName.Value = $"Player {OwnerClientId+1}";
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
