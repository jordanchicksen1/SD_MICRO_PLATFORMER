using UnityEngine;

public class HatShopTrigger : MonoBehaviour
{
    [SerializeField] Transform cameraFocusPoint;

    [Header("UI")]
    [SerializeField] HatShopUI ui;

    void Awake()
    {
        if (!ui)
            ui = FindFirstObjectByType<HatShopUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ui == null) return;
        if (ui.IsOpen) return;

        HubPlayerController3D player =
            other.GetComponentInParent<HubPlayerController3D>();

        if (player == null) return;

        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        ui.Open(
            cameraFocusPoint,
            player,
            playerRb
        );
    }
}