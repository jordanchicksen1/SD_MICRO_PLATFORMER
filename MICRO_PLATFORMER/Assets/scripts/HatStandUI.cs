using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HatStandUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] Button exitButton;
    [SerializeField] HatStandItem[] hatItems;

    [Header("Camera")]
    [SerializeField] HubCameraFocus cameraFocus;
    [SerializeField] HubCameraFollow cameraFollow;

    [Header("Presentation")]
    [SerializeField] HatStandPresentation presentation;

    Vector3 returnPos;
    Quaternion returnRot;

    HubPlayerController3D lockedPlayer;
    Rigidbody lockedRb;
    HubFollower follower;
    [SerializeField] UIPanelSlide panelSlide;

    bool isOpen;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (panelRoot)
            panelRoot.SetActive(false);
    }

    public void Open(
        Transform focusPoint,
        HubPlayerController3D player,
        Rigidbody rb)
    {
        if (isOpen)
            return;

        isOpen = true;

        RefreshPurchasedItems();
        RefreshAllItems();

        panelRoot.SetActive(true);

        panelSlide.SlideIn();

        lockedPlayer = player;
        lockedRb = rb;

        follower = FindFirstObjectByType<HubFollower>();

        LockPlayer();

        returnPos = Camera.main.transform.position;
        returnRot = Camera.main.transform.rotation;

        if (cameraFollow)
            cameraFollow.enabled = false;

        if (cameraFocus)
            cameraFocus.FocusOn(focusPoint);

        if (presentation)
            presentation.BeginPresentation();

        if (EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        }
    }

    public void Close()
    {
        StartCoroutine(CloseRoutine());

        UnlockPlayer();
        StartCoroutine(ReturnCamera());
        if (presentation)
            presentation.EndPresentation();

        HubHatVisual[] visuals =
    FindObjectsByType<HubHatVisual>(
        FindObjectsSortMode.None);

        foreach (HubHatVisual visual in visuals)
        {
            visual.RefreshHat();
        }

        isOpen = false;
    }

    IEnumerator CloseRoutine()
    {
        panelSlide.SlideOut();

        yield return new WaitForSecondsRealtime(panelSlide.Duration);

        panelRoot.SetActive(false);
    }

    void LockPlayer()
    {
        lockedPlayer.enabled = false;

        lockedRb.linearVelocity = Vector3.zero;
        lockedRb.angularVelocity = Vector3.zero;

        lockedRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotation;
    }

    void UnlockPlayer()
    {
        lockedRb.constraints =
            RigidbodyConstraints.FreezeRotation;

        lockedPlayer.enabled = true;
    }

    public void RefreshAllItems()
    {
        foreach (HatStandItem item in hatItems)
        {
            item.Refresh();
        }
    }

    public void RefreshPurchasedItems()
    {
        foreach (HatStandItem item in hatItems)
        {
            item.RefreshPurchasedState();
        }
    }

    IEnumerator ReturnCamera()
    {
        cameraFocus.ReturnTo(returnPos, returnRot);

        while (cameraFocus.IsMoving)
            yield return null;

        cameraFollow.enabled = true;
    }
}