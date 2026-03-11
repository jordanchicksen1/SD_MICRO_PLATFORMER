using UnityEngine;

public class GemDescend : MonoBehaviour
{
    [SerializeField] float fallHeight = 8f;
    [SerializeField] float fallTime = 1.2f;

    Vector3 startPos;
    Vector3 targetPos;

    float t;

    void Start()
    {
        targetPos = transform.position;
        startPos = targetPos + Vector3.up * fallHeight;

        transform.position = startPos;
    }

    void Update()
    {
        if (t >= 1f) return;

        t += Time.deltaTime / fallTime;

        transform.position = Vector3.Lerp(startPos, targetPos, t);
    }
}