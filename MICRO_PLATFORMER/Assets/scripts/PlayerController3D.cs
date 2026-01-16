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

    [Header("Jump Assist")]
    [SerializeField] float coyoteTime = 0.15f;

    float lastGroundedTime;


    Rigidbody rb;
    Vector2 moveInput;
    bool jumpHeld;

    [Header("Ground Pound")]
    [SerializeField] float groundPoundForce = 20f;
    [SerializeField] float groundPoundLockTime = 0.15f;

    bool isGroundPounding;
    bool groundPoundQueued;

    [Header("Dive")]
    [SerializeField] float diveForwardForce = 12f;
    [SerializeField] float diveDownForce = 4f;
    [SerializeField] float diveDuration = 0.35f;
    [SerializeField] float diveControlLockTime = 0.2f;

    bool isDiving;

    [Header("Long Jump")]
    [SerializeField] float longJumpForwardForce = 14f;
    [SerializeField] float longJumpUpForce = 3f;
    [SerializeField] float longJumpDuration = 0.25f;
    [SerializeField] float longJumpLockTime = 0.2f;

    bool isLongJumping;



    [Header("Player Differentiation")]
    int playerIndex;
    int playerNumber;
    [SerializeField] GameObject[] playerVisuals;
    public GameObject p1Tag;
    public GameObject p2Tag;


    [Header("Camera Stuff")]
    CoopCameraController coopCamera;
    OffScreenIndicatorManager indicatorManager;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 8f;
    [SerializeField] float knockbackUpwardForce = 3f;
    [SerializeField] float knockbackLockTime = 0.2f;
    [SerializeField] Renderer[] renderers;
    [SerializeField] float flashInterval = 0.1f;
    Material[][] cachedMaterials;


    bool isKnockedBack;

    [Header("Ground Indicator")]
    [SerializeField] GameObject groundIndicatorPrefab;
    [SerializeField] Material player1Mat;
    [SerializeField] Material player2Mat;

    PlayerAnimator playerAnimator;

    [Header("Interactables")]
    [SerializeField] float interactRange = 2f;
    [SerializeField] LayerMask interactLayer;

    public bool IsGroundPounding()
    {
        return isGroundPounding;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>();

        cachedMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            cachedMaterials[i] = renderers[i].materials;
        }
    }

    private void Start()
    {
        coopCamera = FindFirstObjectByType<CoopCameraController>();
        coopCamera.RegisterPlayer(transform);

        playerIndex = GetComponent<PlayerInput>().playerIndex;
        playerNumber = playerIndex + 1;

        Debug.Log($"Player {playerNumber} joined");
        
        indicatorManager = FindFirstObjectByType<OffScreenIndicatorManager>();
        indicatorManager.RegisterPlayer(this);

        PlayerHealthUIManager healthUIManager =
        FindFirstObjectByType<PlayerHealthUIManager>();

        healthUIManager.RegisterPlayer(GetComponent<PlayerHealth>());

        PlayerHealth health = GetComponent<PlayerHealth>();
        health.OnDamaged += ApplyKnockback;
        

        for (int i = 0; i < playerVisuals.Length; i++)
        {
            playerVisuals[i].SetActive(i == playerIndex);
        }

        StartCoroutine(TurnOffP1Tag());
        StartCoroutine(TurnOffP2Tag());

        playerAnimator = GetComponentInChildren<PlayerAnimator>();


        GameObject indicator = Instantiate(groundIndicatorPrefab, transform.position, Quaternion.identity);

        GroundIndicator gi = indicator.GetComponent<GroundIndicator>();
        gi.SetTarget(transform);

        Renderer r = indicator.GetComponentInChildren<Renderer>();

        if (playerIndex == 0)
            r.material = player1Mat;
        else
            r.material = player2Mat;


    }

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
        }

        HandleGroundPound();

        if (!isGroundPounding && !isKnockedBack && !isDiving && !isLongJumping)
            Move();


        if (!isKnockedBack && !isDiving && !isLongJumping)
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
        if (isGroundPounding || isDiving || isLongJumping)
            return;



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

            // Stop motion
            rb.linearVelocity = Vector3.zero;

            // Slam down
            rb.AddForce(Vector3.down * groundPoundForce, ForceMode.Impulse);

            // ?? TELL ANIMATOR
            if (playerAnimator != null)
            {
                playerAnimator.SetGroundPound(true);
                Debug.Log("GROUND POUND START");
            }
        }

        if (isGroundPounding && IsGrounded())
        {
            OnGroundPoundImpact();
        }
    }


    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        float stickX = context.ReadValue<Vector2>().x; // current frame value
        coopCamera.AddRotationInput(stickX); // pass it directly to the camera
    }



    void OnGroundPoundImpact()
    {
        isGroundPounding = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetGroundPound(false);
            Debug.Log("GROUND POUND END");
        }

        StartCoroutine(GroundPoundRecovery());
    }


    public void OnDive(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (IsGrounded())
        {
            // Ground ? Long Jump
            if (!isLongJumping && !isGroundPounding && !isKnockedBack)
            {
                StartCoroutine(LongJumpCoroutine());
            }
        }
        else
        {
            // Air ? Dive (existing behavior)
            if (!isDiving && !isGroundPounding)
            {
                StartCoroutine(DiveCoroutine());
            }
        }
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

        if (!context.performed)
            return;

        bool canCoyoteJump = Time.time - lastGroundedTime <= coyoteTime;

        if (canCoyoteJump && !isGroundPounding && !isDiving && !isLongJumping)
        {
            // Clear downward velocity so jump is consistent
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryInteract();
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        

    }

    void ApplyKnockback(Vector3 sourcePosition)
    {
        if (isKnockedBack)
            return;

        StartCoroutine(KnockbackCoroutine(sourcePosition));

        PlayerHealth health = GetComponent<PlayerHealth>();
        StartCoroutine(FlashCoroutine(health.InvulnerabilityDuration));
    }


    void SetFlash(bool normal)
    {
        for (int i = 0; i < cachedMaterials.Length; i++)
        {
            foreach (Material m in cachedMaterials[i])
            {
                if (!m.HasProperty("_EmissionColor")) continue;

                m.EnableKeyword("_EMISSION");

                if (normal)
                {
                    m.SetColor("_EmissionColor", Color.black);
                }
                else
                {
                    m.SetColor("_EmissionColor", Color.white * 3f);
                }
            }
        }
    }




    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            GetComponent<PlayerHealth>().TakeDamage(1, other.transform.position);
           
        }

        if(other.tag == "Heart")
        {
            GetComponent<PlayerHealth>().Heal(1);

        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
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

    IEnumerator KnockbackCoroutine(Vector3 sourcePosition)
    {
        isKnockedBack = true;

        Vector3 direction = (transform.position - sourcePosition).normalized;

        rb.linearVelocity = Vector3.zero;

        Vector3 force =
            direction * knockbackForce +
            Vector3.up * knockbackUpwardForce;

        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackLockTime);

        isKnockedBack = false;
    }

    IEnumerator FlashCoroutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            SetFlash(false);
            yield return new WaitForSeconds(flashInterval);

            SetFlash(true);
            yield return new WaitForSeconds(flashInterval);

            timer += flashInterval * 2f;
        }

        SetFlash(true); // hard reset
    }


    IEnumerator DiveCoroutine()
    {
        isDiving = true;

        // Kill current motion
        rb.linearVelocity = Vector3.zero;

        // Forward dive direction (character facing)
        Vector3 diveDir = -transform.forward;

        Vector3 force =
            diveDir * diveForwardForce +
            Vector3.down * diveDownForce;

        rb.AddForce(force, ForceMode.Impulse);

        // Notify animator
        if (playerAnimator != null)
            playerAnimator.SetDive(true);

        float timer = 0f;

        while (timer < diveDuration && !IsGrounded())
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isDiving = false;

        if (playerAnimator != null)
            playerAnimator.SetDive(false);
    }

    IEnumerator LongJumpCoroutine()
    {
        isLongJumping = true;

        // Stop current motion
        rb.linearVelocity = Vector3.zero;

        // Forward direction
        Vector3 jumpDir = GetCameraRelativeMovement();
        if (jumpDir.sqrMagnitude < 0.01f)
            jumpDir = -transform.forward;

        Vector3 force =
            jumpDir.normalized * longJumpForwardForce +
            Vector3.up * longJumpUpForce;

        rb.AddForce(force, ForceMode.Impulse);

        // Reuse dive / ground pound animation pose
        if (playerAnimator != null)
            playerAnimator.SetDive(true);

        yield return new WaitForSeconds(longJumpDuration);

        if (playerAnimator != null)
            playerAnimator.SetDive(false);

        yield return new WaitForSeconds(longJumpLockTime);

        isLongJumping = false;
    }




}
