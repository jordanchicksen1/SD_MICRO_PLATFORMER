using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 1;
    [SerializeField] float lifetime = 4f;
    Enemy owner;
    bool reflected;
    Collider ownerCollider;
    Vector3 direction;
    TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void Init(Vector3 dir, Enemy enemy)
    {
        direction = dir.normalized;
        owner = enemy;

        if (owner != null)
            ownerCollider = owner.GetComponent<Collider>();

        if (trail != null)
            trail.emitting = true;


        Destroy(gameObject, lifetime);
    }

    public void Reflect()
    {
        if (owner == null)
            return;

        reflected = true;

        Collider myCollider = GetComponent<Collider>();

        if (myCollider != null && ownerCollider != null)
        {
            Physics.IgnoreCollision(myCollider, ownerCollider, false);
        }

        direction = (owner.transform.position - transform.position).normalized;

        speed *= 1.5f; // Optional, but feels great
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Ignore enemy + enemy triggers
        // Hit enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (reflected)
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    Vector3 knockbackDirection =
                        (enemy.transform.position - transform.position).normalized;

                    enemy.TakeKick(knockbackDirection);

                    Destroy(gameObject);
                }
            }

            return;
        }

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
