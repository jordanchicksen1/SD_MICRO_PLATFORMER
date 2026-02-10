using UnityEngine;
using UnityEngine.Video;

public class LevelSelectTrigger : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] string sceneName;
    [SerializeField] string levelId;               // NEW (must match gem pickups)
    [SerializeField] int totalGemsInLevel = 3;     // NEW
    [SerializeField] string levelTitle;

    [TextArea(2, 4)]
    [SerializeField] string levelDescription;

    [SerializeField] VideoClip previewClip;
    [SerializeField] Transform cameraFocusPoint;

    [Header("UI Manager")]
    [SerializeField] HubLevelSelectUI ui;

    void Awake()
    {
        if (!ui)
            ui = FindFirstObjectByType<HubLevelSelectUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ui == null) return;
        if (ui.IsOpen) return;

        // Get the player root even if the trigger hits a child collider
        HubPlayerController3D player = other.GetComponentInParent<HubPlayerController3D>();
        if (player == null) return;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        ui.Open(
            sceneName,
            previewClip,
            levelTitle,
            levelDescription,
            cameraFocusPoint,
            player,
            playerRb,
            levelId,            
            totalGemsInLevel    
        );
    }
}
