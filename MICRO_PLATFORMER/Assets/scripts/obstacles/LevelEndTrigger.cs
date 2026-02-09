using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] LevelResultsUI resultsUI;

    void Awake()
    {
        if (!resultsUI)
            resultsUI = FindFirstObjectByType<LevelResultsUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (resultsUI)
            resultsUI.Show();
    }
}
