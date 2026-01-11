using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealthUIManager : MonoBehaviour
{
    [SerializeField] PlayerHealthUI p1UI;
    [SerializeField] PlayerHealthUI p2UI;

    PlayerHealth p1Health;
    PlayerHealth p2Health;

    public void RegisterPlayer(PlayerHealth health)
    {
        int index = health.GetComponent<PlayerInput>().playerIndex;

        if (index == 0)
            p1Health = health;
        else if (index == 1)
            p2Health = health;

        TryBind();
    }

    void TryBind()
    {
        if (p1Health != null && p2Health != null)
        {
            p1UI.Bind(p1Health);
            p2UI.Bind(p2Health);
        }
    }
}
