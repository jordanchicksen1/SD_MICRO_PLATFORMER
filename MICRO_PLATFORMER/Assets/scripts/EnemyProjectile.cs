using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 1;
    [SerializeField] float lifetime = 4f;

    Vector3 direction;

    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore enemy + enemy triggers
        if (other.CompareTag("Enemy"))
            return;

        // Damage player
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
            Destroy(gameObject);
            return;
        }

        // Hit environment (ground, walls, etc.)
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
