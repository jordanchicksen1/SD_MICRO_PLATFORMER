using UnityEngine;

public class CoopDoor : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] float openHeight = 4f;
    [SerializeField] float openSpeed = 2f;

    Vector3 closedPos;
    Vector3 openPos;

    bool isOpen;
    bool openedEver;

    public bool IsOpen => isOpen;
    public bool HasOpenedEver => openedEver;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            openSpeed * Time.deltaTime
        );
    }

    // Returns true only if this call transitions closed->open AND it's the first time ever opened
    public bool SetOpen(bool open)
    {
        bool wasOpen = isOpen;
        isOpen = open;

        if (!wasOpen && isOpen)
        {
            bool firstTime = !openedEver;
            openedEver = true;
            return firstTime;
        }

        return false;
    }
}
