using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Launch")]
    [SerializeField] float launchVelocity = 12f;   // tweak this for height
    [SerializeField] float upwardExtraForce = 0f;  // optional extra “pop”
    [SerializeField] float cooldown = 0.1f;        // prevents double-triggers

    float lastLaunchTime;

    void OnTriggerEnter(Collider other)
    {
        // Only affect players
        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (!rb) return;

        // Only bounce if player is moving downward / landing
        if (rb.linearVelocity.y > 0.1f) return;

        if (Time.time - lastLaunchTime < cooldown) return;
        lastLaunchTime = Time.time;

        // Reset vertical speed so bounce is consistent
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        // Launch upward
        rb.AddForce(Vector3.up * launchVelocity, ForceMode.VelocityChange);

        // Optional: extra pop (Impulse)
        if (upwardExtraForce > 0f)
            rb.AddForce(Vector3.up * upwardExtraForce, ForceMode.Impulse);
    }
}
