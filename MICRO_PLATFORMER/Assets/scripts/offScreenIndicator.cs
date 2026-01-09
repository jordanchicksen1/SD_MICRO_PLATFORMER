using UnityEngine;
using UnityEngine.UI;

public class offScreenIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform trackedPlayer;   // the OTHER player
    [SerializeField] Camera cam;

    [Header("Settings")]
    [SerializeField] float screenEdgeBuffer = 40f;

    RectTransform rectTransform;
    RawImage image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<RawImage>();
    }

    void Update()
    {
        if (trackedPlayer == null || cam == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(trackedPlayer.position);

        bool offScreen =
            screenPos.z < 0 ||
            screenPos.x < 0 ||
            screenPos.x > Screen.width ||
            screenPos.y < 0 ||
            screenPos.y > Screen.height;

        image.enabled = offScreen;

        if (!offScreen)
            return;

        // Clamp to screen edge
        screenPos.x = Mathf.Clamp(
            screenPos.x,
            screenEdgeBuffer,
            Screen.width - screenEdgeBuffer
        );

        screenPos.y = Mathf.Clamp(
            screenPos.y,
            screenEdgeBuffer,
            Screen.height - screenEdgeBuffer
        );

        rectTransform.position = screenPos;

        // Rotate arrow toward player
        Vector3 dir = (trackedPlayer.position - cam.transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);
    }

    public void SetTrackedPlayer(Transform player)
    {
        trackedPlayer = player;
    }
}
