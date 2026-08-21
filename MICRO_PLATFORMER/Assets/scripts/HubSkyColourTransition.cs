using UnityEngine;

public class HubSkyColorTransition : MonoBehaviour
{
    [Header("Island Sky Colours")]

    [SerializeField]
    Color island0SkyColor =
        new Color32(0, 76, 195, 255);

    [SerializeField]
    Color island1SkyColor =
        new Color32(28, 141, 197, 255);

    Camera hubCamera;

    void Awake()
    {
        hubCamera = Camera.main;
    }

    public void SetDestinationSkyColor(
        int destinationIslandID
    )
    {
        if (hubCamera == null)
            return;

        if (destinationIslandID == 0)
        {
            hubCamera.backgroundColor =
                island0SkyColor;
        }
        else if (destinationIslandID == 1)
        {
            hubCamera.backgroundColor =
                island1SkyColor;
        }
    }

    public void SetCurrentIslandSkyColor(
        int currentIslandID
    )
    {
        if (hubCamera == null)
            return;

        if (currentIslandID == 0)
        {
            hubCamera.backgroundColor =
                island0SkyColor;
        }
        else if (currentIslandID == 1)
        {
            hubCamera.backgroundColor =
                island1SkyColor;
        }
    }
}