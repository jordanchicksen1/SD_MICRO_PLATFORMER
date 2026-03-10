using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HubPauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject firstSelectedButton;

    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    [SerializeField] GameObject mainPausePanel;
    [SerializeField] GameObject quitConfirmPanel;

    [SerializeField] GameObject quitConfirmFirstButton;

    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject controlsFirstButton;

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

        if (pauseCanvas)
            pauseCanvas.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;

        if (paused)
            PlayerInputUtil.EnterUIMode(uiMapName);
        else
            StartCoroutine(ReturnGameplayNextFrame());

        if (paused && EventSystem.current && firstSelectedButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    IEnumerator ReturnGameplayNextFrame()
    {
        yield return null; // wait one frame so the UI submit doesn't leak through
        PlayerInputUtil.ExitUIMode(gameplayMapName);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void Quit()
    {
        if (mainPausePanel) mainPausePanel.SetActive(false);
        if (quitConfirmPanel) quitConfirmPanel.SetActive(true);

        if (EventSystem.current && quitConfirmFirstButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(quitConfirmFirstButton);
        }
    }

    public void CancelQuit()
    {
        if (quitConfirmPanel) quitConfirmPanel.SetActive(false);
        if (mainPausePanel) mainPausePanel.SetActive(true);

        if (EventSystem.current && firstSelectedButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void ConfirmQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void OpenControls()
    {
        if (mainPausePanel) mainPausePanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(true);

        if (EventSystem.current && controlsFirstButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(controlsFirstButton);
        }
    }

    public void BackFromControls()
    {
        if (controlsPanel) controlsPanel.SetActive(false);
        if (mainPausePanel) mainPausePanel.SetActive(true);

        if (EventSystem.current && firstSelectedButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}