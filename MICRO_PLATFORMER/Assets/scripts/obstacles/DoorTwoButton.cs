using UnityEngine;

public class CoopDoor : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] int requiredButtons = 2;
    [SerializeField] float openHeight = 4f;
    [SerializeField] float openSpeed = 2f;

    int pressedButtons;
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

    public void ButtonPressed()
    {
        pressedButtons++;
        CheckDoor();
    }

    

    void CheckDoor()
    {
        isOpen = pressedButtons >= requiredButtons;
    }
}
