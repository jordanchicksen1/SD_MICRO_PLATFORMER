using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    PauseManager pauseManager; // NEW
    bool shown;

    void Awake()
    {
        shown = false;

        if (!root) root = gameObject;
        root.SetActive(false);

        pauseManager = FindFirstObjectByType<PauseManager>(); // NEW

        if (continueButton)
            continueButton.onClick.AddListener(ContinueToHub);
    }

    PlayerInput FindAnyActivePlayerInput()
    {
        // FindObjectsByType returns also inactive if you ask it to — we DON'T want inactive.
        // So we use the default and filter for activeInHierarchy.
        var inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        foreach (var pi in inputs)
        {
            if (!pi) continue;
            if (!pi.gameObject.activeInHierarchy) continue;

            // Optional: only players, if you tag them
            // if (!pi.CompareTag("Player")) continue;

            return pi;
        }

        return null;
    }


    public void Show()
    {
        if (shown) return;
        shown = true;

        if (!root) root = gameObject;
        root.SetActive(true);

        // NEW: kill pause UI + prevent PauseManager from switching maps while results are open
        if (pauseManager)
        {
            pauseManager.SetPaused(false);
            pauseManager.SetPauseLocked(true);
        }

        // Stop gameplay + enter UI map
        foreach (var pi1 in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            Debug.Log($"[PI] name={pi1.name} active={pi1.gameObject.activeInHierarchy} enabled={pi1.enabled} inputIsActive={pi1.inputIsActive}");
        }


        var pi = FindAnyActivePlayerInput();
        if (pi != null)
        {
            PlayerInputUtil.SafeSwitchMap(pi, uiMapName);
        }
        else
        {
            Debug.LogWarning("[LevelResultsUI] No ACTIVE PlayerInput found. UI navigation may fail.");
        }

        // Pause the game (results still navigates because Input System is event-based)
        Time.timeScale = 0f;

        // UI selection
        if (EventSystem.current && continueButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.sendNavigationEvents = true;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }

        int levelCoins = RunCurrency.Instance ? RunCurrency.Instance.LevelCoins : 0;
        int levelGems = RunCurrency.Instance ? RunCurrency.Instance.LevelGems : 0;

        int bankCoins = CurrencyManager.Instance ? CurrencyManager.Instance.Coins : 0;
        int bankGems = CurrencyManager.Instance ? CurrencyManager.Instance.Gems : 0;

        if (levelCoinsText) levelCoinsText.text = levelCoins.ToString();
        if (levelGemsText) levelGemsText.text = levelGems.ToString();

        if (totalCoinsText) totalCoinsText.text = (bankCoins + levelCoins).ToString();
        if (totalGemsText) totalGemsText.text = (bankGems + levelGems).ToString();

        Debug.Log($"[LevelResultsUI] Show. bank={bankCoins}/{bankGems} run={levelCoins}/{levelGems}");
    }

    void ContinueToHub()
    {
        Debug.Log("[LevelResultsUI] ContinueToHub pressed!");

        Time.timeScale = 1f;

        RunCurrency.Instance?.CommitToBank();

        if (RunLevelInfo.Instance != null && PersistentGemProgress.Instance != null)
        {
            PersistentGemProgress.Instance.CommitPendingFromRun(RunLevelInfo.Instance.LevelId);
        }

        // NEW: unlock pause (clean)
        if (pauseManager)
            pauseManager.SetPauseLocked(false);

        var pi = FindAnyActivePlayerInput();
        if (pi != null)
        {
            PlayerInputUtil.SafeSwitchMap(pi, gameplayMapName);
        }

        SceneManager.LoadScene(hubSceneName);
    }
}
