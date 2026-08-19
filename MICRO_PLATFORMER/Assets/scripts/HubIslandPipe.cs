using System.Collections;
using UnityEngine;

public class HubIslandPipe : MonoBehaviour
{
    [Header("Departure")]
    [SerializeField] Transform entryPoint;

    [Header("Destination")]
    [SerializeField] Transform arrivalPoint;
    [SerializeField] Transform exitPoint;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpDuration = 0.6f;

    PipeScreenTransition screenTransition;
    HubCameraFollow hubCamera;

    bool isTransporting;

    HubFollower follower;

    public void StartTransport(HubPlayerController3D player)
    {
        if (isTransporting)
            return;

        if (player == null)
            return;

        screenTransition =
    FindFirstObjectByType<PipeScreenTransition>();


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

        MovePlayersToArrivalPoint(player);

        yield return new WaitForSeconds(0.25f);

        // Turn the players back on at the destination.
        player.gameObject.SetActive(true);
        follower.gameObject.SetActive(true);

        // Reset their movement state before the animation.
        player.ResetMovementState();

        // Tell the hub camera to follow the player on the new island.
        RestoreHubCamera(player);

        yield return new WaitForSeconds(0.2f);

        if (screenTransition != null)
        {
            yield return StartCoroutine(
                screenTransition.Open()
            );
        }

        // Jump out of the destination pipe.
        yield return StartCoroutine(
            JumpOutOfPipe(player)
        );

        // Restore normal physics and controls.
        RestorePlayerControl(player);

        isTransporting = false;
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
            arrivalPoint.position;

        Vector3 followerStart =
            arrivalPoint.position
            - arrivalPoint.forward * 0.5f;

        Vector3 playerEnd =
            exitPoint.position;

        Vector3 followerEnd =
            exitPoint.position
            - arrivalPoint.forward * 0.5f;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / jumpDuration);

            float height =
                Mathf.Sin(t * Mathf.PI)
                * jumpHeight;

            Vector3 playerPosition =
                Vector3.Lerp(
                    playerStart,
                    playerEnd,
                    t
                );

            Vector3 followerPosition =
                Vector3.Lerp(
                    followerStart,
                    followerEnd,
                    t
                );

            playerPosition.y += height;
            followerPosition.y += height;

            player.transform.position =
                playerPosition;

            follower.transform.position =
                followerPosition;

            yield return null;
        }

        player.transform.position =
            playerEnd;

        follower.transform.position =
            followerEnd;
    }

    void RestorePlayerControl(HubPlayerController3D player)
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

        player.ResetMovementState();

        player.enabled = true;
        follower.enabled = true;
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

        Vector3 playerEnd =
            entryPoint.position;

        Vector3 followerEnd =
            entryPoint.position
            - entryPoint.forward * 0.5f;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / jumpDuration);

            float height =
                Mathf.Sin(t * Mathf.PI)
                * jumpHeight;

            Vector3 playerPosition =
                Vector3.Lerp(
                    playerStart,
                    playerEnd,
                    t
                );

            Vector3 followerPosition =
                Vector3.Lerp(
                    followerStart,
                    followerEnd,
                    t
                );

            playerPosition.y += height;
            followerPosition.y += height;

            player.transform.position =
                playerPosition;

            follower.transform.position =
                followerPosition;

            yield return null;
        }

        player.transform.position =
            playerEnd;

        follower.transform.position =
            followerEnd;

        // Hide the characters once they have entered the pipe.
        player.gameObject.SetActive(false);
        follower.gameObject.SetActive(false);
    }
}