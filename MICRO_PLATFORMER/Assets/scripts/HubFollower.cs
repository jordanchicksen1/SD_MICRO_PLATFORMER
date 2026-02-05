using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HubFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float followDistance = 1.6f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotateSpeed = 12f;

    Rigidbody rb;
    Vector3 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        lastPos = transform.position;
    }

    void Update()
    {
        if (!target) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude > followDistance)
        {
            Vector3 dir = toTarget.normalized;
            transform.position += dir * (moveSpeed * Time.deltaTime);

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }

        // make PlayerAnimator "see" velocity
        Vector3 frameVel = (transform.position - lastPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        rb.linearVelocity = frameVel;
        lastPos = transform.position;
    }

    public void SetTarget(Transform t) => target = t;
}
