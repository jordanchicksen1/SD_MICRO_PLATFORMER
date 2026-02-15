using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void LateUpdate()
    {
        if (!cam) return;

        Vector3 direction = transform.position - cam.transform.position;
        direction.y = 0f; // lock vertical tilt

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
