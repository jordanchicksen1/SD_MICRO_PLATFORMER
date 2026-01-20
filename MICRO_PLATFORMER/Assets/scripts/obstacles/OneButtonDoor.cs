using UnityEngine;

public class OneButtonDoor : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] ButtonVisual visual;

    void Awake()
    {
        if (visual == null) visual = GetComponent<ButtonVisual>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        visual?.Press();
        door?.Toggle();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        visual?.Release();
    }
}
