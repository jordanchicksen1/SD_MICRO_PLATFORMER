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
    [SerializeField] HubPipeTravelUI travelUI;

    [Header("Transport")]
    [SerializeField] HubIslandPipe destinationPipe;
    [SerializeField] int destinationIslandID;

    bool ignoreTrigger;

    void Awake()
    {
        if (ui == null)
            ui = FindFirstObjectByType<HubProgressionGateUI>();

        if (travelUI == null)
            travelUI = FindFirstObjectByType<HubPipeTravelUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ignoreTrigger)
            return;

        HubPlayerController3D player =
            other.GetComponentInParent<HubPlayerController3D>();

        if (player == null)
            return;

        // If this destination is already unlocked,
        // use normal fast travel.
        if (IsUnlocked())
        {
            if (travelUI != null)
            {
                travelUI.Open(
                    destinationPipe,
                    player
                );
            }

            return;
        }

        // Destination is still locked.
        // Use the normal progression UI.
        if (ui == null)
            return;

        if (ui.IsOpen)
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

        return HubProgressionManager.Instance.IsIslandUnlocked(
            destinationIslandID
        );
    }

    void OpenTravelUI(
    HubPlayerController3D player
)
    {
        if (travelUI == null)
            return;

        if (destinationPipe == null)
            return;

        travelUI.Open(
            destinationPipe,
            player
        );
    }

    public void DisableTriggerTemporarily()
    {
        ignoreTrigger = true;
    }

    public void EnableTrigger()
    {
        ignoreTrigger = false;
    }

}