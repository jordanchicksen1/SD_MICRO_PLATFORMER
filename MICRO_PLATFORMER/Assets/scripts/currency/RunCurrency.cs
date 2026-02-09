using System;
using UnityEngine;

public class RunCurrency : MonoBehaviour
{
    public static RunCurrency Instance { get; private set; }

    public int LevelCoins { get; private set; }
    public int LevelGems { get; private set; }

    public event Action<int> OnLevelCoinsChanged;
    public event Action<int> OnLevelGemsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // IMPORTANT: do NOT DontDestroyOnLoad — this is per-level
    }

    public void ResetRun()
    {
        LevelCoins = 0;
        LevelGems = 0;
        OnLevelCoinsChanged?.Invoke(LevelCoins);
        OnLevelGemsChanged?.Invoke(LevelGems);
    }

    public void AddCoin(int amount = 1)
    {
        if (amount <= 0) return;
        LevelCoins += amount;
        OnLevelCoinsChanged?.Invoke(LevelCoins);
    }

    public void AddGem(int amount = 1)
    {
        if (amount <= 0) return;
        LevelGems += amount;
        OnLevelGemsChanged?.Invoke(LevelGems);
    }

    public void CommitToBank()
    {
        if (CurrencyManager.Instance == null) return;

        CurrencyManager.Instance.AddCoins(LevelCoins);
        CurrencyManager.Instance.AddGems(LevelGems);
    }
}
