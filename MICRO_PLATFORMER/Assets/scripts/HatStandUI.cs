using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HatStandUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] Button exitButton;

    [Header("Camera")]
    [SerializeField] HubCameraFocus cameraFocus;
    [SerializeField] HubCameraFollow cameraFollow;

    [Header("Presentation")]
    [SerializeField] HatStandController standController;

    Vector3 returnPos;
    Quaternion returnRot;

    HubPlayerController3D lockedPlayer;
    Rigidbody lockedRb;
    HubFollower follower;

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

        panelRoot.SetActive(true);

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

        if (standController && follower)
            standController.BeginPresentation(player, follower);

        if (EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        }
    }

    public void Close()
    {
        panelRoot.SetActive(false);

        UnlockPlayer();

        if (standController)
            standController.EndPresentation();

        StartCoroutine(ReturnCamera());

        isOpen = false;
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

    IEnumerator ReturnCamera()
    {
        cameraFocus.ReturnTo(returnPos, returnRot);

        while (cameraFocus.IsMoving)
            yield return null;

        cameraFollow.enabled = true;
    }
}