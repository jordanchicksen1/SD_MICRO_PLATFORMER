using System.Collections;
using UnityEngine;

public class DoorOpenSimple : MonoBehaviour
{
    [Header("Open Settings")]
    [SerializeField] Vector3 openOffset = new Vector3(0, 4f, 0); // move up like Mario door
    [SerializeField] float openDuration = 0.6f;

    bool isOpen;
    Vector3 closedPos;
    Vector3 openPos;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StopAllCoroutines();
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / openDuration;
            transform.position = Vector3.Lerp(closedPos, openPos, t);
            yield return null;
        }

        transform.position = openPos;
    }
}
