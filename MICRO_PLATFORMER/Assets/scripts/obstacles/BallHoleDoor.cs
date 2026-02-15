using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] Vector3 openOffset = new Vector3(0, 3f, 0);
    [SerializeField] float openTime = 0.4f;
    public AudioSource doorSFX;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

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
        StartCoroutine(MoveDoor(closedPos, openPos));
    }

    IEnumerator MoveDoor(Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / openTime;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.position = to;
        doorSFX.Play();
    }
}
