using UnityEngine;

public class HubProgressionManager : MonoBehaviour
{
    public static HubProgressionManager Instance { get; private set; }

    bool island2Unlocked;

    public bool Island2Unlocked => island2Unlocked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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

        Debug.Log(
            $"[HubProgressionManager] Awake. Island2Unlocked={island2Unlocked}"
        );
    }

    public void UnlockIsland2()
    {
        island2Unlocked = true;

        Debug.Log(
            "[HubProgressionManager] Island 2 unlocked!"
        );
    }
}