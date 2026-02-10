using System;
using UnityEngine;

public class RunCurrency : MonoBehaviour
{
    public static RunCurrency Instance { get; private set; }

    public int LevelCoins { get; private set; }
    public int LevelGems { get; private set; }

    public event Action<int> OnLevelCoinsChanged;
    public event Action<int> OnLevelGemsChanged;

    bool banked;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("[RunCurrency] Awake -> ResetRun");
        ResetRun();
    }


    void Start()
    {
        Debug.Log("[RunCurrency] ResetRun on level start");
        ResetRun();
    }

    public void ResetRun()
    {
        LevelCoins = 0;
        LevelGems = 0;
        banked = false;

        OnLevelCoinsChanged?.Invoke(LevelCoins);
        OnLevelGemsChanged?.Invoke(LevelGems);

        Debug.Log($"[RunCurrency] ResetRun -> {LevelCoins}/{LevelGems}");

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
        if (banked) return;
        banked = true;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[RunCurrency] No CurrencyManager found! CommitToBank failed.");
            return;
        }

        CurrencyManager.Instance.AddCoins(LevelCoins);
        CurrencyManager.Instance.AddGems(LevelGems);

        Debug.Log($"[RunCurrency] CommitToBank: adding {LevelGems} gems to bank");

    }

}
