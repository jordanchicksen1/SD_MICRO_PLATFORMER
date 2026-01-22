using System.Collections;
using UnityEngine;

public class KeyDoor : MonoBehaviour, IInteractable
{
    [SerializeField] Door door; // or whatever door script you use
    public GameObject lockGold;
    public GameObject lockSilver;
    public void Interact(PlayerController3D player)
    {
        if (!player) return;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        if (!inv.HasKey)
        {
            Debug.Log("Need a key!");
            return;
        }

        // consume key
        inv.UseKey();

        // hide key icon for this player
        PlayerHealthUIManager ui = FindFirstObjectByType<PlayerHealthUIManager>();
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ui != null && ph != null)
            ui.SetPlayerHasKey(ph, false);

        // open door
        if (door != null)
            door.Toggle(); // or Open()
        StartCoroutine(DestroyLock());
    }

    public IEnumerator DestroyLock()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(lockGold);
        Destroy(lockSilver);
    }
}
