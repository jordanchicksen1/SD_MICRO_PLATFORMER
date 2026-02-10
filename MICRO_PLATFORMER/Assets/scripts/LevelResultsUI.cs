using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelResultsUI : MonoBehaviour
{
    [SerializeField] GameObject root;

    [SerializeField] TextMeshProUGUI levelCoinsText;
    [SerializeField] TextMeshProUGUI levelGemsText;
    [SerializeField] TextMeshProUGUI totalCoinsText;
    [SerializeField] TextMeshProUGUI totalGemsText;

    [SerializeField] Button continueButton;
    [SerializeField] string hubSceneName = "HubWorld";

    bool shown;

    void Awake()
    {
        shown = false;

        if (!root) root = gameObject;
        root.SetActive(false);

        if (continueButton)
            continueButton.onClick.AddListener(ContinueToHub);
    }

    public void Show()
    {
        if (shown) return;
        shown = true;

        if (!root) root = gameObject;
        root.SetActive(true);

        int levelCoins = RunCurrency.Instance ? RunCurrency.Instance.LevelCoins : 0;
        int levelGems = RunCurrency.Instance ? RunCurrency.Instance.LevelGems : 0;

        int bankCoins = CurrencyManager.Instance ? CurrencyManager.Instance.Coins : 0;
        int bankGems = CurrencyManager.Instance ? CurrencyManager.Instance.Gems : 0;

        if (levelCoinsText) levelCoinsText.text = levelCoins.ToString();
        if (levelGemsText) levelGemsText.text = levelGems.ToString();

        if (totalCoinsText) totalCoinsText.text = (bankCoins + levelCoins).ToString();
        if (totalGemsText) totalGemsText.text = (bankGems + levelGems).ToString();

        Time.timeScale = 0f;

        if (EventSystem.current && continueButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }

        Debug.Log($"[LevelResultsUI] Show. bank={bankCoins}/{bankGems} run={levelCoins}/{levelGems}");
    }

    void ContinueToHub()
    {
        Debug.Log("[LevelResultsUI] ContinueToHub pressed!");

        Time.timeScale = 1f;

        // Bank coins + gems into player totals
        RunCurrency.Instance?.CommitToBank();

        // Commit permanent gem progress for THIS level only
        if (RunLevelInfo.Instance != null && PersistentGemProgress.Instance != null)
        {
            PersistentGemProgress.Instance.CommitPendingFromRun(
                RunLevelInfo.Instance.LevelId
            );
        }

        SceneManager.LoadScene(hubSceneName);
    }
}
