using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject firstSelectedButton;

    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

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

    public void Resume()
    {
        SetPaused(false);
    }

    public void Retry()
    {
        Debug.Log("shows page that asks are you sure you want to restart the level");
    }

    public void QuitLevel()
    {
        Debug.Log("shows page that asks are you sure you want to quit the level");
    }

    public void Controls()
    {
        Debug.Log("show controls");
    }

    public void BackControlsPage()
    {
        Debug.Log("exit controls page");
    }

    public void BackRetryLevel()
    {
        Debug.Log("turn off prompt that shows when you press Retry");
    }

    public void BackQuitLevel()
    {
        Debug.Log("turn off prompt that shows when you press Quit");
    }

    public void ConfirmQuitLevel()
    {
        Debug.Log("takes player back to the hub world");
    }

    public void ConfirmRestartsLevel()
    {
        Debug.Log("takes player back to the hub world");
    }
}
