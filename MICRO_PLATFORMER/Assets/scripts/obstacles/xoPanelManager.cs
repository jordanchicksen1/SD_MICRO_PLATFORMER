using UnityEngine;

public class QuestionPanelManager : MonoBehaviour
{
    [SerializeField] int requiredCount = 3;
    [SerializeField] DoorOpenSimple door;

    int currentCount;
    bool doorOpened;

    public void AddPanel()
    {
        currentCount++;
        Debug.Log($"Panels: {currentCount}/{requiredCount}");
        CheckDoor();
    }

    public void RemovePanel()
    {
        currentCount--;
        if (currentCount < 0) currentCount = 0;

        Debug.Log($"Panels: {currentCount}/{requiredCount}");
        // If your door can’t close, we just don’t reopen/close it.
        // If you later want a closing door, we can add it here.
    }

    void CheckDoor()
    {
        if (!doorOpened && currentCount >= requiredCount)
        {
            doorOpened = true;
            if (door != null) door.Open();
        }
    }
}
