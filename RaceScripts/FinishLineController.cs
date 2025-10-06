using Unity.Netcode;
using UnityEngine;

public class FinishLineController : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            RaceManager.Instance.PlayerFinishedRace(playerController.OwnerClientId);
        }
    }
}
