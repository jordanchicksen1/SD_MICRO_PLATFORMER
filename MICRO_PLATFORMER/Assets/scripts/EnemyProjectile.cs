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

    private void OnCollisionEnter(Collision other)
    {
        // Ignore enemy + enemy triggers
        if (other.gameObject.CompareTag("Enemy"))
            return;

        // Damage player
        PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
            Destroy(gameObject);
            return;
        }

        // Hit environment (ground, walls, etc.)
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
