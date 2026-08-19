using UnityEngine;

public class HubSkyColorTransition : MonoBehaviour
{
    [SerializeField]
    Color destinationSkyColor = new Color32(28, 141, 197, 255);

    Camera hubCamera;

    void Awake()
    {
        hubCamera = Camera.main;
    }

    public void SetDestinationSkyColor()
    {
        if (hubCamera == null)
            return;

        hubCamera.backgroundColor =
            destinationSkyColor;
    }

}