using UnityEngine;
using System.Collections.Generic;

public class FallSaver : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;
    public Transform ballPoint;

    [Header("Damage")]
    public int damage = 1;

    [Header("Safety")]
    [SerializeField] float reentryCooldown = 0.25f;

    readonly Dictionary<int, float> nextAllowedTime = new();

    void OnTriggerEnter(Collider other) => Handle(other);
    void OnTriggerStay(Collider other) => Handle(other);

    void Handle(Collider other)
    {
        Transform root = other.transform.root;

        // =========================
        // PLAYER HANDLING
        // =========================
        if (root.CompareTag("Player"))
        {
            var bubble = root.GetComponent<PlayerBubbleState>();
            var health = root.GetComponent<PlayerHealth>();
            if (health == null) return;

            // Ignore bubbled players
            if (bubble != null && bubble.IsBubbled)
                return;

            int id = root.gameObject.GetInstanceID();
            if (nextAllowedTime.TryGetValue(id, out float t) && Time.time < t)
                return;

            nextAllowedTime[id] = Time.time + reentryCooldown;

            // Lethal
            if (health.CurrentHealth - damage <= 0)
            {
                health.TakeDamage(damage, transform.position);
                return;
            }

            // Non-lethal
            health.TakeDamage(damage, transform.position);

            if (respawnPoint != null)
            {
                var rb = root.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                root.position = respawnPoint.position;
            }

            return; // IMPORTANT: stop here
        }

        // =========================
        // BALL HANDLING
        // =========================
        if (other.CompareTag("Ball"))
        {
            if (ballPoint != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                other.transform.position = ballPoint.position;
            }
        }
    }

}
