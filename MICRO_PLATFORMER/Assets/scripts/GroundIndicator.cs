using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] float groundOffset = 0.02f;
    [SerializeField] LayerMask groundLayer;

    Transform target;

    public void SetTarget(Transform player)
    {
        target = player;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 rayOrigin = target.position + Vector3.up;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * groundOffset;
        }
        else
        {
            // Fallback if no ground found
            transform.position = new Vector3(
                target.position.x,
                target.position.y - 1f,
                target.position.z
            );
        }
    }
}
