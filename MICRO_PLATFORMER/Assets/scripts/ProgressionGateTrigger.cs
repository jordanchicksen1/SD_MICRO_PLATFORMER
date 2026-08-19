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

    [Header("Transport")]
    [SerializeField] HubIslandPipe destinationPipe;

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

        if (IsUnlocked())
            return;

        ui.Open(
            gateTitle,
            gateDescription,
            gemCost,
            cameraFocusPoint,
            player
        );
    }

    public bool IsUnlocked()
    {
        if (HubProgressionManager.Instance == null)
        {
            Debug.LogWarning(
                "[ProgressionGateTrigger] No HubProgressionManager found!"
            );

            return false;
        }

        Debug.Log(
            $"[ProgressionGateTrigger] Island2Unlocked = " +
            $"{HubProgressionManager.Instance.Island2Unlocked}"
        );

        return HubProgressionManager.Instance.Island2Unlocked;
    }
}