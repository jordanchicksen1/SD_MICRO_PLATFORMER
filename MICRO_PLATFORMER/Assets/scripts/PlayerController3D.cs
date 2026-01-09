using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float rotationSpeed = 12f;

    [Header("Gravity")]
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    Rigidbody rb;
    Vector2 moveInput;
    bool jumpHeld;

    [Header("Ground Pound")]
    [SerializeField] float groundPoundForce = 20f;
    [SerializeField] float groundPoundLockTime = 0.15f;

    bool isGroundPounding;
    bool groundPoundQueued;


    [Header("Player Differentiation")]
    int playerIndex;
    int playerNumber;
    [SerializeField] GameObject[] playerVisuals;
    public GameObject p1Tag;
    public GameObject p2Tag;

    [Header("Camera Stuff")]
    CoopCameraController coopCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        coopCamera = FindFirstObjectByType<CoopCameraController>();
        coopCamera.RegisterPlayer(transform);



        playerIndex = GetComponent<PlayerInput>().playerIndex;
        playerNumber = playerIndex + 1;

        Debug.Log($"Player {playerNumber} joined");
       
        for (int i = 0; i < playerVisuals.Length; i++)
        {
            playerVisuals[i].SetActive(i == playerIndex);
        }

        StartCoroutine(TurnOffP1Tag());
        StartCoroutine(TurnOffP2Tag());
    }

    void FixedUpdate()
    {
        HandleGroundPound();

        if (!isGroundPounding)
            Move();

        RotateTowardsMovement();
        ApplyBetterGravity();
    }

    Vector3 GetCameraRelativeMovement()
    {
        Camera cam = Camera.main;

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * moveInput.y + right * moveInput.x;
    }


    void Move()
    {
        Vector3 move = GetCameraRelativeMovement();

        rb.linearVelocity = new Vector3(
            move.x * moveSpeed,
            rb.linearVelocity.y,
            move.z * moveSpeed
        );
    }


    void RotateTowardsMovement()
    {
        Vector3 move = GetCameraRelativeMovement();

        if (move.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(-move);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    void ApplyBetterGravity()
    {
        if (isGroundPounding)
            return; // gravity is overridden

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void HandleGroundPound()
    {
        if (groundPoundQueued && !isGroundPounding)
        {
            isGroundPounding = true;
            groundPoundQueued = false;

            // Kill upward or sideways motion
            rb.linearVelocity = Vector3.zero;

            // Slam downward
            rb.AddForce(Vector3.down * groundPoundForce, ForceMode.Impulse);
        }

        // Detect landing
        if (isGroundPounding && IsGrounded())
        {
            OnGroundPoundImpact();
        }
    }

    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<Vector2>().x;
        coopCamera.AddRotationInput(input);
    }



    void OnGroundPoundImpact()
    {
        isGroundPounding = false;

        // Small lockout to avoid instant movement
        StartCoroutine(GroundPoundRecovery());
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpHeld = true;

        if (context.canceled)
            jumpHeld = false;

        if (context.performed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnGroundPound(InputAction.CallbackContext context)
    {
        if (context.performed && !IsGrounded())
        {
            groundPoundQueued = true;
        }
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public IEnumerator TurnOffP1Tag()
    {
        yield return new WaitForSeconds(5f);
        p1Tag.SetActive(false);
    }

    public IEnumerator TurnOffP2Tag()
    {
        yield return new WaitForSeconds(5f);
        p2Tag.SetActive(false);
    }

    public IEnumerator GroundPoundRecovery()
    {
        yield return new WaitForSeconds(groundPoundLockTime);
    }

}
