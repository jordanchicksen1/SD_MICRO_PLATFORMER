using TMPro;
using UnityEngine;

public class HubCurrencyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;

    void OnEnable()
    {
        // Subscribe
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += OnCoinsChanged;
            CurrencyManager.Instance.OnGemsChanged += OnGemsChanged;
        }

        // Always refresh when the hub UI becomes active
        Refresh();
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
            CurrencyManager.Instance.OnGemsChanged -= OnGemsChanged;
        }
    }

    void OnCoinsChanged(int _) => Refresh();
    void OnGemsChanged(int _) => Refresh();

    void Refresh()
    {
        var cm = CurrencyManager.Instance;
        if (cm == null) return;

        if (coinsText) coinsText.text = cm.Coins.ToString();
        if (gemsText) gemsText.text = cm.Gems.ToString();
    }
}
