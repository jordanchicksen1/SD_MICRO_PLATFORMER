using UnityEngine;

public class RunLevelInfo : MonoBehaviour
{
    public static RunLevelInfo Instance { get; private set; }

    [SerializeField] string levelId;

    public string LevelId => levelId;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // NOT DontDestroyOnLoad — this is per level
    }
}
