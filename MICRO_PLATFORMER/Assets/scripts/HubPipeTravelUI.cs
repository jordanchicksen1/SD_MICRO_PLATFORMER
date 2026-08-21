using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HubPipeTravelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] TextMeshProUGUI destinationText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    [Header("Input")]
    [SerializeField] string gameplayMapName = "Gameplay";
    [SerializeField] string uiMapName = "UI";

    bool isOpen;

    HubIslandPipe currentPipe;
    HubPlayerController3D currentPlayer;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (yesButton != null)
            yesButton.onClick.AddListener(ConfirmTravel);

        if (noButton != null)
            noButton.onClick.AddListener(CancelTravel);
    }

    public void Open(
        HubIslandPipe pipe,
        HubPlayerController3D player
    )
    {
        if (isOpen)
            return;

        if (pipe == null)
            return;

        if (player == null)
            return;

        currentPipe = pipe;
        currentPlayer = player;

        isOpen = true;

        if (destinationText != null)
        {
            destinationText.text =
                $"Do you want to travel to " +
                $"{pipe.DestinationIslandName} Island?";
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);

        PlayerInputUtil.EnterUIMode(uiMapName);

        if (EventSystem.current != null &&
            yesButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(
                yesButton.gameObject
            );

            EventSystem.current.sendNavigationEvents = true;
        }
    }

    void ConfirmTravel()
    {
        if (!isOpen)
            return;

        if (currentPipe == null)
            return;

        if (currentPlayer == null)
            return;

        HubIslandPipe pipe = currentPipe;
        HubPlayerController3D player = currentPlayer;

        Close();

        pipe.StartFastTravel(player);
    }

    void CancelTravel()
    {
        Close();
    }

    public void Close()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        PlayerInputUtil.ExitUIMode(
            gameplayMapName
        );

        isOpen = false;

        currentPipe = null;
        currentPlayer = null;
    }
}