using UnityEngine;
using System.Collections.Generic;

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


    Transform pivot;
    Camera cam;

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


    void LateUpdate()
    {
        if (players.Count == 0) return;

        Follow();
        Zoom();
        Rotate(); // this will use rotationInput
    }


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








}