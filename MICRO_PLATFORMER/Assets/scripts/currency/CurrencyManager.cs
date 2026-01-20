using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Coins { get; private set; }
    public int Gems { get; private set; }

    // Optional: UI can subscribe to this
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return true;
        if (Coins < amount) return false;

        Coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0) return true;
        if (Gems < amount) return false;

        Gems -= amount;
        OnGemsChanged?.Invoke(Gems);
        return true;
    }

    public bool HasCoins(int amount) => Coins >= amount;
    public bool HasGems(int amount) => Gems >= amount;

    // Optional: for debugging / cheats
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
