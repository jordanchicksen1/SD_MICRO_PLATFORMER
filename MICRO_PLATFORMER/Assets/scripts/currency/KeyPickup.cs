using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
        if (!player) return;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (!inv) return;

        // already has a key? optional: ignore
        if (inv.HasKey) return;

        // give key
        inv.GiveKey();
     

        // show key icon on the correct player's UI
        PlayerHealthUIManager ui = FindFirstObjectByType<PlayerHealthUIManager>();
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ui != null && ph != null)
            ui.SetPlayerHasKey(ph, true);

        // remove key from world
        Destroy(gameObject);
    }
}
