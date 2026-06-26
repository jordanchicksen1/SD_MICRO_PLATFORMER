using UnityEngine;

public class HatStandTrigger : MonoBehaviour
{
    [SerializeField] Transform cameraFocusPoint;
    [SerializeField] HatStandUI ui;

    void Awake()
    {
        if (!ui)
            ui = FindFirstObjectByType<HatStandUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ui == null)
            return;

        if (ui.IsOpen)
            return;

        HubPlayerController3D player =
            other.GetComponentInParent<HubPlayerController3D>();

        if (!player)
            return;

        Rigidbody rb =
            player.GetComponent<Rigidbody>();

        ui.Open(
            cameraFocusPoint,
            player,
            rb);
    }
}