using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class HubLevelSelectUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;   // the whole panel
    [SerializeField] Button playButton;
    [SerializeField] Button cancelButton;

    [Header("Video")]
    [SerializeField] VideoPlayer videoPlayer;

    [Header("Dependencies")]
    [SerializeField] HubCameraFocus cameraFocus;
    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    Rigidbody lockedRb;
    HubPlayerController3D lockedController;
    bool isOpen;
    public bool IsOpen => isOpen;

    RigidbodyConstraints savedConstraints;
    bool savedConstraintsValid;

    Vector3 returnPos;
    Quaternion returnRot;

    string pendingSceneName;
    Transform pendingFocusPoint;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [Header("Gem Progress (finite gems)")]
    [SerializeField] TextMeshProUGUI levelGemsProgressText; // <-- add to your panel (e.g. "2 / 5")

    [SerializeField] HubCameraFollow cameraFollowToDisable;

    [SerializeField] ScreenFader fader;
    [SerializeField] float fadeOutDuration = 0.4f;
    bool isLoading;

    void Awake()
    {
        if (panelRoot) panelRoot.SetActive(false);

        if (!cameraFocus)
            cameraFocus = Camera.main ? Camera.main.GetComponent<HubCameraFocus>() : null;

        if (playButton) playButton.onClick.AddListener(OnPlayPressed);
        if (cancelButton) cancelButton.onClick.AddListener(OnCancelPressed);

        if (!fader) fader = FindFirstObjectByType<ScreenFader>();
    }

    public void Open(
        string sceneName,
        VideoClip previewClip,
        string title,
        string description,
        Transform focusPoint,
        HubPlayerController3D playerController,
        Rigidbody playerRb,

        // NEW: finite-gems progress
        string levelId,
        int totalGemsInLevel
    )
    {
        if (isOpen) return;
        isOpen = true;

        pendingSceneName = sceneName;
        pendingFocusPoint = focusPoint;

        if (panelRoot) panelRoot.SetActive(true);

        // Select button for controller navigation
        if (EventSystem.current != null && playButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        LockPlayer(playerRb, playerController);

        // Save current camera pose to return to
        if (Camera.main != null)
        {
            returnPos = Camera.main.transform.position;
            returnRot = Camera.main.transform.rotation;
        }

        if (cameraFollowToDisable) cameraFollowToDisable.enabled = false;

        if (cameraFocus && focusPoint)
            cameraFocus.FocusOn(focusPoint);

        if (videoPlayer)
        {
            videoPlayer.Stop();
            videoPlayer.clip = previewClip;
            if (previewClip != null)
                videoPlayer.Play();
        }

        if (titleText) titleText.text = title;
        if (descriptionText) descriptionText.text = description;

        // -------- NEW: show gems collected for this level --------
        if (levelGemsProgressText)
        {
            Debug.Log($"[HubLevelSelectUI] levelId='{levelId}', total={totalGemsInLevel}, progressMgr={(PersistentGemProgress.Instance ? "OK" : "NULL")}");

            int collected = 0;

            if (PersistentGemProgress.Instance != null && !string.IsNullOrEmpty(levelId))
                collected = PersistentGemProgress.Instance.GetCollectedCount(levelId);

            // clamp just in case you change totals later
            collected = Mathf.Clamp(collected, 0, Mathf.Max(0, totalGemsInLevel));

            levelGemsProgressText.text = $"{collected} / {totalGemsInLevel}";
            Debug.Log($"[HubLevelSelectUI] collected={collected}");

        }
        // --------------------------------------------------------
    }

    public void OnPlayPressed()
    {
        if (isLoading) return;
        if (string.IsNullOrEmpty(pendingSceneName)) return;

        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        if (panelRoot) panelRoot.SetActive(false);
        isLoading = true;

        if (playButton) playButton.interactable = false;
        if (cancelButton) cancelButton.interactable = false;

        if (fader != null)
            yield return fader.FadeTo(1f, fadeOutDuration);

        SceneManager.LoadScene(pendingSceneName);
    }

    void OnCancelPressed()
    {
        Close();
    }

    public void Close()
    {
        if (videoPlayer) videoPlayer.Stop();

        if (panelRoot) panelRoot.SetActive(false);

        StartCoroutine(ReturnThenEnableFollow());

        UnlockPlayer();
        isOpen = false;

        pendingSceneName = null;
        pendingFocusPoint = null;

        // Reset buttons for next open
        if (playButton) playButton.interactable = true;
        if (cancelButton) cancelButton.interactable = true;

        isLoading = false;
    }

    void LockPlayer(Rigidbody rb, HubPlayerController3D controller)
    {
        lockedRb = rb;
        lockedController = controller;

        if (lockedController) lockedController.enabled = false;

        if (lockedRb)
        {
            savedConstraints = lockedRb.constraints;
            savedConstraintsValid = true;

            lockedRb.linearVelocity = Vector3.zero;
            lockedRb.angularVelocity = Vector3.zero;

            lockedRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }

    void UnlockPlayer()
    {
        if (lockedRb)
        {
            lockedRb.linearVelocity = Vector3.zero;
            lockedRb.angularVelocity = Vector3.zero;

            if (savedConstraintsValid)
                lockedRb.constraints = savedConstraints;
        }

        if (lockedController) lockedController.enabled = true;

        lockedRb = null;
        lockedController = null;
        savedConstraintsValid = false;
    }

    IEnumerator ReturnThenEnableFollow()
    {
        if (cameraFocus)
            cameraFocus.ReturnTo(returnPos, returnRot);

        while (cameraFocus != null && cameraFocus.IsMoving)
            yield return null;

        if (cameraFollowToDisable) cameraFollowToDisable.enabled = true;
    }
}
