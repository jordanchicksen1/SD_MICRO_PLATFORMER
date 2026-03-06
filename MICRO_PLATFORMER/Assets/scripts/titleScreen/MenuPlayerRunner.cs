using UnityEngine;

public class MenuPlayerRunner : MonoBehaviour
{
    Rigidbody rb;
    PlayerAnimator anim;

    Vector3 startPosition;

    [SerializeField] float runSpeed = 5f;
    [SerializeField] float resetDistance = 2f;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponentInChildren<PlayerAnimator>();

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = transform.forward * runSpeed;

        if (anim != null)
            anim.SetMoveBlend(1f);

        // keep them from running away
        if (Vector3.Distance(startPosition, transform.position) > resetDistance)
        {
            transform.position = startPosition;
        }
    }
}