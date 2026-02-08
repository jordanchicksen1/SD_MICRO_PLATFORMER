using UnityEngine;
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
    //[SerializeField] HubPlayerController3D playerControllerToDisable; // drag HubPlayerController3D component here (P1)
    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";
    Rigidbody lockedRb;
    HubPlayerController3D lockedController;
    bool isOpen;
    public bool IsOpen => isOpen;
    RigidbodyConstraints savedConstraints;
    bool savedConstraintsValid;





    string pendingSceneName;
    Transform pendingFocusPoint;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [SerializeField] HubCameraFollow cameraFollowToDisable;
  
    

   
   



    void Awake()
    {
        if (panelRoot) panelRoot.SetActive(false);

        if (!cameraFocus)
            cameraFocus = Camera.main ? Camera.main.GetComponent<HubCameraFocus>() : null;

        playButton.onClick.AddListener(OnPlayPressed);
        cancelButton.onClick.AddListener(OnCancelPressed);
    }

    public void Open(
    string sceneName,
    VideoClip previewClip,
    string title,
    string description,
    Transform focusPoint,
    HubPlayerController3D playerController,
    Rigidbody playerRb)
    {
        pendingSceneName = sceneName;
        pendingFocusPoint = focusPoint;

        if (panelRoot) panelRoot.SetActive(true);
        if (EventSystem.current != null && playButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        if (isOpen) return;
        isOpen = true;

        LockPlayer(playerRb, playerController);




        // if (playerControllerToDisable)playerControllerToDisable.enabled = false;


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
        if (cameraFollowToDisable) cameraFollowToDisable.enabled = false;
        

    }


    void OnPlayPressed()
    {
        // Optionally: small delay / fade can be added later
        if (!string.IsNullOrEmpty(pendingSceneName))
            SceneManager.LoadScene(pendingSceneName);
        Close();
    }

    void OnCancelPressed()
    {
        Close();
    }

    public void Close()
    {
        if (videoPlayer) videoPlayer.Stop();

        if (panelRoot) panelRoot.SetActive(false);

        if (cameraFocus) cameraFocus.ReturnToDefault();

        // Re-enable player control

        //if (playerControllerToDisable) playerControllerToDisable.enabled = true;

        UnlockPlayer();
        isOpen = false;
        



    pendingSceneName = null;
        pendingFocusPoint = null;
        if (cameraFollowToDisable) cameraFollowToDisable.enabled = true;
        

    }

    void LockPlayer(Rigidbody rb, HubPlayerController3D controller)
    {
        lockedRb = rb;
        lockedController = controller;

        if (lockedController) lockedController.enabled = false;

        if (lockedRb)
        {
            // SAVE ORIGINAL CONSTRAINTS ONCE
            savedConstraints = lockedRb.constraints;
            savedConstraintsValid = true;

            lockedRb.linearVelocity = Vector3.zero;
            lockedRb.angularVelocity = Vector3.zero;

            // lock completely while UI is open
            lockedRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }


    void UnlockPlayer()
    {
        if (lockedRb)
        {
            lockedRb.linearVelocity = Vector3.zero;
            lockedRb.angularVelocity = Vector3.zero;

            // RESTORE ORIGINAL CONSTRAINTS
            if (savedConstraintsValid)
                lockedRb.constraints = savedConstraints;
        }

        if (lockedController) lockedController.enabled = true;

        lockedRb = null;
        lockedController = null;
        savedConstraintsValid = false;
    }


}
