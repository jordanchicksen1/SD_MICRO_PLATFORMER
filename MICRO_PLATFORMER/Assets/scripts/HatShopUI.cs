using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum ShopSpeechType
{
    Greeting,
    Purchase,
    NoMoney
}

public class HatShopUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] Button firstButton;
    [SerializeField] Button exitButton;
    [SerializeField] UIPanelSlide panelSlide;

    [Header("Speech Bubble")]
    [SerializeField] GameObject speechBubble;
    [SerializeField] TMPro.TextMeshProUGUI speechText;
    [SerializeField] TypewriterText typewriter;
    [SerializeField] float speechDuration = 2f;
    [SerializeField] ShopkeeperAnimation shopkeeper;
    ShopSpeechType currentSpeechType;

    [Header("Dialogue")]

    [SerializeField]
    string[] greetingLines =
{
    "What are you getting today?",
    "Take your time!",
    "Looking for a new hat?",
    "Welcome back!"
};

    [SerializeField]
    string[] purchaseLines =
    {
    "Great choice!",
    "Looking stylish!",
    "Excellent!",
    "Wear it proudly!"
};

    [SerializeField]
    string[] noMoneyLines =
    {
    "You'll need more coins!",
    "Come back after another adventure!",
    "You're a little short.",
    "Keep exploring!"
};

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
    Coroutine speechRoutine;

    void Awake()
    {
        if (panelRoot)
            panelRoot.SetActive(false);

        if (speechBubble)
            speechBubble.SetActive(false);

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

        panelSlide.SlideIn();

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
            StartCoroutine(ShowGreetingAfterCamera());
        }
    }

    public void Close()
    {
        if (panelRoot)
            StartCoroutine(CloseRoutine());

        StartCoroutine(ReturnThenEnableFollow());

        UnlockPlayer();

        isOpen = false;
    }

    System.Collections.IEnumerator CloseRoutine()
    {
        panelSlide.SlideOut();

        yield return new WaitForSecondsRealtime(panelSlide.Duration);

        panelRoot.SetActive(false);
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

    public void ShowSpeech(ShopSpeechType type)
    {
        string[] lines = null;

        switch (type)
        {
            case ShopSpeechType.Greeting:
                lines = greetingLines;
                break;

            case ShopSpeechType.Purchase:
                lines = purchaseLines;
                break;

            case ShopSpeechType.NoMoney:
                lines = noMoneyLines;
                break;
        }

        if (lines == null || lines.Length == 0)
            return;

        string chosenLine = lines[Random.Range(0, lines.Length)];

        if (speechRoutine != null)
            StopCoroutine(speechRoutine);

        currentSpeechType = type;
        speechRoutine = StartCoroutine(SpeechRoutine(chosenLine));
    }

    System.Collections.IEnumerator ShowGreetingAfterCamera()
    {
        while (cameraFocus != null && cameraFocus.IsMoving)
            yield return null;

        ShowSpeech(ShopSpeechType.Greeting);
    }

    System.Collections.IEnumerator SpeechRoutine(string message)
    {
        if (!speechBubble || !speechText)
            yield break;

        switch (currentSpeechType)
        {
            case ShopSpeechType.Greeting:
                if (shopkeeper)
                    shopkeeper.StartTalking();
                break;

            case ShopSpeechType.Purchase:
                if (shopkeeper)
                    shopkeeper.StartCelebrate();
                break;

            case ShopSpeechType.NoMoney:
                if (shopkeeper)
                    shopkeeper.StartHeadShake();
                break;
        }

        speechBubble.SetActive(true);
        typewriter.ShowText(message);

        // Start tiny
        speechBubble.transform.localScale = Vector3.zero;

        // Pop in
        float timer = 0f;

        while (timer < 0.15f)
        {
            timer += Time.unscaledDeltaTime;

            speechBubble.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    Vector3.one,
                    timer / 0.15f);

            yield return null;
        }

        // Hold
        yield return new WaitForSecondsRealtime(speechDuration);

        // Pop out
        timer = 0f;

        while (timer < 0.15f)
        {
            timer += Time.unscaledDeltaTime;

            speechBubble.transform.localScale =
                Vector3.Lerp(
                    Vector3.one,
                    Vector3.zero,
                    timer / 0.15f);

            yield return null;
        }

        speechBubble.SetActive(false);
        if (shopkeeper)
            shopkeeper.StopAnimation();

        speechRoutine = null;

        speechRoutine = null;
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