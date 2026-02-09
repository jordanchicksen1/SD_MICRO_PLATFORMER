using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubCurrencyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;

    CurrencyManager cm;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(InitWhenReady());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (cm != null)
        {
            cm.OnCoinsChanged -= OnCoinsChanged;
            cm.OnGemsChanged -= OnGemsChanged;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When Hub loads again, re-init + refresh
        StartCoroutine(InitWhenReady());
    }

    IEnumerator InitWhenReady()
    {
        // Unsubscribe from previous instance (just in case)
        if (cm != null)
        {
            cm.OnCoinsChanged -= OnCoinsChanged;
            cm.OnGemsChanged -= OnGemsChanged;
            cm = null;
        }

        // Wait until CurrencyManager exists
        while (CurrencyManager.Instance == null)
            yield return null;

        cm = CurrencyManager.Instance;

        cm.OnCoinsChanged += OnCoinsChanged;
        cm.OnGemsChanged += OnGemsChanged;

        Refresh();
    }

    void OnCoinsChanged(int _) => Refresh();
    void OnGemsChanged(int _) => Refresh();

    void Refresh()
    {
        if (cm == null) return;
        if (coinsText) coinsText.text = cm.Coins.ToString();
        if (gemsText) gemsText.text = cm.Gems.ToString();
    }
}
