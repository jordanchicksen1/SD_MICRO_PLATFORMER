using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject mainPausePanel;

    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject firstSelectedButton;

    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    [Header("Sub Panels")]
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject retryConfirmPanel;
    [SerializeField] GameObject quitConfirmPanel;

    [SerializeField] GameObject controlsFirstButton;
    [SerializeField] GameObject retryFirstButton;
    [SerializeField] GameObject quitFirstButton;

    [SerializeField] string hubSceneName = "HubWorld";


    public bool IsPaused { get; private set; }

    void Start()
    {
        SetPaused(false);
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;

        if (pauseCanvas) pauseCanvas.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;

        // IMPORTANT: don’t deactivate PlayerInputs. Just switch maps safely.
        if (paused)
            PlayerInputUtil.EnterUIMode(uiMapName);
        else
            PlayerInputUtil.ExitUIMode(gameplayMapName);

        // Ensure UI selection
        if (paused && firstSelectedButton && EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        
    }

    void OpenSubPanel(GameObject panel, GameObject firstButton)
    {
        if (!panel) return;

        // Hide main panel
        if (mainPausePanel)
            mainPausePanel.SetActive(false);

        // Hide all other subpanels just in case
        if (controlsPanel) controlsPanel.SetActive(false);
        if (retryConfirmPanel) retryConfirmPanel.SetActive(false);
        if (quitConfirmPanel) quitConfirmPanel.SetActive(false);

        // Show requested panel
        panel.SetActive(true);

        if (EventSystem.current && firstButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }


    void CloseSubPanel(GameObject panel)
    {
        if (!panel) return;

        panel.SetActive(false);

        // Re-enable main pause panel
        if (mainPausePanel)
            mainPausePanel.SetActive(true);

        if (EventSystem.current && firstSelectedButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }



    public void Resume()
    {
        SetPaused(false);
    }

    public void Retry()
    {
        OpenSubPanel(retryConfirmPanel, retryFirstButton);
        Debug.Log("shows page that asks are you sure you want to restart the level");
    }

    public void QuitLevel()
    {
        OpenSubPanel(quitConfirmPanel, quitFirstButton);
        Debug.Log("shows page that asks are you sure you want to quit the level");
    }

    public void Controls()
    {
        OpenSubPanel(controlsPanel, controlsFirstButton);
        Debug.Log("show controls");
    }

    public void BackControlsPage()
    {
        CloseSubPanel(controlsPanel);
        Debug.Log("exit controls page");
    }

    public void BackRetryLevel()
    {
        CloseSubPanel(retryConfirmPanel);
        Debug.Log("turn off prompt that shows when you press Retry");
    }

    public void BackQuitLevel()
    {
        CloseSubPanel(quitConfirmPanel);
        Debug.Log("turn off prompt that shows when you press Quit");
    }

    public void ConfirmQuitLevel()
    {
        Time.timeScale = 1f;

        PlayerInputUtil.ExitUIMode(gameplayMapName);

        SceneManager.LoadScene(hubSceneName);
        Debug.Log("takes player back to the hub world");
    }

    public void ConfirmRestartsLevel()
    {
        Time.timeScale = 1f;

        PlayerInputUtil.ExitUIMode(gameplayMapName);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("takes player back to the hub world");
    }
}
