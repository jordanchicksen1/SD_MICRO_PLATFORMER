using System.Collections;
using UnityEngine;

public class HubIslandPipe : MonoBehaviour
{
    [Header("Departure")]
    [SerializeField] Transform entryPoint;

    [Header("Destination")]
    [SerializeField] Transform arrivalPoint;
    [SerializeField] Transform exitPoint;
    [SerializeField] Transform playerExitPoint;
    [SerializeField] Transform followerExitPoint;
    
    [Header("Travel Destination")]
    [SerializeField] int destinationIslandID;
    [SerializeField] string destinationIslandName;
    public int DestinationIslandID =>
    destinationIslandID;
    public string DestinationIslandName =>
        destinationIslandName;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpDuration = 0.6f;
    [SerializeField] float pipeEntryDepth = 1.2f;
    [SerializeField] float pipeShrinkDuration = 0.25f;

    PipeScreenTransition screenTransition;
    HubCameraFollow hubCamera;
    HubSkyColorTransition skyColorTransition;
    [SerializeField] ProgressionGateTrigger progressionGateTrigger;


    bool isTransporting;
    Vector3 playerOriginalScale;
    Vector3 followerOriginalScale;

    HubFollower follower;

    public void StartTransport(HubPlayerController3D player)
    {
        if (isTransporting)
            return;

        if (player == null)
            return;

        screenTransition = FindFirstObjectByType<PipeScreenTransition>();
        skyColorTransition =FindFirstObjectByType<HubSkyColorTransition>();


        follower = FindFirstObjectByType<HubFollower>();

        hubCamera = FindFirstObjectByType<HubCameraFollow>();

        if (follower == null)
            return;

        isTransporting = true;

        StartCoroutine(PipeTransportSequence(player));
    }

    IEnumerator PipeTransportSequence(
        HubPlayerController3D player
    )
    {
        // Close the progression UI and return from UI input.
        HubProgressionGateUI gateUI =
            FindFirstObjectByType<HubProgressionGateUI>();

        if (gateUI != null)
            gateUI.CloseForTransport();

        // Disable normal player control.
        player.enabled = false;

        // Disable follower control.
        follower.enabled = false;

        playerOriginalScale =
    player.transform.localScale;

        followerOriginalScale =
            follower.transform.localScale;

        // Stop rigidbody movement.
        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        Rigidbody followerRb =
            follower.GetComponent<Rigidbody>();

        if (followerRb != null)
        {
            followerRb.linearVelocity = Vector3.zero;
            followerRb.angularVelocity = Vector3.zero;
            followerRb.isKinematic = true;
        }

        // Move both characters toward the pipe.
        yield return StartCoroutine(
            MoveCharactersToPipe(player)
        );

        // Make both characters jump into the pipe.
        yield return StartCoroutine(
            JumpIntoPipe(player)
        );

        if (screenTransition != null)
        {
            yield return StartCoroutine(
                screenTransition.Close()
            );
        }

        if (skyColorTransition != null)
        {
            skyColorTransition.SetDestinationSkyColor();
        }

        MovePlayersToArrivalPoint(player);

        yield return new WaitForSeconds(0.25f);

        // Bring the players back at the destination pipe.
        player.gameObject.SetActive(true);
        follower.gameObject.SetActive(true);

        // Make sure they are at the destination pipe.
        player.transform.position =
            exitPoint.position;

        follower.transform.position =
            exitPoint.position
            - arrivalPoint.forward * 0.5f;

        // Make sure their scale is restored.
        player.transform.localScale =
     playerOriginalScale;

        follower.transform.localScale =
            followerOriginalScale;

        // Make sure the camera follows the destination player.
        if (hubCamera != null)
        {
            hubCamera.SetTarget(player.transform);
            hubCamera.enabled = true;
        }

        // Give the camera a brief moment to catch the player.
        yield return new WaitForSeconds(0.2f);

        // Start the exit jump and iris opening together.
        Coroutine jumpOutRoutine =
            StartCoroutine(JumpOutOfPipe(player));

        if (screenTransition != null)
        {
            yield return StartCoroutine(
                screenTransition.Open()
            );
        }

        // Make absolutely sure the jump has finished.
        yield return jumpOutRoutine;

        // Now give control back to the normal hub systems.
        RestorePlayerControl(player);
        follower.ResumeFollowing(player.transform);

        if (gateUI != null)
        {
            gateUI.FinishTransport();
        }

        isTransporting = false;

        if (progressionGateTrigger != null)
        {
            progressionGateTrigger.EnableTrigger();
        }
    }

    void MovePlayersToArrivalPoint(
    HubPlayerController3D player
)
    {
        player.transform.position =
            arrivalPoint.position;

        follower.transform.position =
            arrivalPoint.position
            - arrivalPoint.forward * 0.5f;
    }

    IEnumerator JumpOutOfPipe(
    HubPlayerController3D player
)
    {
        Vector3 playerStart =
            exitPoint.position;

        Vector3 followerStart =
            exitPoint.position
            - arrivalPoint.forward * 0.5f;

        Vector3 playerEnd =
            playerExitPoint.position;

        Vector3 followerEnd =
            followerExitPoint.position;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / jumpDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            player.transform.position =
                GetArcPosition(
                    playerStart,
                    playerEnd,
                    jumpHeight,
                    smoothT
                );

            follower.transform.position =
                GetArcPosition(
                    followerStart,
                    followerEnd,
                    jumpHeight,
                    smoothT
                );

            yield return null;
        }

        player.transform.position =
            playerEnd;

        follower.transform.position =
            followerEnd;
    }

    void RestorePlayerControl(
    HubPlayerController3D player
)
    {
        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        Rigidbody followerRb =
            follower.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (followerRb != null)
        {
            followerRb.isKinematic = false;
            followerRb.linearVelocity = Vector3.zero;
            followerRb.angularVelocity = Vector3.zero;
        }
    }

    void RestoreHubCamera(
    HubPlayerController3D player
)
    {
        if (hubCamera == null)
            return;

        hubCamera.SetTarget(player.transform);
    }

    IEnumerator MoveCharactersToPipe(
        HubPlayerController3D player
    )
    {
        Vector3 playerTarget = entryPoint.position;

        Vector3 followerTarget =
            entryPoint.position
            - entryPoint.forward * 0.8f;

        while (true)
        {
            Vector3 playerPosition =
                player.transform.position;

            Vector3 followerPosition =
                follower.transform.position;

            playerPosition = Vector3.MoveTowards(
                playerPosition,
                playerTarget,
                moveSpeed * Time.deltaTime
            );

            followerPosition = Vector3.MoveTowards(
                followerPosition,
                followerTarget,
                moveSpeed * Time.deltaTime
            );

            player.transform.position =
                playerPosition;

            follower.transform.position =
                followerPosition;

            if (
                Vector3.Distance(
                    playerPosition,
                    playerTarget
                ) < 0.05f &&
                Vector3.Distance(
                    followerPosition,
                    followerTarget
                ) < 0.05f
            )
            {
                break;
            }

            yield return null;
        }

        player.transform.position = playerTarget;
        follower.transform.position = followerTarget;
    }

    IEnumerator JumpIntoPipe(
    HubPlayerController3D player
)
    {
        Vector3 playerStart =
            player.transform.position;

        Vector3 followerStart =
            follower.transform.position;

        // The top/rim of the pipe.
        Vector3 playerPipeTop =
            entryPoint.position;

        Vector3 followerPipeTop =
            entryPoint.position
            - entryPoint.forward * 0.5f;

        // Move down into the vertical pipe.
        Vector3 playerPipeBottom =
            playerPipeTop
            + Vector3.down * pipeEntryDepth;

        Vector3 followerPipeBottom =
            followerPipeTop
            + Vector3.down * pipeEntryDepth;

        float timer = 0f;

        // ------------------------------------------------
        // PART 1: Jump into the pipe
        // ------------------------------------------------

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / jumpDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            player.transform.position =
                GetArcPosition(
                    playerStart,
                    playerPipeTop,
                    jumpHeight,
                    smoothT
                );

            follower.transform.position =
                GetArcPosition(
                    followerStart,
                    followerPipeTop,
                    jumpHeight,
                    smoothT
                );

            yield return null;
        }

        // Make sure they're exactly at the pipe rim.
        player.transform.position =
            playerPipeTop;

        follower.transform.position =
            followerPipeTop;

        // ------------------------------------------------
        // PART 2: Sink into the pipe + shrink
        // ------------------------------------------------

        Vector3 playerStartScale =
            player.transform.localScale;

        Vector3 followerStartScale =
            follower.transform.localScale;

        Vector3 playerOriginalScale =
    player.transform.localScale;

        Vector3 followerOriginalScale =
            follower.transform.localScale;

        Vector3 playerEndScale =
            Vector3.zero;

        Vector3 followerEndScale =
            Vector3.zero;

        timer = 0f;

        while (timer < pipeShrinkDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / pipeShrinkDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            player.transform.position =
                Vector3.Lerp(
                    playerPipeTop,
                    playerPipeBottom,
                    smoothT
                );

            follower.transform.position =
                Vector3.Lerp(
                    followerPipeTop,
                    followerPipeBottom,
                    smoothT
                );

            player.transform.localScale =
                Vector3.Lerp(
                    playerStartScale,
                    playerEndScale,
                    smoothT
                );

            follower.transform.localScale =
                Vector3.Lerp(
                    followerStartScale,
                    followerEndScale,
                    smoothT
                );

            yield return null;
        }

        player.transform.position =
            playerPipeBottom;

        follower.transform.position =
            followerPipeBottom;

        player.transform.localScale =
            playerEndScale;

        follower.transform.localScale =
            followerEndScale;

        player.gameObject.SetActive(false);
        follower.gameObject.SetActive(false);
    }

    Vector3 GetArcPosition(
    Vector3 start,
    Vector3 end,
    float height,
    float t
)
    {
        Vector3 midpoint =
            Vector3.Lerp(start, end, 0.5f);

        Vector3 controlPoint =
            midpoint + Vector3.up * height;

        float oneMinusT = 1f - t;

        return
            oneMinusT * oneMinusT * start +
            2f * oneMinusT * t * controlPoint +
            t * t * end;
    }

    public void StartFastTravel(HubPlayerController3D player)
    {
        if (isTransporting)
            return;

        if (player == null)
            return;

        // Make absolutely sure the travel UI is closed.
        HubPipeTravelUI travelUI =
            FindFirstObjectByType<HubPipeTravelUI>();

        if (travelUI != null)
        {
            travelUI.Close();
        }

        screenTransition =
            FindFirstObjectByType<PipeScreenTransition>();

        skyColorTransition =
            FindFirstObjectByType<HubSkyColorTransition>();

        follower =
            FindFirstObjectByType<HubFollower>();

        hubCamera =
            FindFirstObjectByType<HubCameraFollow>();

        if (follower == null)
            return;

        isTransporting = true;

        StartCoroutine(
            FastTravelSequence(player)
        );
    }

    IEnumerator FastTravelSequence(
    HubPlayerController3D player
)
    {
        // Disable player control.
        player.enabled = false;

        // Disable follower control.
        follower.enabled = false;

        // Stop player physics.
        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        // Stop follower physics.
        Rigidbody followerRb =
            follower.GetComponent<Rigidbody>();

        if (followerRb != null)
        {
            followerRb.linearVelocity = Vector3.zero;
            followerRb.angularVelocity = Vector3.zero;
            followerRb.isKinematic = true;
        }

        // Close the iris.
        if (screenTransition != null)
        {
            yield return StartCoroutine(
                screenTransition.Close()
            );
        }

        // Change the sky while the screen is black.
        if (skyColorTransition != null)
        {
            skyColorTransition.SetDestinationSkyColor();
        }

        // Find the spawn point for the destination island.
        HubIslandSpawnPoint[] spawnPoints =
            FindObjectsByType<HubIslandSpawnPoint>(
                FindObjectsSortMode.None
            );

        HubIslandSpawnPoint destinationSpawn = null;

        foreach (
            HubIslandSpawnPoint spawnPoint
            in spawnPoints
        )
        {
            if (
                spawnPoint.IslandID ==
                destinationIslandID
            )
            {
                destinationSpawn = spawnPoint;
                break;
            }
        }

        if (destinationSpawn == null)
        {
            Debug.LogError(
                $"[HubIslandPipe] No HubIslandSpawnPoint found for island {destinationIslandID}."
            );

            RestorePlayerControl(player);

            follower.ResumeFollowing(
                player.transform
            );

            isTransporting = false;

            yield break;
        }

        // Teleport the player.
        player.transform.position =
            destinationSpawn.transform.position;

        // Put the follower beside the player.
        follower.transform.position =
            destinationSpawn.transform.position
            - destinationSpawn.transform.right * 1.5f;

        // Save the new island.
        if (HubIslandSaveManager.Instance != null)
        {
            HubIslandSaveManager.Instance.SetCurrentIsland(
                destinationIslandID
            );
        }

        // Make sure the camera follows the player.
        if (hubCamera != null)
        {
            hubCamera.SetTarget(
                player.transform
            );

            hubCamera.enabled = true;
        }

        // Give the camera one frame to update.
        yield return null;

        // Open the iris.
        if (screenTransition != null)
        {
            yield return StartCoroutine(
                screenTransition.Open()
            );
        }

        // Restore normal movement.
        RestorePlayerControl(player);

        follower.ResumeFollowing(
            player.transform
        );

        isTransporting = false;
    }
}