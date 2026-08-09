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

    [Header("Title Screen Animation")]
    [SerializeField] TypewriterText titleTypewriter;
    [SerializeField] TypewriterText subtitleTypewriter;
    [SerializeField] string titleText = "Tiny Horizons";
    [SerializeField] string subtitleText = "a co-op platforming adventure";
    [SerializeField] RectTransform startButtonRect;
    [SerializeField] Vector2 startButtonTargetPosition;
    [SerializeField] float startButtonSlideTime = 0.35f;

    [Header("Menu Animation")]
    [SerializeField] RectTransform playButtonRect;
    [SerializeField] RectTransform resetButtonRect;
    [SerializeField] RectTransform quitButtonRect;
    [SerializeField] Vector2 playTargetPos;
    [SerializeField] Vector2 resetTargetPos;
    [SerializeField] Vector2 quitTargetPos;
    [SerializeField] float menuSlideTime = 0.3f;
    [SerializeField] float menuButtonDelay = 0.08f;

    [Header("Menu SFX")]
    [SerializeField] AudioSource uiAudio;
    [SerializeField] AudioClip slideSFX;

    void Awake()
    {
        //set the framerate
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 1;
    }

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

        titleTypewriter.ShowText(titleText);

        yield return new WaitForSeconds(0.35f);

        subtitleTypewriter.ShowText(subtitleText);

        yield return new WaitUntil(() =>
    !titleTypewriter.IsTyping &&
    !subtitleTypewriter.IsTyping);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(SlideInStartButton());

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

        StartCoroutine(MenuOpenRoutine());
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

        CosmeticManager.Instance?.ResetCosmetics();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator PlayRoutine()
    {
        yield return fader.FadeTo(1f, 0.5f);

        SceneManager.LoadScene("HubWorld");
    }

    IEnumerator MenuOpenRoutine()
    {
        yield return StartCoroutine(SlideInMenu());

        EventSystem.current.SetSelectedGameObject(playButton);
    }

    IEnumerator SlideInStartButton()
    {
        if (slideSFX != null)
        {
            uiAudio.PlayOneShot(slideSFX);
        }

        Vector2 startPosition = startButtonRect.anchoredPosition;

        float timer = 0f;

        while (timer < startButtonSlideTime)
        {
            timer += Time.deltaTime;

            float t = timer / startButtonSlideTime;

            // Ease Out Cubic
            t = 1f - Mathf.Pow(1f - t, 3f);

            startButtonRect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    startButtonTargetPosition,
                    t);

            yield return null;
        }

        startButtonRect.anchoredPosition = startButtonTargetPosition;


    }

    IEnumerator SlideUI(
    RectTransform rect,
    Vector2 target)
    {
        if (slideSFX != null)
        {
            uiAudio.PlayOneShot(slideSFX);
        }

        Vector2 start = rect.anchoredPosition;

        float timer = 0f;

        while (timer < menuSlideTime)
        {
            timer += Time.deltaTime;

            float t = timer / menuSlideTime;

            // Ease Out Cubic
            t = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition =
                Vector2.Lerp(
                    start,
                    target,
                    t);

            yield return null;
        }

        rect.anchoredPosition = target;
    }

    IEnumerator SlideInMenu()
    {
        yield return StartCoroutine(
            SlideUI(playButtonRect, playTargetPos));

        yield return new WaitForSeconds(menuButtonDelay);

        yield return StartCoroutine(
            SlideUI(resetButtonRect, resetTargetPos));

        yield return new WaitForSeconds(menuButtonDelay);

        yield return StartCoroutine(
            SlideUI(quitButtonRect, quitTargetPos));
    }

    public void PressQuit()
    {
        Application.Quit();
    }
}