using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

        // Switch every player to the correct action map
        foreach (var pi in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            if (!pi) continue;
            pi.SwitchCurrentActionMap(paused ? uiMapName : gameplayMapName);
        }

        // Ensure a button is selected for controller navigation
        if (paused && firstSelectedButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    // Hook this to Resume button OnClick
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
