using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [SerializeField] CoopDoor door;
    [SerializeField] float pressDepth = 0.2f;

    Vector3 startPos;
    bool pressed;

    void Awake()
    {
        startPos = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;

        if (other.GetComponent<PlayerController3D>())
        {
            pressed = true;
            transform.position = startPos + Vector3.down * pressDepth;
            door.ButtonPressed();
        }
    }

   
}
