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

    // NEW: if true, pause cannot open and PauseManager won't switch maps
    public bool PauseLocked { get; private set; }

    void Start()
    {
        SetPaused(false);
    }

    // NEW: call this from Results UI (or any modal UI)
    public void SetPauseLocked(bool locked)
    {
        PauseLocked = locked;

        // If we lock while paused, force unpause + hide pause UI
        if (PauseLocked && IsPaused)
            SetPaused(false);
    }

    public void TogglePause()
    {
        if (PauseLocked) return; // NEW: ignore pause input while locked
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool paused)
    {
        if (PauseLocked && paused) return; // NEW: can't open pause while locked

        IsPaused = paused;

        if (pauseCanvas) pauseCanvas.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        // IMPORTANT: only switch maps if NOT locked
        if (!PauseLocked)
        {
            foreach (var pi in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                if (!pi) continue;

                // Optional: only affect actual players, not UI-only PlayerInputs
                // if (!pi.CompareTag("Player")) continue;

                if (PauseLocked) continue;

                var targetMap = paused ? uiMapName : gameplayMapName;
                PlayerInputUtil.SafeSwitchMap(pi, targetMap);
            }
        }

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
}
