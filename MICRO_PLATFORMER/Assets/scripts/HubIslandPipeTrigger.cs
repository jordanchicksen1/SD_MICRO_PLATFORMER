using UnityEngine;

public class HubIslandPipeTrigger : MonoBehaviour
{
    [SerializeField] HubIslandPipe pipe;

    bool playerInside;

    void OnTriggerEnter(Collider other)
    {
        if (playerInside)
            return;

        HubPlayerController3D player =
            other.GetComponent<HubPlayerController3D>();

        if (player == null)
            return;

        if (pipe == null)
            return;

        playerInside = true;

        HubPipeTravelUI ui =
            FindFirstObjectByType<HubPipeTravelUI>();

        if (ui == null)
            return;

        ui.Open(pipe, player);
    }

    void OnTriggerExit(Collider other)
    {
        HubPlayerController3D player =
            other.GetComponent<HubPlayerController3D>();

        if (player != null)
            playerInside = false;
    }
}