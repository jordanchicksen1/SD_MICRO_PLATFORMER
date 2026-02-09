using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Coins { get; private set; }
    public int Gems { get; private set; }

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    // If Domain Reload is disabled, statics can survive between plays.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    // Create the bank BEFORE the first scene loads.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureExists()
    {
        if (Instance != null) return;

        var go = new GameObject("CurrencyManager");
        go.AddComponent<CurrencyManager>();
        DontDestroyOnLoad(go);
        Debug.Log("Added currency manager");
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

        Debug.Log($"[CurrencyManager] Awake. Coins={Coins} Gems={Gems} id={GetInstanceID()}");
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;
        Gems += amount;
        OnGemsChanged?.Invoke(Gems);
    }

    public void SetCoins(int value)
    {
        Coins = Mathf.Max(0, value);
        OnCoinsChanged?.Invoke(Coins);
    }

    public void SetGems(int value)
    {
        Gems = Mathf.Max(0, value);
        OnGemsChanged?.Invoke(Gems);
    }
}
