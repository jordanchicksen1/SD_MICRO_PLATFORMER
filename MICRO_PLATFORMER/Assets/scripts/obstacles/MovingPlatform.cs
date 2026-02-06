using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float speed = 2f;
    [SerializeField] float arriveDistance = 0.05f;

    public Vector3 Velocity { get; private set; }  // <-- key

    Rigidbody rb;
    Vector3 target;
    Vector3 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        target = pointB.position;
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        Vector3 next = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        Velocity = (next - lastPos) / Time.fixedDeltaTime;
        lastPos = next;

        if (Vector3.Distance(next, target) < arriveDistance)
            target = (target == pointA.position) ? pointB.position : pointA.position;
    }
}
