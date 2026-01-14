using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float openHeight = 3f;
    [SerializeField] float speed = 3f;

    Vector3 closedPos;
    Vector3 openPos;
    bool open;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 target = open ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    }

    public void Toggle()
    {
        open = !open;
    }
}
