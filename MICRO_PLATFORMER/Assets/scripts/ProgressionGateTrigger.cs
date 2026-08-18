using UnityEngine;

public class ProgressionGateTrigger : MonoBehaviour
{
    [Header("Gate Data")]
    [SerializeField] string gateTitle;

    [TextArea(2, 4)]
    [SerializeField] string gateDescription;

    [SerializeField] int gemCost = 1;

    [Header("Camera")]
    [SerializeField] Transform cameraFocusPoint;

    [Header("UI")]
    [SerializeField] HubProgressionGateUI ui;

    void Awake()
    {
        if (ui == null)
            ui = FindFirstObjectByType<HubProgressionGateUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ui == null)
            return;

        if (ui.IsOpen)
            return;

        HubPlayerController3D player =
            other.GetComponentInParent<HubPlayerController3D>();

        if (player == null)
            return;

        ui.Open(
            gateTitle,
            gateDescription,
            gemCost,
            cameraFocusPoint,
            player
        );
    }
}