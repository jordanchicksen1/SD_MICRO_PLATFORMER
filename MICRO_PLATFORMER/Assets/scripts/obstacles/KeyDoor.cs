using System.Collections;
using UnityEngine;

public class KeyDoor : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] Door door;

    [Header("Locks")]
    public GameObject lockGold;
    public GameObject lockSilver;

    [Header("Camera Focus (first open only)")]
    [SerializeField] DoorCameraFocus cameraFocus; // put DoorCameraFocus on your CoopCamera rig
    [SerializeField] Transform focusPoint;        // assign DoorFocusPoint_Key

    public void Interact(PlayerController3D player)
    {
        if (!player) return;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        if (!inv.HasKey)
        {
            player.ShowNeedKeyPrompt();
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

        // Open the door (FIRST OPEN ONLY triggers focus)
        if (door != null)
        {
            bool firstTimeOpened = door.Open(); // <-- use Open, not Toggle
            if (firstTimeOpened)
            {
                if (!cameraFocus) cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
                if (cameraFocus && focusPoint)
                    cameraFocus.FocusOn(focusPoint);
            }
        }

        StartCoroutine(DestroyLock());
    }

    public IEnumerator DestroyLock()
    {
        yield return new WaitForSeconds(0.5f);
        if (lockGold) Destroy(lockGold);
        if (lockSilver) Destroy(lockSilver);
    }
}
