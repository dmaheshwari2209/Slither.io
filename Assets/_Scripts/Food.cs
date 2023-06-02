using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;

        if (!NetworkManager.Singleton.IsServer) return;

        if (collider.TryGetComponent(out PlayerLength playerLength))
        {
            playerLength.AddLength();
        }
        else if (collider.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner.GetComponent<PlayerLength>().AddLength();
        }
        
        NetworkObject.Despawn();
    }
}