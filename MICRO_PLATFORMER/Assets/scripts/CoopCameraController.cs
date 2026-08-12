using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopCameraController : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> players = new();

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
    bool wasRotating;
    [SerializeField] float rotationSmoothTime = 0.05f; // small lag for smoothness
    float rotationVelocity; // used for SmoothDamp
    float splitYawP1;
    float splitYawP2;
    float sharedYawBeforeSplit;
    float splitRotationVelocityP1;
    float splitRotationVelocityP2;

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
        if (!players.Contains(player))
            players.Add(player);
    }

    public void UnregisterPlayer(Transform player)
    {
        if (player == null) return;
        players.Remove(player);
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

    Transform GetPlayerTarget(int index)
    {
        if (index < 0 || index >= players.Count)
            return null;

        return players[index];
    }

    float GetPlayerDistance()
    {
        if (players.Count < 2)
            return 0f;

        if (players[0] == null || players[1] == null)
            return 0f;

        return Vector3.Distance(
            players[0].position,
            players[1].position
        );
    }



    void UpdateSplitScreenState()
    {
        if (players.Count < 2)
            return;

        float distance = GetPlayerDistance();

        if (!isSplitScreen && distance >= splitDistance)
        {
            EnableSplitScreen();
        }
        else if (isSplitScreen && distance <= mergeDistance)
        {
            DisableSplitScreen();
        }
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

    void SyncSharedCameraToSplitCameras()
    {
        if (players.Count < 2)
            return;

        if (player1Pivot == null || player2Pivot == null)
            return;

        // Keep the shared camera positioned between the players.
        Vector3 sharedPosition =
            (player1Pivot.position + player2Pivot.position) * 0.5f;

        pivot.position = sharedPosition;

        // Keep the shared camera between the two split-camera angles.
        float yaw1 = splitYawP1;
        float yaw2 = splitYawP2;

        float averageYaw = Mathf.LerpAngle(yaw1, yaw2, 0.5f);

        currentYaw = averageYaw;

        pivot.rotation =
            Quaternion.Euler(fixedPitch, averageYaw, 0f);

        // Keep the shared camera zoomed to the appropriate distance.
        Zoom();
    }

    void SyncSplitCameraTransforms()
    {
        sharedYawBeforeSplit = pivot.eulerAngles.y;

        splitYawP1 = sharedYawBeforeSplit;
        splitYawP2 = sharedYawBeforeSplit;

        splitRotationVelocityP1 = 0f;
        splitRotationVelocityP2 = 0f;

        if (players.Count >= 2)
        {
            if (player1Pivot != null)
            {
                player1Pivot.position = players[0].position;
                player1Pivot.rotation = pivot.rotation;
            }

            if (player2Pivot != null)
            {
                player2Pivot.position = players[1].position;
                player2Pivot.rotation = pivot.rotation;
            }
        }
    }


    public void EnableSplitScreen()
    {
        if (players.Count < 2)
            return;

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
        // Restore the shared camera's original orientation
        // before turning it back on.
        currentYaw = sharedYawBeforeSplit;

        pivot.rotation =
            Quaternion.Euler(fixedPitch, sharedYawBeforeSplit, 0f);

        // Make sure the shared camera is positioned correctly
        // around the current player group.
        Follow();
        Zoom();

        player1Camera.enabled = false;
        player2Camera.enabled = false;

        isSplitScreen = false;

        cam.enabled = true;
    }

    void LateUpdate()
    {
        if (cutsceneActive) return;
        if (players.Count == 0) return;

        if (!isSplitScreen)
        {
            Follow();
            Zoom();
            Rotate();
        }
        else
        {
            RotateSplitCameras();
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

    public void AddSplitRotationInput(int playerIndex, float value)
    {
        if (playerIndex == 0)
        {
            splitRotationVelocityP1 = value * rotationSpeed;
        }
        else if (playerIndex == 1)
        {
            splitRotationVelocityP2 = value * rotationSpeed;
        }
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

    void RotateSplitCameras()
    {
        splitYawP1 += splitRotationVelocityP1 * Time.deltaTime;
        splitYawP2 += splitRotationVelocityP2 * Time.deltaTime;

        if (player1Pivot != null)
        {
            player1Pivot.rotation =
                Quaternion.Euler(fixedPitch, splitYawP1, 0f);
        }

        if (player2Pivot != null)
        {
            player2Pivot.rotation =
                Quaternion.Euler(fixedPitch, splitYawP2, 0f);
        }
    }


    public void AddRotationInputForPlayer(int playerIndex, float value)
    {
        if (isSplitScreen)
        {
            AddSplitRotationInput(playerIndex, value);
        }
        else
        {
            AddRotationInput(value);
        }
    }


}