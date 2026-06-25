using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HatShopUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] Button firstButton;
    [SerializeField] Button exitButton;

    [Header("Dependencies")]
    [SerializeField] HubCameraFocus cameraFocus;
    [SerializeField] HubCameraFollow cameraFollowToDisable;

    Rigidbody lockedRb;
    HubPlayerController3D lockedController;

    RigidbodyConstraints savedConstraints;
    bool savedConstraintsValid;

    Vector3 returnPos;
    Quaternion returnRot;

    bool isOpen;
    public bool IsOpen => isOpen;

    void Awake()
    {
        if (panelRoot)
            panelRoot.SetActive(false);

        if (!cameraFocus)
            cameraFocus = Camera.main ?
                Camera.main.GetComponent<HubCameraFocus>() : null;

        if (exitButton)
            exitButton.onClick.AddListener(Close);
    }

    public void Open(
        Transform focusPoint,
        HubPlayerController3D playerController,
        Rigidbody playerRb
    )
    {
        if (isOpen)
            return;

        isOpen = true;

        if (panelRoot)
            panelRoot.SetActive(true);

        LockPlayer(playerRb, playerController);

        if (Camera.main != null)
        {
            returnPos = Camera.main.transform.position;
            returnRot = Camera.main.transform.rotation;
        }

        if (cameraFollowToDisable)
            cameraFollowToDisable.enabled = false;

        if (cameraFocus && focusPoint)
            cameraFocus.FocusOn(focusPoint);

        if (EventSystem.current && firstButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    public void Close()
    {
        if (panelRoot)
            panelRoot.SetActive(false);

        StartCoroutine(ReturnThenEnableFollow());

        UnlockPlayer();

        isOpen = false;
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

    public void SelectExitButton()
    {
        if (EventSystem.current && exitButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        }
    }

    System.Collections.IEnumerator ReturnThenEnableFollow()
    {
        if (cameraFocus)
            cameraFocus.ReturnTo(returnPos, returnRot);

        while (cameraFocus != null && cameraFocus.IsMoving)
            yield return null;

        if (cameraFollowToDisable)
            cameraFollowToDisable.enabled = true;
    }
}