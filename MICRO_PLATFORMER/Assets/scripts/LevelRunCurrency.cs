using TMPro;
using UnityEngine;

public class LevelRunCurrencyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;

    void Start()
    {
        // In case RunCurrency spawns slightly later, do an initial refresh
        Refresh();

        if (RunCurrency.Instance != null)
        {
            RunCurrency.Instance.OnLevelCoinsChanged += OnCoinsChanged;
            RunCurrency.Instance.OnLevelGemsChanged += OnGemsChanged;
        }
        else
        {
            Debug.LogError("[LevelRunCurrencyUI] No RunCurrency in the scene!");
        }
    }

    void OnDestroy()
    {
        if (RunCurrency.Instance != null)
        {
            RunCurrency.Instance.OnLevelCoinsChanged -= OnCoinsChanged;
            RunCurrency.Instance.OnLevelGemsChanged -= OnGemsChanged;
        }
    }

    void OnCoinsChanged(int _) => Refresh();
    void OnGemsChanged(int _) => Refresh();

    void Refresh()
    {
        if (RunCurrency.Instance == null) return;

        if (coinsText) coinsText.text = RunCurrency.Instance.LevelCoins.ToString();
        if (gemsText) gemsText.text = RunCurrency.Instance.LevelGems.ToString();
    }
}
