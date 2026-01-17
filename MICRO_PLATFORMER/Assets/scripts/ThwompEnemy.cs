using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ThwompEnemy : MonoBehaviour
{
    enum ThwompState
    {
        Idle,
        Shaking,
        Slamming,
        BottomWait,   
        Rising
    }


    [Header("Movement")]
    [SerializeField] float slamSpeed = 30f;
    [SerializeField] float riseSpeed = 6f;
    [SerializeField] float waitAtBottom = 1f;

    [Header("Detection")]
    [SerializeField] float detectionWidth = 2f;
    [SerializeField] float detectionDepth = 2f;
    [SerializeField] float detectionDistance = 10f;

    [Header("Shake")]
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeAmount = 0.08f;

    [Header("Damage")]
    [SerializeField] int damage = 1;

    [Header("Shadow")]
    [SerializeField] GameObject groundIndicatorPrefab;
    [SerializeField] Material shadowMaterial;


    Rigidbody rb;
    Vector3 startPos;
    ThwompState state = ThwompState.Idle;

    Transform carriedPlayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        startPos = transform.position;

        GameObject shadow = Instantiate(groundIndicatorPrefab, transform.position, Quaternion.identity);

        GroundIndicator gi = shadow.GetComponent<GroundIndicator>();
        gi.SetTarget(transform);

        // Apply shadow material
        Renderer r = shadow.GetComponentInChildren<Renderer>();
        if (r != null && shadowMaterial != null)
        {
            r.material = shadowMaterial;
        }

    }

    void FixedUpdate()
    {
        switch (state)
        {
            case ThwompState.Idle:
                rb.linearVelocity = Vector3.zero;
                DetectPlayerBelow();
                break;

            case ThwompState.Slamming:
                rb.linearVelocity = Vector3.down * slamSpeed;
                break;

            case ThwompState.BottomWait:   // ? NEW
                rb.linearVelocity = Vector3.zero;
                break;

            case ThwompState.Rising:
                rb.linearVelocity = Vector3.up * riseSpeed;

                if (transform.position.y >= startPos.y)
                {
                    transform.position = startPos;
                    rb.linearVelocity = Vector3.zero;
                    state = ThwompState.Idle;
                }
                break;
        }
    }


    // ---------------- DETECTION ----------------

    void DetectPlayerBelow()
    {
        Vector3 boxCenter = transform.position + Vector3.down * (detectionDistance * 0.5f);

        Collider[] hits = Physics.OverlapBox(
            boxCenter,
            new Vector3(detectionWidth, detectionDistance * 0.5f, detectionDepth),
            Quaternion.identity
        );

        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<PlayerController3D>())
            {
                StartCoroutine(ShakeAndSlam());
                break;
            }
        }
    }

    // ---------------- SHAKE + SLAM ----------------

    IEnumerator ShakeAndSlam()
    {
        if (state != ThwompState.Idle)
            yield break;

        state = ThwompState.Shaking;

        Vector3 basePos = transform.position;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float offset = Random.Range(-1f, 1f) * shakeAmount;
            transform.position = basePos + Vector3.right * offset;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = basePos;

        StartSlam();
    }

    void StartSlam()
    {
        if (carriedPlayer)
        {
            carriedPlayer.SetParent(null);
            carriedPlayer = null;
        }

        state = ThwompState.Slamming;
    }

    // ---------------- COLLISIONS ----------------

    void OnCollisionEnter(Collision collision)
    {
        // Damage player ONLY while slamming
        if (state == ThwompState.Slamming &&
            collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage, transform.position);
        }

        // Hit ground ? stop slam and wait at bottom (no damage during wait)
        if (state == ThwompState.Slamming &&
            collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector3.zero;
            state = ThwompState.BottomWait;      
            StartCoroutine(RiseAfterDelay());


        }


        // Carry player while rising
        if (state == ThwompState.Rising &&
            collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].normal.y > 0.5f)
            {
                carriedPlayer = collision.transform;
                carriedPlayer.SetParent(transform);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform == carriedPlayer)
        {
            carriedPlayer.SetParent(null);
            carriedPlayer = null;
        }
    }

    IEnumerator RiseAfterDelay()
    {
        yield return new WaitForSeconds(waitAtBottom);

        // Only rise if we’re still waiting (prevents weird double triggers)
        if (state == ThwompState.BottomWait)
            state = ThwompState.Rising;
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.down * (detectionDistance * 0.5f);
        Gizmos.DrawWireCube(
            center,
            new Vector3(detectionWidth * 2, detectionDistance, detectionDepth * 2)
        );
    }
#endif
}
