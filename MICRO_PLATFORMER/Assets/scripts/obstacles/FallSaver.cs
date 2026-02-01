using UnityEngine;
using System.Collections.Generic;

public class FallSaver : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Damage")]
    public int damage = 1;

    [Header("Safety")]
    [SerializeField] float reentryCooldown = 0.25f; // prevents rapid re-trigger after teleport

    // per player cooldown by instance id
    readonly Dictionary<int, float> nextAllowedTime = new();

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Always operate on the PLAYER ROOT, not the child collider
        Transform playerRoot = other.transform.root;

        var bubble = playerRoot.GetComponent<PlayerBubbleState>();
        var health = playerRoot.GetComponent<PlayerHealth>();

        if (health == null) return;

        // If bubbled/dead, ignore completely
        if (bubble != null && bubble.IsBubbled)
            return;

        // Cooldown guard (prevents flicker if they immediately re-enter trigger)
        int id = playerRoot.gameObject.GetInstanceID();
        if (nextAllowedTime.TryGetValue(id, out float t) && Time.time < t)
            return;

        nextAllowedTime[id] = Time.time + reentryCooldown;

        // Lethal hit: deal damage but DO NOT teleport.
        // Bubble system will handle the rest.
        if (health.CurrentHealth - damage <= 0)
        {
            health.TakeDamage(damage, transform.position);
            return;
        }

        // Non-lethal: damage + teleport to respawn
        health.TakeDamage(damage, transform.position);

        if (respawnPoint != null)
        {
            // Reset motion so they don't instantly fall back in
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
