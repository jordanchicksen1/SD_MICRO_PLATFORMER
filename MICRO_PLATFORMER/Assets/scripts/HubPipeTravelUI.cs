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
    bool travelInProgress;
    HubIslandPipe currentPipe;
    HubPlayerController3D currentPlayer;
    int currentDestinationIslandID;
    string currentDestinationIslandName;

    public bool IsOpen => isOpen;

    public void BeginTravel()
    {
        travelInProgress = true;
    }

    public void EndTravel()
    {
        travelInProgress = false;
    }

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
     int destinationIslandID,
     string destinationIslandName,
     HubPlayerController3D player
 )
    {
        if (travelInProgress)
            return;

        if (isOpen)
            return;

        if (pipe == null)
            return;

        if (player == null)
            return;

        currentPipe = pipe;
        currentPlayer = player;

        currentDestinationIslandID =
            destinationIslandID;

        currentDestinationIslandName =
            destinationIslandName;

        isOpen = true;

        if (destinationText != null)
        {
            destinationText.text =
                $"Do you want to travel to " +
                $"{currentDestinationIslandName}?";
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

        HubIslandPipe pipe = currentPipe;
        HubPlayerController3D player = currentPlayer;

        int destinationIslandID =
            currentDestinationIslandID;

        // Hide the travel UI immediately.
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        // Return control to gameplay.
        PlayerInputUtil.ExitUIMode(
            gameplayMapName
        );

        // Mark the UI as closed.
        isOpen = false;

        // Clear the stored UI references.
        currentPipe = null;
        currentPlayer = null;

        if (pipe == null)
            return;

        if (player == null)
            return;

        // Start fast travel using the destination
        // supplied by the interaction that opened the UI.
        pipe.StartFastTravel(
    player,
    destinationIslandID,
    currentDestinationIslandName
);
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