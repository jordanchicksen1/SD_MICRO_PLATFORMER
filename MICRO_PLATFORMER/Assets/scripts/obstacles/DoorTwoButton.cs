using UnityEngine;

public class CoopDoor : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] float openHeight = 4f;
    [SerializeField] float openSpeed = 2f;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

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

    // Called by a "manager" script (below)
    public void SetOpen(bool open)
    {
        isOpen = open;
    }
}
