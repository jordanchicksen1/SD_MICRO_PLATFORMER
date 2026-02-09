using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelResultsUI : MonoBehaviour
{
    [Header("Root Panel (optional)")]
    [SerializeField] GameObject root; // If not set, we’ll use this GameObject

    [Header("Text")]
    [SerializeField] TextMeshProUGUI levelCoinsText;
    [SerializeField] TextMeshProUGUI levelGemsText;
    [SerializeField] TextMeshProUGUI totalCoinsText;
    [SerializeField] TextMeshProUGUI totalGemsText;

    [Header("Buttons")]
    [SerializeField] Button continueButton;
    [SerializeField] string hubSceneName = "HubWorld";

    bool shown;

    void Awake()
    {
        // If you put this script on the panel itself, root can be left empty.
        if (!root) root = gameObject;

        // Start hidden
        root.SetActive(false);

        if (continueButton)
            continueButton.onClick.AddListener(ContinueToHub);
    }

    public void Show()
    {
        if (shown) return;
        shown = true;

        // Turn the panel on
        if (!root) root = gameObject;
        root.SetActive(true);

        int levelCoins = RunCurrency.Instance ? RunCurrency.Instance.LevelCoins : 0;
        int levelGems = RunCurrency.Instance ? RunCurrency.Instance.LevelGems : 0;

        int totalCoins = CurrencyManager.Instance ? CurrencyManager.Instance.Coins : 0;
        int totalGems = CurrencyManager.Instance ? CurrencyManager.Instance.Gems : 0;

        if (levelCoinsText) levelCoinsText.text = levelCoins.ToString();
        if (levelGemsText) levelGemsText.text = levelGems.ToString();

        // Show what totals WILL be after banking
        if (totalCoinsText) totalCoinsText.text = (totalCoins + levelCoins).ToString();
        if (totalGemsText) totalGemsText.text = (totalGems + levelGems).ToString();

        // Pause gameplay
        Time.timeScale = 0f;

        // Select button for controller navigation
        if (EventSystem.current && continueButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }
    }

    void ContinueToHub()
    {
        // Resume gameplay time
        Time.timeScale = 1f;

        // Bank level currency into persistent totals
        RunCurrency.Instance?.CommitToBank();

        SceneManager.LoadScene(hubSceneName);
    }
}
