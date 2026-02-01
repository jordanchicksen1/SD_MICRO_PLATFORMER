using UnityEngine;
using System.Collections.Generic;

public class FallSaver : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Damage")]
    public int damage = 1;

    [Header("Safety")]
    [SerializeField] float reentryCooldown = 0.25f;

    readonly Dictionary<int, float> nextAllowedTime = new();

    void OnTriggerEnter(Collider other) => Handle(other);
    void OnTriggerStay(Collider other) => Handle(other); // NEW

    void Handle(Collider other)
    {
        if (!other.transform.root.CompareTag("Player")) return;

        Transform playerRoot = other.transform.root;

        var bubble = playerRoot.GetComponent<PlayerBubbleState>();
        var health = playerRoot.GetComponent<PlayerHealth>();
        if (health == null) return;

        // Ignore bubbled players entirely
        if (bubble != null && bubble.IsBubbled)
            return;

        int id = playerRoot.gameObject.GetInstanceID();
        if (nextAllowedTime.TryGetValue(id, out float t) && Time.time < t)
            return;

        nextAllowedTime[id] = Time.time + reentryCooldown;

        // Lethal: deal damage, do NOT teleport
        if (health.CurrentHealth - damage <= 0)
        {
            health.TakeDamage(damage, transform.position);
            return;
        }

        // Non-lethal: damage + teleport
        health.TakeDamage(damage, transform.position);

        if (respawnPoint != null)
        {
            var rb = playerRoot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerRoot.position = respawnPoint.position;
        }
    }
}
