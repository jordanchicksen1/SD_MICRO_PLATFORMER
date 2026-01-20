using UnityEngine;

public class CoopDoorController : MonoBehaviour
{
    [SerializeField] CoopDoor door;
    [SerializeField] ToggleButton buttonA;
    [SerializeField] ToggleButton buttonB;

    void Awake()
    {
        if (door == null) door = GetComponent<CoopDoor>();
    }

    public void RecomputeDoor()
    {
        bool open = buttonA != null && buttonB != null &&
                    buttonA.IsPressed && buttonB.IsPressed;

        door.SetOpen(open);
    }
}
