using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;

    void OnEnable()
    {
        // If manager isn't ready yet, try again next frame
        if (CurrencyManager.Instance == null)
        {
            Invoke(nameof(TryHook), 0f);
            return;
        }

        Hook();
    }

    void TryHook()
    {
        if (CurrencyManager.Instance == null) return;
        Hook();
    }

    void Hook()
    {
        CurrencyManager.Instance.OnCoinsChanged += UpdateCoins;
        CurrencyManager.Instance.OnGemsChanged += UpdateGems;

        // Force initial refresh so UI is correct on scene load
        UpdateCoins(CurrencyManager.Instance.Coins);
        UpdateGems(CurrencyManager.Instance.Gems);
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance == null) return;

        CurrencyManager.Instance.OnCoinsChanged -= UpdateCoins;
        CurrencyManager.Instance.OnGemsChanged -= UpdateGems;
    }

    void UpdateCoins(int value)
    {
        if (coinsText != null)
            coinsText.text = value.ToString();
    }

    void UpdateGems(int value)
    {
        if (gemsText != null)
            gemsText.text = value.ToString();
    }
}
