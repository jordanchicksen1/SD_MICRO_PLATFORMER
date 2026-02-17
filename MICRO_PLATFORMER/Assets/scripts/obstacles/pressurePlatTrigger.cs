using UnityEngine;

public class PressurePlatformTrigger : MonoBehaviour
{
    PressurePlatform platform;

    void Awake()
    {
        platform = GetComponentInParent<PressurePlatform>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            platform.PlayerEnter();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            platform.PlayerExit();
    }
}
