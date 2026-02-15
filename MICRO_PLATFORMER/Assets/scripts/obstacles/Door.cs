using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float openHeight = 3f;
    [SerializeField] float speed = 3f;
    
    Vector3 closedPos;
    Vector3 openPos;

    bool open;
    bool openedEver;

    public bool IsOpen => open;
    public bool HasOpenedEver => openedEver;

    public AudioSource doorSFX;

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

    // Returns true ONLY the first time it ever opens
    public bool Open()
    {
        doorSFX.Play();
        if (open) return false;

        open = true;

        bool firstTime = !openedEver;
        openedEver = true;

        return firstTime;
    }

    public void Close()
    {
        doorSFX.Play();
        open = false;
    }

    // Keep Toggle if you still want it elsewhere
    public void Toggle()
    {
        if (open) Close();
        else Open();
    }
}
