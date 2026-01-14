using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] int damage = 1;

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
        }
    }
}
