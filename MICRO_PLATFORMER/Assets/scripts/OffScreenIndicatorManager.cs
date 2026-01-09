using UnityEngine;
using UnityEngine.InputSystem;

public class OffScreenIndicatorManager : MonoBehaviour
{
    [Header("Indicators (Scene Objects)")]
    [SerializeField] offScreenIndicator p1Indicator;
    [SerializeField] offScreenIndicator p2Indicator;

    PlayerController3D p1;
    PlayerController3D p2;

    public void RegisterPlayer(PlayerController3D player)
    {
        int index = player.GetComponent<PlayerInput>().playerIndex;

        if (index == 0)
            p1 = player;
        else if (index == 1)
            p2 = player;

        TryAssignIndicators();
    }

    void TryAssignIndicators()
    {
        if (p1 == null || p2 == null)
            return;

        // Player 1 sees Player 2
        p1Indicator.SetTrackedPlayer(p2.transform);

        // Player 2 sees Player 1
        p2Indicator.SetTrackedPlayer(p1.transform);
    }
}
