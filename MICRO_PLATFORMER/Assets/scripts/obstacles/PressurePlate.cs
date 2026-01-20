using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] ButtonVisual visual;

    int count;
    bool isOpen;

    void Awake()
    {
        if (visual == null) visual = GetComponent<ButtonVisual>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        count++;
        UpdateState();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        count = Mathf.Max(0, count - 1);
        UpdateState();
    }

    void UpdateState()
    {
        bool shouldBeOpen = count > 0;

        if (shouldBeOpen != isOpen)
        {
            isOpen = shouldBeOpen;
            door?.Toggle();
        }

        if (shouldBeOpen) visual?.Press();
        else visual?.Release();
    }
}
