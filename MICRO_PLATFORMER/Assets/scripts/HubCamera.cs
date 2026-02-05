using UnityEngine;

public class HubCameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 10f, -12f);
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] bool lookAtTarget = true;

    Vector3 vel;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);

        if (lookAtTarget)
            transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public void SetTarget(Transform t) => target = t;
}
