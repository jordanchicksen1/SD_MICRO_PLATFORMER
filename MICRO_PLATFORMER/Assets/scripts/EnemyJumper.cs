using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyJumper : MonoBehaviour
{
    [Header("Hop Movement")]
    [SerializeField] float hopForwardSpeed = 6f;
    [SerializeField] float hopUpForce = 6f;
    [SerializeField] float hopInterval = 0.8f;

    [Header("Targeting")]
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float rotationSpeed = 10f;

    [Tooltip("Turn this on if your model visually faces backwards.")]
    [SerializeField] bool modelForwardIsBackwards = false;

    Rigidbody rb;
    Transform target;
    float hopTimer;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        FindTarget();

        // ? ALWAYS face the player while they're in range (even between hops)
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                dir.Normalize();
                Vector3 lookDir = modelForwardIsBackwards ? -dir : dir;

                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
            }
        }

        hopTimer -= Time.fixedDeltaTime;

        if (!grounded) return;
        if (hopTimer > 0f) return;

        hopTimer = hopInterval;

        // ? If no target, hop in place (no forward movement)
        Vector3 hopDir = Vector3.zero;

        if (target != null)
        {
            hopDir = target.position - transform.position;
            hopDir.y = 0f;
            if (hopDir.sqrMagnitude > 0.0001f)
                hopDir.Normalize();
            else
                hopDir = Vector3.zero;
        }

        rb.linearVelocity = new Vector3(
            hopDir.x * hopForwardSpeed,
            hopUpForce,
            hopDir.z * hopForwardSpeed
        );
    }

    void FindTarget()
    {
        float closest = chaseRange;
        target = null;

        foreach (PlayerController3D p in FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None))
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < closest)
            {
                closest = d;
                target = p.transform;
            }
        }
    }

    // Ground detection via collision normals
    void OnCollisionStay(Collision c)
    {
        for (int i = 0; i < c.contactCount; i++)
        {
            if (c.contacts[i].normal.y > 0.5f)
            {
                grounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision c)
    {
        grounded = false;
    }


    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(1, transform.position);
        }
    }
}
