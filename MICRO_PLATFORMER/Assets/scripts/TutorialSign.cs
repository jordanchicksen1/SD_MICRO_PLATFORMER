using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class TutorialSign : MonoBehaviour, IInteractable
{
    [Header("Instruction UI")]
    [SerializeField] GameObject instructionCanvas;
    [SerializeField] RectTransform instructionPanel;

    [Header("Tutorial Video")]
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip videoClip;

    [Header("Panel Animation")]
    [SerializeField] float popInDuration = 0.2f;
    [SerializeField] float popOutDuration = 0.15f;
    [SerializeField] float popOvershoot = 1.15f;

    [Header("Camera Focus")]
    DoorCameraFocus cameraFocus;
    [SerializeField] Transform focusPoint;

    bool isOpen;
    bool isAnimating;
    Coroutine animationRoutine;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();

        if (cameraFocus == null)
        {
            Debug.LogError(
                "TutorialSign could not find a DoorCameraFocus in the scene.",
                this
            );
        }

        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = false;

            if (videoClip != null)
                videoPlayer.clip = videoClip;
        }
    }

    public void Interact(PlayerController3D player)
    {
        if (isAnimating)
            return;

        if (isOpen)
        {
            CloseInstructions();
        }
        else
        {
            OpenInstructions();
        }
    }

    void OpenInstructions()
    {
        if (isOpen)
            return;

        isOpen = true;
        isAnimating = true;

        SetPlayerInteractPromptSuppressed(true);
        SetPlayerTutorialLock(true);

        if (instructionCanvas != null)
            instructionCanvas.SetActive(true);

        if (instructionPanel != null)
            instructionPanel.localScale = Vector3.zero;

        if (cameraFocus != null && focusPoint != null)
        {
            cameraFocus.FocusUntilClosed(
                focusPoint,
                OnCameraFocusComplete
            );
        }
    }

    void OnCameraFocusComplete()
    {
        if (!isOpen)
            return;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();

            if (videoClip != null)
                videoPlayer.clip = videoClip;

            videoPlayer.Play();
        }

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(PopIn());
    }

    void CloseInstructions()
    {
        if (!isOpen)
            return;

        isOpen = false;
        isAnimating = true;

        SetPlayerInteractPromptSuppressed(false);
        SetPlayerTutorialLock(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (cameraFocus != null)
            cameraFocus.CloseManualFocus();

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(PopOut());
    }

    void SetPlayerInteractPromptSuppressed(bool suppressed)
    {
        CoopCameraController coopCam =
            FindFirstObjectByType<CoopCameraController>();

        if (coopCam == null)
            return;

        foreach (Transform player in coopCam.players)
        {
            if (player == null)
                continue;

            PlayerController3D controller =
                player.GetComponent<PlayerController3D>();

            if (controller != null)
                controller.SetInteractPromptSuppressed(suppressed);
        }
    }

    void SetPlayerTutorialLock(bool locked)
    {
        CoopCameraController coopCam =
            FindFirstObjectByType<CoopCameraController>();

        if (coopCam == null)
            return;

        foreach (Transform player in coopCam.players)
        {
            if (player == null)
                continue;

            PlayerController3D controller =
                player.GetComponent<PlayerController3D>();

            if (controller != null)
                controller.SetTutorialLocked(locked);
        }
    }

    IEnumerator PopIn()
    {
        if (instructionPanel == null)
            yield break;

        float t = 0f;

        Vector3 startScale = Vector3.zero;
        Vector3 overshootScale =
            Vector3.one * popOvershoot;

        Vector3 finalScale = Vector3.one;

        // 0 → overshoot
        while (t < popInDuration * 0.7f)
        {
            t += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    t / (popInDuration * 0.7f)
                );

            progress = 1f - Mathf.Pow(1f - progress, 3f);

            instructionPanel.localScale =
                Vector3.Lerp(
                    startScale,
                    overshootScale,
                    progress
                );

            yield return null;
        }

        t = 0f;

        // overshoot → normal
        while (t < popInDuration * 0.3f)
        {
            t += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    t / (popInDuration * 0.3f)
                );

            progress = 1f - Mathf.Pow(1f - progress, 3f);

            instructionPanel.localScale =
                Vector3.Lerp(
                    overshootScale,
                    finalScale,
                    progress
                );

            yield return null;
        }

        instructionPanel.localScale = finalScale;

        animationRoutine = null;
        isAnimating = false;
    }

    IEnumerator PopOut()
    {
        if (instructionPanel == null)
            yield break;

        float t = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 overshootScale =
            Vector3.one * popOvershoot;

        // normal → slight overshoot
        while (t < popOutDuration * 0.25f)
        {
            t += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    t / (popOutDuration * 0.25f)
                );

            instructionPanel.localScale =
                Vector3.Lerp(
                    startScale,
                    overshootScale,
                    progress
                );

            yield return null;
        }

        t = 0f;

        // overshoot → zero
        while (t < popOutDuration * 0.75f)
        {
            t += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    t / (popOutDuration * 0.75f)
                );

            progress = progress * progress;

            instructionPanel.localScale =
                Vector3.Lerp(
                    overshootScale,
                    Vector3.zero,
                    progress
                );

            yield return null;
        }

        instructionPanel.localScale = Vector3.zero;

        if (instructionCanvas != null)
            instructionCanvas.SetActive(false);

        animationRoutine = null;
        isAnimating = false;
    }
}