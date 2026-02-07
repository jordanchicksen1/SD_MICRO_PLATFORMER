using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HubFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float followDistance = 1.6f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotateSpeed = 12f;

    [Header("Ground Snap")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float snapFromAbove = 10f;
    [SerializeField] float snapDownDistance = 30f;
    [SerializeField] float groundOffset = 0.02f;

    [SerializeField] Collider followerCollider;

    HubPlayerAnimator anim;


    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (groundMask == 0)
            groundMask = LayerMask.GetMask("GroundLayer");

        if (followerCollider == null)
            followerCollider = GetComponent<Collider>();

        anim = GetComponentInChildren<HubPlayerAnimator>();


    }

    void FixedUpdate()
    {
        if (!target) return;

        Vector3 pos = rb.position;

        Vector3 startPos = rb.position;


        // Follow in XZ
        Vector3 toTarget = target.position - pos;
        toTarget.y = 0f;

        if (toTarget.magnitude > followDistance)
        {
            Vector3 dir = toTarget.normalized;
            pos += dir * moveSpeed * Time.fixedDeltaTime;

            // Match your player’s “backwards” facing
            Quaternion rot = Quaternion.LookRotation(-dir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, rot, rotateSpeed * Time.fixedDeltaTime));
        }

        // Snap to ground (GroundLayer only)
       
        Vector3 origin = pos + Vector3.up * snapFromAbove;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, snapDownDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            float halfHeight = 0.5f;
            if (followerCollider != null)
                halfHeight = followerCollider.bounds.extents.y;

            pos.y = hit.point.y + halfHeight + groundOffset;
        }


        rb.MovePosition(pos);

        if (anim != null)
        {
            float moved = (pos - startPos).magnitude;
            float speed01 = Mathf.Clamp01(moved / (moveSpeed * Time.fixedDeltaTime));
            anim.SetMoveBlend(speed01);
        }

    }

    public void SetTarget(Transform t) => target = t;
}
