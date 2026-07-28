using UnityEngine;

public class TargetMarkerAnimator : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] float floatHeight = 0.2f;
    [SerializeField] float floatSpeed = 3f;

    Vector3 startLocalPosition;

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = startLocalPosition + Vector3.up * y;
    }
}