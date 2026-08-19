using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HubProgressionGateUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] UIPanelSlide panelSlide;
    [SerializeField] Button unlockButton;
    [SerializeField] Button cancelButton;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI gemCostText;

    [Header("Camera")]
    [SerializeField] HubCameraFocus cameraFocus;
    [SerializeField] HubCameraFollow cameraFollowToDisable;

    [Header("Input")]
    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    [Header("Transport")]
    [SerializeField] HubIslandPipe destinationPipe;

    Rigidbody lockedRb;
    HubPlayerController3D lockedController;

    RigidbodyConstraints savedConstraints;
    bool savedConstraintsValid;

    Vector3 returnPos;
    Quaternion returnRot;

    bool isOpen;
    HubPlayerController3D currentPlayer;
    int currentGemCost;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (cameraFocus == null)
        {
            cameraFocus = Camera.main
                ? Camera.main.GetComponent<HubCameraFocus>()
                : null;
        }

        if (unlockButton != null)
            unlockButton.onClick.AddListener(OnUnlockPressed);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Close);
    }

    public void Open(string title, string description, int cost, Transform focusPoint, HubPlayerController3D player)
    {

        if (isOpen)
            return;

        isOpen = true;
        currentPlayer = player;
        currentGemCost = cost;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (gemCostText != null)
            gemCostText.text = $"{cost}";

        // -------------------------
        // SHOW UI
        // -------------------------

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (panelSlide != null)
            panelSlide.SlideIn();

        // -------------------------
        // SWITCH TO UI INPUT
        // -------------------------

        PlayerInputUtil.EnterUIMode(uiMapName);

        // -------------------------
        // SELECT FIRST BUTTON
        // -------------------------

        if (EventSystem.current != null &&
            unlockButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(
                unlockButton.gameObject
            );

            EventSystem.current.sendNavigationEvents = true;
        }

        // -------------------------
        // LOCK PLAYER
        // -------------------------

        if (player != null)
        {
            Rigidbody playerRb =
                player.GetComponent<Rigidbody>();

            LockPlayer(playerRb, player);

            HubPlayerAnimator hubAnimator =
                player.GetComponentInChildren<HubPlayerAnimator>();

            if (hubAnimator != null)
                hubAnimator.SetMoveBlend(0f);
        }

        // -------------------------
        // SAVE CAMERA POSITION
        // -------------------------

        if (Camera.main != null)
        {
            returnPos = Camera.main.transform.position;
            returnRot = Camera.main.transform.rotation;
        }

        // -------------------------
        // STOP CAMERA FOLLOW
        // -------------------------

        if (cameraFollowToDisable != null)
            cameraFollowToDisable.enabled = false;

        // -------------------------
        // FOCUS CAMERA
        // -------------------------

        if (cameraFocus != null &&
            focusPoint != null)
        {
            cameraFocus.FocusOn(focusPoint);
        }
    }

    void LockPlayer(
    Rigidbody rb,
    HubPlayerController3D controller
)
    {
        lockedRb = rb;
        lockedController = controller;

        if (lockedController)
            lockedController.enabled = false;

        if (lockedRb)
        {
            savedConstraints = lockedRb.constraints;
            savedConstraintsValid = true;

            lockedRb.linearVelocity = Vector3.zero;
            lockedRb.angularVelocity = Vector3.zero;

            lockedRb.constraints =
                RigidbodyConstraints.FreezePosition |
                RigidbodyConstraints.FreezeRotation;
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

        if (lockedController)
            lockedController.enabled = true;

        lockedRb = null;
        lockedController = null;
        savedConstraintsValid = false;
    }

    void OnUnlockPressed()
    {
        if (!isOpen)
            return;

        if (CurrencyManager.Instance == null)
            return;

        bool spent =
            CurrencyManager.Instance.SpendGems(currentGemCost);

        if (!spent)
        {
            Debug.Log("Not enough gems to unlock this island.");
            return;
        }

        if (HubProgressionManager.Instance != null)
        {
            HubProgressionManager.Instance.UnlockIsland2();
        }

        if (destinationPipe != null)
        {
            destinationPipe.StartTransport(currentPlayer);
        }
    }

    public void Close()
    {
        if (!isOpen)
            return;

        if (panelRoot != null)
            panelRoot.SetActive(false);

        PlayerInputUtil.ExitUIMode(gameplayMapName);

        UnlockPlayer();

        isOpen = false;

        StartCoroutine(ReturnThenEnableFollow());
    }

    public void CloseForTransport()
    {
        if (!isOpen)
            return;

        if (panelRoot != null)
            panelRoot.SetActive(false);

        PlayerInputUtil.ExitUIMode(gameplayMapName);

        isOpen = false;
    }

    IEnumerator ReturnThenEnableFollow()
    {
        if (cameraFocus != null)
            cameraFocus.ReturnTo(
                returnPos,
                returnRot
            );

        while (
            cameraFocus != null &&
            cameraFocus.IsMoving
        )
        {
            yield return null;
        }

        if (cameraFollowToDisable != null)
            cameraFollowToDisable.enabled = true;
    }
}