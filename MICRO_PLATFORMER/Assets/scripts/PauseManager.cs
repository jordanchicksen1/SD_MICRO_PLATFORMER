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
        Debug.Log("Restart the level that you are currently on");
    }

    public void QuitLevel()
    {
        Debug.Log("quit level and went back to hub world");
    }

    public void Controls()
    {
        Debug.Log("show controls");
    }
}
