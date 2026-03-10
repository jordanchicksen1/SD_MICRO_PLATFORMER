using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleSceneController : MonoBehaviour
{
    [Header("Intro Images")]
    [SerializeField] CanvasGroup intro1;
    [SerializeField] CanvasGroup intro2;

    [SerializeField] float holdTime = 1.5f;
    [SerializeField] float fadeTime = 1f;

    [Header("UI")]
    [SerializeField] GameObject titleGroup;
    [SerializeField] GameObject menuGroup;

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject playButton;

    [Header("Fade")]
    [SerializeField] ScreenFader fader;

    [SerializeField] GameObject resetConfirmPanel;
    [SerializeField] GameObject resetConfirmFirstButton;
    [SerializeField] GameObject resetDataButton;

    bool menuOpened = false;

    

    void Start()
    {
        StartCoroutine(IntroSequence());
        
            Debug.Log(Application.persistentDataPath);
            
        
    }

    IEnumerator IntroSequence()
    {
        // Image 1 fully visible on top
        intro1.alpha = 1;
        intro2.alpha = 1;

        yield return new WaitForSeconds(holdTime);

        // Fade out image 1 revealing image 2
        yield return FadeOut(intro1);

        yield return new WaitForSeconds(holdTime);

        // Fade out image 2
        yield return FadeOut(intro2);

        // Show title screen
        titleGroup.SetActive(true);

        // Highlight Start button for controller
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    IEnumerator FadeOut(CanvasGroup group)
    {
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            float p = t / fadeTime;

            group.alpha = 1 - p;

            yield return null;
        }

        group.alpha = 0;
    }

    public void PressStart()
    {
        if (menuOpened) return;

        menuOpened = true;

        titleGroup.SetActive(false);
        menuGroup.SetActive(true);

        // Highlight Play button
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void PressPlay()
    {
        StartCoroutine(PlayRoutine());
    }

    public void OpenResetConfirm()
    {
        if (menuGroup) menuGroup.SetActive(false);
        if (resetConfirmPanel) resetConfirmPanel.SetActive(true);

        if (EventSystem.current && resetConfirmFirstButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resetConfirmFirstButton);
        }
    }

    public void CancelReset()
    {
        if (resetConfirmPanel) resetConfirmPanel.SetActive(false);
        if (menuGroup) menuGroup.SetActive(true);

        if (EventSystem.current && resetDataButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resetDataButton);
        }
    }

    public void ConfirmReset()
    {
        Debug.Log("Resetting save data");

        PersistentGemProgress.Instance?.ClearSave();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        
    }

    IEnumerator PlayRoutine()
    {
        yield return fader.FadeTo(1f, 0.5f);

        SceneManager.LoadScene("HubWorld");
    }

    public void PressQuit()
    {
        Application.Quit();
    }
}