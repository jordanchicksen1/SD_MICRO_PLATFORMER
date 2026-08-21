using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopCameraController : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> players = new();
    Dictionary<PlayerHealth, System.Action> deathHandlers = new Dictionary<PlayerHealth, System.Action>();

    [Header("Follow")]
    [SerializeField] float followSmoothTime = 0.2f;
    Vector3 followVelocity;

    [Header("Zoom")]
    [SerializeField] float minZoom = 1f;
    [SerializeField] float maxZoom = 30f;
    [SerializeField] float zoomLimiter = 10f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 120f;
    [SerializeField] float fixedPitch = 35f;
    [SerializeField] bool snapYaw = true;
    [SerializeField] float snapAngle = 90f;
    float currentYaw; 
    [SerializeField] float rotationSmoothTime = 0.05f; // small lag for smoothness
    float rotationVelocity; // used for SmoothDamp
    float sharedYawBeforeSplit;
    [SerializeField] CameraRotationWarning player1RotationWarning;
    [SerializeField] CameraRotationWarning player2RotationWarning;

    Transform pivot;
    Camera cam;

    [Header("Split Screen")]
    [SerializeField] Transform player1Pivot;
    [SerializeField] Transform player2Pivot;
    [SerializeField] Camera player1Camera;
    [SerializeField] Camera player2Camera;
    [SerializeField] float splitCameraDistance = 10f;
    [SerializeField] float splitFollowSmoothTime = 0.2f;
    [SerializeField] float splitDistance = 14f;
    [SerializeField] float mergeDistance = 11f;
    bool isSplitScreen;
    [SerializeField] CameraTransition cameraTransition;
    [SerializeField] float splitScreenDelay = 0.5f;
    Coroutine splitScreenDelayRoutine;

    [Header("Cutscene")]
    public bool cutsceneActive;
    float rotationInput;


    void Awake()
    {
        pivot = transform.GetChild(0);
        cam = pivot.GetComponentInChildren<Camera>();

        currentYaw = pivot.eulerAngles.y;
    }

    public void RegisterPlayer(Transform player)
    {
        if (player == null)
            return;

        if (!players.Contains(player))
            players.Add(player);

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null && !deathHandlers.ContainsKey(health))
        {
            System.Action handler = () => OnPlayerDied(player);

            deathHandlers.Add(health, handler);
            health.OnDied += handler;
        }
    }

    public void UnregisterPlayer(Transform player)
    {
        if (player == null)
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null &&
            deathHandlers.TryGetValue(health, out System.Action handler))
        {
            health.OnDied -= handler;
            deathHandlers.Remove(health);
        }

        players.Remove(player);
    }

    void OnPlayerDied(Transform deadPlayer)
    {
        Debug.Log($"Camera: {deadPlayer.name} died. Forcing shared camera.");

        // Cancel any pending split-screen activation.
        CancelSplitScreenDelay();

        // If split-screen is currently active, immediately begin
        // returning to the shared camera.
        if (isSplitScreen)
        {
            DisableSplitScreen();
        }
    }

    public Camera GetCameraForPlayer(int playerIndex)
    {
        if (isSplitScreen)
        {
            if (playerIndex == 0)
                return player1Camera;

            if (playerIndex == 1)
                return player2Camera;
        }

        return cam;
    }

    public void ReplaceTarget(Transform removeThis, Transform addThis)
    {
        UnregisterPlayer(removeThis);
        RegisterPlayer(addThis);
    }


    Vector3 GetCenterPoint()
    {
        if (players.Count == 1)
            return players[0].position;

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 1; i < players.Count; i++)
            bounds.Encapsulate(players[i].position);

        return bounds.center;
    }

    Transform GetPlayerTarget(int playerIndex)
    {
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            PlayerInput input =
                player.GetComponent<PlayerInput>();

            if (input != null &&
                input.playerIndex == playerIndex)
            {
                return player;
            }
        }

        return null;
    }



    void UpdateSplitScreenState()
    {
        if (players.Count < 2)
            return;

        // Death always forces shared camera.
        if (IsEitherPlayerDead())
        {
            CancelSplitScreenDelay();

            if (isSplitScreen)
                DisableSplitScreen();

            return;
        }

        // Don't make another decision while the camera
        // is already transitioning.
        if (IsCameraTransitioning())
            return;

        Transform player1Transform = GetPlayerTarget(0);
        Transform player2Transform = GetPlayerTarget(1);

        if (player1Transform == null || player2Transform == null)
            return;

        PlayerController3D player1 =
            player1Transform.GetComponent<PlayerController3D>();

        PlayerController3D player2 =
            player2Transform.GetComponent<PlayerController3D>();

        if (player1 == null || player2 == null)
            return;

        float distance = Vector3.Distance(
            player1Transform.position,
            player2Transform.position
        );

        bool bothPlayersGrounded =
            player1.IsPlayerGrounded() &&
            player2.IsPlayerGrounded();

        // =========================
        // ENTER SPLIT SCREEN
        // =========================

        if (!isSplitScreen)
        {
            if (distance >= splitDistance &&
                bothPlayersGrounded)
            {
                if (splitScreenDelayRoutine == null)
                {
                    splitScreenDelayRoutine =
                        StartCoroutine(DelayedSplitScreen());
                }
            }
            else
            {
                CancelSplitScreenDelay();
            }

            return;
        }

        // =========================
        // RETURN TO SHARED CAMERA
        // =========================

        if (distance <= mergeDistance &&
            bothPlayersGrounded)
        {
            DisableSplitScreen();
        }
    }

    IEnumerator DelayedSplitScreen()
    {
        yield return new WaitForSeconds(splitScreenDelay);

        splitScreenDelayRoutine = null;

        if (players.Count < 2)
            yield break;

        if (IsCameraTransitioning())
            yield break;

        Transform player1Transform = GetPlayerTarget(0);
        Transform player2Transform = GetPlayerTarget(1);

        if (player1Transform == null || player2Transform == null)
            yield break;

        PlayerController3D player1 =
            player1Transform.GetComponent<PlayerController3D>();

        PlayerController3D player2 =
            player2Transform.GetComponent<PlayerController3D>();

        if (player1 == null || player2 == null)
            yield break;

        // Don't split if either player has died.
        if (IsEitherPlayerDead())
            yield break;

        float distance = Vector3.Distance(
            player1Transform.position,
            player2Transform.position
        );

        bool bothPlayersGrounded =
            player1.IsPlayerGrounded() &&
            player2.IsPlayerGrounded();

        // Check the conditions AGAIN after the delay.
        if (distance >= splitDistance &&
            bothPlayersGrounded &&
            !isSplitScreen)
        {
            EnableSplitScreen();
        }
    }

    void CancelSplitScreenDelay()
    {
        if (splitScreenDelayRoutine != null)
        {
            StopCoroutine(splitScreenDelayRoutine);
            splitScreenDelayRoutine = null;
        }
    }

    bool IsEitherPlayerDead()
    {
        Transform player1Transform = GetPlayerTarget(0);
        Transform player2Transform = GetPlayerTarget(1);

        if (player1Transform == null || player2Transform == null)
            return false;

        PlayerHealth player1Health =
            player1Transform.GetComponent<PlayerHealth>();

        PlayerHealth player2Health =
            player2Transform.GetComponent<PlayerHealth>();

        if (player1Health == null || player2Health == null)
            return false;

        return player1Health.hasDied || player2Health.hasDied;
    }


    void UpdateSplitCameraPositions()
    {
        Transform p1 = GetPlayerTarget(0);
        Transform p2 = GetPlayerTarget(1);

        if (p1 != null && player1Pivot != null)
        {
            player1Pivot.position = p1.position;

            if (player1Camera != null)
            {
                player1Camera.transform.localPosition =
                    new Vector3(0f, 0f, -splitCameraDistance);
            }
        }

        if (p2 != null && player2Pivot != null)
        {
            player2Pivot.position = p2.position;

            if (player2Camera != null)
            {
                player2Camera.transform.localPosition =
                    new Vector3(0f, 0f, -splitCameraDistance);
            }
        }
    }



    void SyncSplitCameraTransforms()
    {
        // Remember the exact shared camera angle
        // at the moment split-screen begins.
        sharedYawBeforeSplit = pivot.eulerAngles.y;

        Transform player1 = GetPlayerTarget(0);
        Transform player2 = GetPlayerTarget(1);

        if (player1 != null && player1Pivot != null)
        {
            player1Pivot.position = player1.position;
            player1Pivot.rotation = pivot.rotation;
        }

        if (player2 != null && player2Pivot != null)
        {
            player2Pivot.position = player2.position;
            player2Pivot.rotation = pivot.rotation;
        }
    }


    public void EnableSplitScreen()
    {
        if (players.Count < 2)
            return;

        if (cameraTransition != null)
        {
            cameraTransition.PlayTransition(
                EnableSplitScreenImmediate
            );
        }
        else
        {
            EnableSplitScreenImmediate();
        }
    }

    void EnableSplitScreenImmediate()
    {
        SyncSplitCameraTransforms();

        isSplitScreen = true;

        if (cam != null)
            cam.enabled = false;

        if (player1Camera != null)
            player1Camera.enabled = true;

        if (player2Camera != null)
            player2Camera.enabled = true;
    }

    public void DisableSplitScreen()
    {
        if (cameraTransition != null)
        {
            cameraTransition.PlayTransition(
                DisableSplitScreenImmediate
            );
        }
        else
        {
            DisableSplitScreenImmediate();
        }
    }

    void DisableSplitScreenImmediate()
    {
        // Restore the exact shared camera angle.
        currentYaw = sharedYawBeforeSplit;

        pivot.rotation =
            Quaternion.Euler(
                fixedPitch,
                sharedYawBeforeSplit,
                0f
            );

        // Make sure the shared camera is already
        // positioned correctly around both players.
        Follow();
        Zoom();

        if (player1Camera != null)
            player1Camera.enabled = false;

        if (player2Camera != null)
            player2Camera.enabled = false;

        isSplitScreen = false;

        // Hide any right-stick warnings when
        // the shared camera becomes active.
        if (player1RotationWarning != null)
        {
            player1RotationWarning.Hide();
        }

        if (player2RotationWarning != null)
        {
            player2RotationWarning.Hide();
        }

        if (cam != null)
            cam.enabled = true;

    }

    public bool IsCameraTransitioning()
    {
        return cameraTransition != null &&
               cameraTransition.IsTransitioning;
    }

    void LateUpdate()
    {
        if (cutsceneActive) return;
        if (players.Count == 0) return;

        // Keep the shared camera's position updated
        // even while split-screen is active.
        Follow();
        Zoom();

        // Only allow camera rotation while the shared
        // camera is active.
        if (!isSplitScreen)
        {
            Rotate();
        }

        UpdateSplitCameraPositions();

        UpdateSplitScreenState();
    }

    //=================== TEST ======================
    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
                EnableSplitScreen();

            if (Keyboard.current.f2Key.wasPressedThisFrame)
                DisableSplitScreen();
        }
    }

    //=================== TEST =======================

    void Follow()
    {
        Vector3 target = GetCenterPoint();
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target,
            ref followVelocity,
            followSmoothTime
        );
    }

    void Zoom()
    {
        if (players.Count < 2)
        {
            cam.transform.localPosition = new Vector3(0, 0, -minZoom);
            return;
        }

        float greatestDistance = 0f;

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = i + 1; j < players.Count; j++)
            {
                float dist = Vector3.Distance(players[i].position, players[j].position);
                greatestDistance = Mathf.Max(greatestDistance, dist);
            }
        }

        float t = Mathf.Clamp01(greatestDistance / zoomLimiter);

        float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);

        cam.transform.localPosition = new Vector3(
            0f,
            0f,
            -targetZoom
        );

        //Debug.Log($"Distance: {greatestDistance}, Zoom: {targetZoom}");
    }


    public void AddRotationInput(float value)
    {
        // store raw stick input
        rotationVelocity = value * rotationSpeed;
    }

 

    void Rotate()
    {
        // Directly add input scaled by deltaTime
        currentYaw += rotationVelocity * Time.deltaTime;

        // Optional smoothing (feels like inertia)
        float smoothedYaw = Mathf.LerpAngle(pivot.eulerAngles.y, currentYaw, 1f - Mathf.Exp(-10f * Time.deltaTime));
        pivot.rotation = Quaternion.Euler(fixedPitch, smoothedYaw, 0f);

        // Reset velocity so input doesn’t stack
       // rotationVelocity = 0f;
    }


    public bool IsSplitScreen()
    {
        return isSplitScreen;
    }

    public void AddRotationInputForPlayer(int playerIndex, float value)
    {
        // Shared camera: right-stick rotation works normally.
        if (!isSplitScreen)
        {
            AddRotationInput(value);
            return;
        }

        // Split-screen: camera rotation is disabled.
        // Only show the warning when the player actually
        // moves the right stick.
        if (Mathf.Abs(value) > 0.1f)
        {
            if (playerIndex == 0)
            {
                if (player1RotationWarning != null)
                    player1RotationWarning.Show();
            }
            else if (playerIndex == 1)
            {
                if (player2RotationWarning != null)
                    player2RotationWarning.Show();
            }
        }
    }


}