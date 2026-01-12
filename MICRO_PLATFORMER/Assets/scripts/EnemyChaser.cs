using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChaser : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float rotationSpeed = 8f;

    [Header("Combat")]
    [SerializeField] int damage = 1;

    Rigidbody rb;
    Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        FindTarget();

        if (target == null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 dir = (target.position - transform.position);
        dir.y = 0;
        dir.Normalize();

        rb.linearVelocity = new Vector3(
            dir.x * moveSpeed,
            rb.linearVelocity.y,
            dir.z * moveSpeed
        );

        Quaternion lookRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime)
        );
    }

    void FindTarget()
    {
        float closest = detectionRange;
        target = null;

        foreach (PlayerController3D player in FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None))
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = player.transform;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
        }
    }

}
