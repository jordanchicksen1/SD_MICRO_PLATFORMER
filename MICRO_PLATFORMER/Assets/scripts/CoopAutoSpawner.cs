using UnityEngine;
using UnityEngine.InputSystem;

public class CoopAutoSpawner : MonoBehaviour
{
    PlayerInputManager manager;

    void Awake()
    {
        manager = GetComponent<PlayerInputManager>();
    }

    void Start()
    {
        var gamepads = Gamepad.all;

        // Spawn up to 2 players automatically
        for (int i = 0; i < Mathf.Min(2, gamepads.Count); i++)
        {
            PlayerInput.Instantiate(
                manager.playerPrefab,
                controlScheme: null,
                pairWithDevice: gamepads[i]
            );
        }
    }
}