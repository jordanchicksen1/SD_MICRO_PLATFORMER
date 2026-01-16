using UnityEngine;

public class RotatorButton : MonoBehaviour
{
    [SerializeField] RotatePlatform90 targetPlatform;

    bool pressed;

    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;

        // Must be player
        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player) return;

        // Optional: only count as a “jump on it” if player is falling down
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.y > 0.1f)
            return;

        pressed = true;

        if (targetPlatform != null)
            targetPlatform.RotateNow();
    }
}
