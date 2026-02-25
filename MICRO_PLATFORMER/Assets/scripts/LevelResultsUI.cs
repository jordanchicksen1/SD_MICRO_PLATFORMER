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

    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    bool shown;

    void Awake()
    {
        shown = false;

        if (!root) root = gameObject;
        root.SetActive(false);

        if (continueButton)
            continueButton.onClick.AddListener(ContinueToHub);
    }

    void OnEnable()
    {
        // Safety: rebind on enable (Unity sometimes duplicates listeners in editor)
        if (continueButton)
        {
            continueButton.onClick.RemoveListener(ContinueToHub);
            continueButton.onClick.AddListener(ContinueToHub);
        }
    }

    public void Show()
    {
        if (shown) return;
        shown = true;

        if (!root) root = gameObject;
        root.SetActive(true);

        // Switch ALL players to UI map so ANY controller can drive the menu
        PlayerInputUtil.EnterUIMode(uiMapName);

        // Select button
        if (EventSystem.current && continueButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            EventSystem.current.sendNavigationEvents = true;
        }

        int levelCoins = RunCurrency.Instance ? RunCurrency.Instance.LevelCoins : 0;
        int levelGems = RunCurrency.Instance ? RunCurrency.Instance.LevelGems : 0;

        int bankCoins = CurrencyManager.Instance ? CurrencyManager.Instance.Coins : 0;
        int bankGems = CurrencyManager.Instance ? CurrencyManager.Instance.Gems : 0;

        if (levelCoinsText) levelCoinsText.text = levelCoins.ToString();
        if (levelGemsText) levelGemsText.text = levelGems.ToString();

        if (totalCoinsText) totalCoinsText.text = (bankCoins + levelCoins).ToString();
        if (totalGemsText) totalGemsText.text = (bankGems + levelGems).ToString();

        Time.timeScale = 0f;

        Debug.Log($"[LevelResultsUI] Show. bank={bankCoins}/{bankGems} run={levelCoins}/{levelGems}");
    }

    void ContinueToHub()
    {
        Debug.Log("Committing levelId: " + RunLevelInfo.Instance?.LevelId);
        Debug.Log("[LevelResultsUI] ContinueToHub pressed!");

        Time.timeScale = 1f;

        RunCurrency.Instance?.CommitToBank();

        // Commit permanent gem progress (if you're using that system)
        if (RunLevelInfo.Instance != null && PersistentGemProgress.Instance != null)
        {
            PersistentGemProgress.Instance.CommitPendingFromRun(
                RunLevelInfo.Instance.LevelId
            );
        }

        // Restore gameplay maps before loading out (prevents “stuck in UI map”)
        PlayerInputUtil.ExitUIMode(gameplayMapName);

        SceneManager.LoadScene(hubSceneName);
    }
}
