using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CoopLifeManager : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] ReviveBubble bubblePrefab;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] float restartDelay = 5f;

    readonly Dictionary<int, PlayerBubbleState> bubbleByIndex = new();
    readonly Dictionary<int, PlayerHealth> healthByIndex = new();
    readonly Dictionary<int, ReviveBubble> bubbleObjByIndex = new();

    bool gameOver;

    CoopCameraController cam;

    void Awake()
    {
        cam = FindFirstObjectByType<CoopCameraController>();
        if (gameOverScreen) gameOverScreen.SetActive(false);
    }

    public void RegisterPlayer(PlayerBubbleState bubbleState, PlayerHealth health, PlayerInput input)
    {
        if (bubbleState == null) { Debug.LogError("RegisterPlayer: bubbleState is null (missing PlayerBubbleState)"); return; }
        if (health == null) { Debug.LogError("RegisterPlayer: health is null (missing PlayerHealth)"); return; }
        if (input == null) { Debug.LogError("RegisterPlayer: input is null (missing PlayerInput)"); return; }

        int idx = input.playerIndex;

        bubbleByIndex[idx] = bubbleState;
        healthByIndex[idx] = health;

        Debug.Log($"CoopLifeManager registered player idx={idx} name={input.name}");

        // Prevent double-subscribe if RegisterPlayer runs twice
        health.OnDied -= () => OnPlayerDied(idx); // (can't remove lambdas reliably)
        // So we do it safely like this:
        health.OnDied += () => OnPlayerDied(idx);

        // sanity
        if (bubblePrefab == null)
            Debug.LogError("CoopLifeManager: bubblePrefab is NOT assigned in Inspector!");
    }

    void OnPlayerDied(int deadIndex)
    {
        Debug.Log($"OnPlayerDied received for idx={deadIndex}");

        if (gameOver) return;

        int otherIndex = (deadIndex == 0) ? 1 : 0;

        if (!bubbleByIndex.ContainsKey(otherIndex))
        {
            Debug.LogError("Other player not registered -> GAME OVER");
            TriggerGameOver();
            return;
        }

        if (bubbleByIndex[otherIndex].IsBubbled)
        {
            Debug.Log("Other player already bubbled -> GAME OVER");
            TriggerGameOver();
            return;
        }

        BubblePlayer(deadIndex, otherIndex);
    }

    void BubblePlayer(int deadIndex, int aliveIndex)
    {
        bubbleByIndex[deadIndex].EnterBubble();

        if (bubblePrefab == null)
        {
            Debug.LogError("No bubblePrefab assigned on CoopLifeManager!");
            return;
        }

        // destroy old bubble if any
        if (bubbleObjByIndex.TryGetValue(deadIndex, out var oldBubble) && oldBubble != null)
            Destroy(oldBubble.gameObject);

        var bubble = Instantiate(bubblePrefab);
        bubble.Init(this, bubbleByIndex[deadIndex], bubbleByIndex[aliveIndex].transform);
        bubbleObjByIndex[deadIndex] = bubble;

        // OPTION B: camera tracks bubble instead of dead player
        if (cam != null)
            cam.ReplaceTarget(bubbleByIndex[deadIndex].transform, bubble.transform);
    }

    public void PopBubble(int reviveIndex)
    {
        if (gameOver) return;
        if (!bubbleByIndex.ContainsKey(reviveIndex)) return;

        // cache bubble position BEFORE destroying it
        Vector3 bubblePos = bubbleObjByIndex.TryGetValue(reviveIndex, out var bub) && bub != null
            ? bub.transform.position
            : bubbleByIndex[reviveIndex].transform.position;

        // camera swap back (option B)
        if (cam != null && bub != null)
            cam.ReplaceTarget(bub.transform, bubbleByIndex[reviveIndex].transform);

        // destroy bubble
        if (bub != null) Destroy(bub.gameObject);
        bubbleObjByIndex[reviveIndex] = null;

        // respawn at bubble position (slightly down so you don't pop into the air)
        Vector3 respawn = bubblePos + Vector3.down * 1.2f;

        bubbleByIndex[reviveIndex].ExitBubble(respawnPos: respawn, reviveHp: 3);
    }

    void TriggerGameOver()
    {
        gameOver = true;
        if (gameOverScreen) gameOverScreen.SetActive(true);
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
