using UnityEngine;

public class HubProgressionManager : MonoBehaviour
{
    public static HubProgressionManager Instance { get; private set; }

    private const string Island2UnlockedKey =
        "Hub_Island2Unlocked";

    bool island2Unlocked;

    public bool Island2Unlocked =>
        island2Unlocked;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    static void EnsureExists()
    {
        if (Instance != null)
            return;

        GameObject go =
            new GameObject("HubProgressionManager");

        go.AddComponent<HubProgressionManager>();
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

        LoadProgression();
    }

    private void Start()
    {
      ResetProgression();
    }

    void LoadProgression()
    {
        island2Unlocked =
            PlayerPrefs.GetInt(
                Island2UnlockedKey,
                0
            ) == 1;
    }

    public void UnlockIsland2()
    {
        island2Unlocked = true;

        PlayerPrefs.SetInt(
            Island2UnlockedKey,
            1
        );

        PlayerPrefs.Save();

        Debug.Log(
            "[HubProgressionManager] Island 2 unlocked!"
        );
    }

    public void ResetProgression()
    {
        island2Unlocked = false;

        PlayerPrefs.DeleteKey(Island2UnlockedKey);

        PlayerPrefs.Save();

        Debug.Log(
            "[HubProgressionManager] Progression reset."
        );
    }

    public bool IsIslandUnlocked(int islandID)
    {
        // The starting island is always unlocked.
        if (islandID == 0)
            return true;

        // Island 1 is the island currently controlled
        // by our existing Island2Unlocked save.
        if (islandID == 1)
            return island2Unlocked;

        // Any other island is not implemented yet.
        return false;
    }

    public void UnlockIsland(int islandID)
    {
        if (islandID == 0)
            return;

        if (islandID == 1)
        {
            UnlockIsland2();
        }
    }
}