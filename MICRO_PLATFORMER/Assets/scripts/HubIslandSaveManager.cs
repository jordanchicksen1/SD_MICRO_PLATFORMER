using UnityEngine;

public class HubIslandSaveManager : MonoBehaviour
{
    public static HubIslandSaveManager Instance { get; private set; }

    private const string CurrentIslandKey = "CurrentHubIsland";

    public int CurrentIsland { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureExists()
    {
        if (Instance != null)
            return;

        GameObject go =
            new GameObject("HubIslandSaveManager");

        go.AddComponent<HubIslandSaveManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CurrentIsland =
            PlayerPrefs.GetInt(
                CurrentIslandKey,
                0
            );
    }

    private void Start()
    {
    ResetIslandSave();
    }

    public void SetCurrentIsland(int islandID)
    {
        CurrentIsland = islandID;

        PlayerPrefs.SetInt(
            CurrentIslandKey,
            islandID
        );

        PlayerPrefs.Save();
    }

    public void ResetIslandSave()
    {
        PlayerPrefs.DeleteKey(CurrentIslandKey);
        PlayerPrefs.Save();

        CurrentIsland = 0;
    }
}