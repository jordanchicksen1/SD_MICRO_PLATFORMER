using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public enum CombatTool
    {
        Kick,
        BaseballBat,
        Boomerang,
        BoxingGloves
    }

    [SerializeField]
    CombatTool currentTool = CombatTool.Kick;
    bool hasReserveWeapon = false;
    CombatTool reserveTool = CombatTool.Kick;

    public CombatTool CurrentTool => currentTool;
    public bool HasReserveWeapon => hasReserveWeapon;
    public CombatTool ReserveTool => reserveTool;

    PlayerController3D controller;
    PlayerAnimator animator;
    CombatCameraShake combatShake;
    PlayerController3D playerController;
    PlayerInput PlayerInput;
    PlayerHealthUIManager healthUIManager;
    bool isAttacking;
    bool isBatCharging;
    bool isBatSpinning;
    Dictionary<Enemy, float> enemyHitCooldowns = new Dictionary<Enemy, float>();

    [Header("Kick")]
    [SerializeField] Transform kickPoint;
    [SerializeField] float kickRadius = 1f;
    [SerializeField] float kickBallForce = 10f;
    

    [Header("Baseball Bat")]
    [SerializeField] Transform batHitPoint;
    [SerializeField] float batRadius = 1.8f;
    [SerializeField] Transform batSpinHitPoint;
    [SerializeField] float batSpinRadius = 1.5f;
    bool canChargeBat = true;
    [SerializeField] float maxSpinTime = 5f;
    [SerializeField] float spinRechargeTime = 5f;
    float currentSpinTime;
    bool canSpin = true;
    [SerializeField] float batBallForce = 20f;
    SpinMeterUI spinMeterUI;

    [Header("Boomerang")]
    [SerializeField] GameObject boomerangProjectilePrefab;
    bool boomerangInFlight;
    bool isBoomerangCharging;
    bool isBoomerangAimMode;
    BoomerangTargetSelector targetSelector;


    [Header("Boxing Gloves")]
    [SerializeField] Transform gloveHitPoint;
    [SerializeField] float gloveRadius = 1.2f;
    [SerializeField] float uppercutForce = 8f;
    [SerializeField] float enemyLaunchForce = 5f;
    bool isGloveCharging;
    bool canUppercutLift = true;
    bool punchRight = true;
    [SerializeField] float slamRadius = 3f;
    [SerializeField] Transform slamHitPoint;
    [SerializeField] AudioSource glovePunchSFX;
    [SerializeField] AudioClip rightPunchClip;
    [SerializeField] AudioClip leftPunchClip;
    [SerializeField] float gloveChargeTime = 2f;
    [SerializeField] float slamForwardForce = 9f;
    [SerializeField] float slamUpForce = 4f;
    bool slamCharged;
    Coroutine gloveChargeRoutine;
    [SerializeField] float slamLeapForwardForce = 10f;
    [SerializeField] float slamLeapUpForce = 5f;
    [SerializeField] float slamAirTime = 0.15f;
    bool isGroundSlamming;
    bool hasGroundSlamLanded;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip groundSlamSFX;
    bool showGroundSlamUI;
    float groundSlamChargeTimer;
    GroundSlamChargeUI groundSlamChargeUI;

    [Header("Weapon Models")]
    [SerializeField] GameObject baseballBatObject;
    [SerializeField] GameObject boomerangObject;
    [SerializeField] GameObject boxingGlovesObject1;
    [SerializeField] GameObject boxingGlovesObject2;

    [Header("Audio")]
    [SerializeField] AudioSource batAudio;
    [SerializeField] AudioClip batSwingSFX;
    [SerializeField] AudioSource projectileReflectSFX;

    [Header("Trail Renderers")]
    [SerializeField] TrailRenderer batTrail;

    [Header("Weapon Pickup Animation")]
    [SerializeField] float pickupJumpForce = 4f;
    [SerializeField] float pickupForwardForce = 1.5f;
    [SerializeField] Material batPickupMat;
    [SerializeField] Material boomerangPickupMat;
    [SerializeField] Material glovesPickupMat;
    Renderer[] renderers;
    Material[][] originalMaterials;
    [SerializeField] GameObject pickupStarPrefab;
    [SerializeField] int pickupStarCount = 3;
    [SerializeField] float pickupStarForce = 3f;
    [SerializeField] float pickupStarUpForce = 2f;
    [SerializeField] AudioSource pickupAudioSource;
    [SerializeField] AudioClip pickupJumpSFX;
    [SerializeField] AudioClip pickupDongSFX;
    [SerializeField] AudioClip pickupFlashSFX;
    [SerializeField] AudioClip pickupStarSFX;
    Rigidbody rb;

    [Header("Reserve Bubble")]
    [SerializeField] Transform bubbleSpawnPoint;
    [SerializeField] GameObject reserveBubblePrefab;
    [SerializeField] float reserveChargeTime = 1.75f;
    GameObject currentReserveBubble;
    bool isChargingReserve = false;
    bool reserveBubbleVisible = false;
    bool reserveChargeComplete = false;
    float reserveChargeTimer = 0f;
    [SerializeField]
    float reserveBubbleDelay = 0.2f;
    [SerializeField]
    float reserveDeployDistance = 1.2f;
    [SerializeField] GameObject baseballBatPickupPrefab;
    [SerializeField] GameObject boomerangPickupPrefab;
    [SerializeField] GameObject boxingGlovePickupPrefab;
    [SerializeField] Transform reserveBubbleEndPoint;

    void Awake()
    {
        controller = GetComponent<PlayerController3D>();
        animator = GetComponentInChildren<PlayerAnimator>();
        combatShake = FindFirstObjectByType<CombatCameraShake>();
        playerController = GetComponent<PlayerController3D>();
        targetSelector = GetComponent<BoomerangTargetSelector>();
        groundSlamChargeUI = GetComponentInChildren<GroundSlamChargeUI>();
        spinMeterUI = GetComponentInChildren<SpinMeterUI>();
        PlayerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>();
        healthUIManager = FindFirstObjectByType<PlayerHealthUIManager>();
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
    }

    void Start()
    {
        UpdateWeaponVisuals();
        SetWeaponLayers();
        currentSpinTime = maxSpinTime;
    }

    void Update()
    {
        List<Enemy> expired = new List<Enemy>();

        foreach (var pair in enemyHitCooldowns)
        {
            if (Time.time >= pair.Value)
                expired.Add(pair.Key);
        }

        foreach (Enemy enemy in expired)
        {
            enemyHitCooldowns.Remove(enemy);
        }

        if (isBatSpinning)
        {
            SpinAttack();
        }

        if (isBatSpinning)
        {
            currentSpinTime -= Time.deltaTime;

            if (currentSpinTime <= 0f)
            {
                currentSpinTime = 0f;

                canSpin = false;

                EndBatSpin();
            }
        }
        else
        {
            currentSpinTime +=
                Time.deltaTime * (maxSpinTime / spinRechargeTime);

            currentSpinTime =
                Mathf.Clamp(currentSpinTime, 0f, maxSpinTime);
        }

        if (!canSpin && currentSpinTime >= maxSpinTime)
        {
            currentSpinTime = maxSpinTime;
            canSpin = true;

            Debug.Log("Spin Recharged!");
        }

        if (isChargingReserve)
        {
            reserveChargeTimer += Time.deltaTime;

            if (!reserveBubbleVisible &&
                reserveChargeTimer >= reserveBubbleDelay)
            {
                reserveBubbleVisible = true;

                currentReserveBubble = Instantiate( reserveBubblePrefab, bubbleSpawnPoint.position, Quaternion.identity);

                currentReserveBubble.transform.localScale = Vector3.zero;

                reserveBubbleVisible = true;

                controller.SetMovementLocked(true);
            }

            if (!reserveChargeComplete &&
                reserveChargeTimer >= reserveChargeTime)
            {
                reserveChargeComplete = true;

                Debug.Log("Bubble Fully Charged");
            }

            if (currentReserveBubble != null && reserveBubbleVisible && !reserveChargeComplete)
            {
                currentReserveBubble.transform.position = bubbleSpawnPoint.position;

                float growProgress = Mathf.InverseLerp(reserveBubbleDelay, reserveChargeTime, reserveChargeTimer);

                currentReserveBubble.transform.localScale = Vector3.one * growProgress;
            }
        }
    }

    public void StoreReserveWeapon(CombatTool tool)
    {
        hasReserveWeapon = true;
        reserveTool = tool;

        healthUIManager.SetPlayerReserveWeapon(GetComponent<PlayerHealth>(), reserveTool);

        Debug.Log("Reserve weapon is now: " + reserveTool);
    }

    public void ClearReserveWeapon()
    {
        hasReserveWeapon = false;
        reserveTool = CombatTool.Kick;

        healthUIManager.ClearPlayerReserveWeapon(GetComponent<PlayerHealth>());
    }

    public bool PickupWeapon(CombatTool weapon)
    {
        // If the player currently has Kick,
        // always equip the new weapon.
        if (currentTool == CombatTool.Kick)
        {
            PlayWeaponPickupAnimation(weapon);
            return true;
        }

        // Otherwise store (or replace) the reserve weapon.
        StoreReserveWeapon(weapon);

        return true;
    }

    IEnumerator ChargeReserveBubble()
    {
        yield return null;
    }

    void StartReserveCharge()
    {
        isChargingReserve = true;

        reserveChargeComplete = false;

        reserveBubbleVisible = false;

        reserveChargeTimer = 0f;
    }

    void ReleaseReserveCharge()
    {
        isChargingReserve = false;

        if (!reserveBubbleVisible)
        {
            StartCoroutine(KickRoutine());
        }
        else if (!reserveChargeComplete)
        {
            StartCoroutine(ShrinkReserveBubble());
        }
        else
        {
            StartCoroutine(DeployReserveBubble());
        }

        reserveBubbleVisible = false;
        reserveChargeComplete = false;
        reserveChargeTimer = 0f;
    }

    IEnumerator DeployReserveBubble()
    {
        Vector3 startPosition =
            currentReserveBubble.transform.position;

        Vector3 endPosition = reserveBubbleEndPoint.position;

        float duration = 0.45f;

        float height = 0.4f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            Vector3 position =
                Vector3.Lerp(startPosition, endPosition, t);

            position.y += Mathf.Sin(t * Mathf.PI) * height;

            currentReserveBubble.transform.position = position;

            yield return null;
        }

        currentReserveBubble.transform.position = endPosition;

        GameObject pickup = null;

        switch (reserveTool)
        {
            case CombatTool.BaseballBat:
                pickup = baseballBatPickupPrefab;
                break;

            case CombatTool.Boomerang:
                pickup = boomerangPickupPrefab;
                break;

            case CombatTool.BoxingGloves:
                pickup = boxingGlovePickupPrefab;
                break;
        }

        if (pickup != null)
        {
            Instantiate(
                pickup,
                reserveBubbleEndPoint.position,
                Quaternion.identity);
        }

        Destroy(currentReserveBubble);

        currentReserveBubble = null;

        ClearReserveWeapon();

        controller.SetMovementLocked(false);
    }

    IEnumerator ShrinkReserveBubble()
    {
        Vector3 startScale =
            currentReserveBubble.transform.localScale;

        float timer = 0f;

        while (timer < 0.2f)
        {
            timer += Time.deltaTime;

            currentReserveBubble.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    Vector3.zero,
                    timer / 0.2f);

            yield return null;
        }

        Destroy(currentReserveBubble);

        currentReserveBubble = null;

        controller.SetMovementLocked(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Current Tool = " + currentTool);
        Debug.Log($"Started:{context.started}  Performed:{context.performed}  Canceled:{context.canceled}");

        if (isBatSpinning)
            return;

        if (isAttacking)
            return;

        switch (currentTool)
        {
            case CombatTool.Kick:

                if (!hasReserveWeapon)
                {
                    if (context.canceled)
                    {
                        StartCoroutine(KickRoutine());
                    }

                    break;
                }

                if (context.started)
                {
                    StartReserveCharge();
                }

                if (context.canceled)
                {
                    ReleaseReserveCharge();
                }

                break;

            case CombatTool.BaseballBat:

                if (context.started)
                {
                    StartBatCharge();
                }

                if (context.performed)
                {
                    BeginBatSpin();
                }

                if (context.canceled)
                {
                    ReleaseBat();
                }

                break;

            case CombatTool.Boomerang:

                if (context.started)
                {
                    StartBoomerangCharge();
                }

                if (context.performed)
                {
                    BeginBoomerangAim();
                }

                if (context.canceled)
                {
                    ReleaseBoomerang();
                }

                break;

            case CombatTool.BoxingGloves:

                if (context.started)
                {
                    StartGloveCharge();
                }

                if (context.performed)
                {
                    // Ground slam later
                }

                if (context.canceled)
                {
                    ReleaseGloves();
                }

                break;
        }
    }

    IEnumerator WeaponPickupAnimation(CombatTool weapon)
    {
        controller.SetMovementLocked(true);

        Quaternion originalRotation = transform.rotation;

        // Small hop.
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            Vector3.up * pickupJumpForce,
            ForceMode.Impulse);
        pickupAudioSource.PlayOneShot(pickupJumpSFX);

        Material flashMaterial = null;

        switch (weapon)
        {
            case CombatTool.BaseballBat:
                flashMaterial = batPickupMat;
                break;

            case CombatTool.Boomerang:
                flashMaterial = boomerangPickupMat;
                break;

            case CombatTool.BoxingGloves:
                flashMaterial = glovesPickupMat;
                break;
        }

        // Let the player leave the ground.
        yield return new WaitForSeconds(0.20f);

        // Freeze them in the air.
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        //
        // Step 1 - Spin
        //
        float spinTime = 0.90f;

        float timer = 0f;

        Quaternion spinStart = transform.rotation;
        Quaternion spinEnd =
            spinStart * Quaternion.Euler(0f, 360f, 0f);

        bool flash1 = false;
        bool flash2 = false;
        bool flash3 = false;

        while (timer < spinTime)
        {
            timer += Time.deltaTime;

            float t = timer / spinTime;

            transform.rotation =
                Quaternion.Slerp(
                    spinStart,
                    spinEnd,
                    t);

            if (!flash1 && t >= 0.20f)
            {
                flash1 = true;
                SetPickupMaterials(flashMaterial);
                pickupAudioSource.PlayOneShot(pickupFlashSFX);
            }

            if (!flash2 && t >= 0.40f)
            {
                flash2 = true;
                RestoreMaterials();
                pickupAudioSource.PlayOneShot(pickupDongSFX);
            }

            if (!flash3 && t >= 0.60f)
            {
                flash3 = true;
                SetPickupMaterials(flashMaterial);
                pickupAudioSource.PlayOneShot(pickupFlashSFX);
            }

            yield return null;
        }

        transform.rotation = spinEnd;

        //
        // Step 2 - Face the camera
        //

        RestoreMaterials();
        CoopCameraController coopCam =
     FindFirstObjectByType<CoopCameraController>();

        Camera cam = null;

        if (coopCam != null)
        {
            cam = coopCam.GetCameraForPlayer(PlayerIndex);
        }

        if (cam == null)
        {
            Debug.LogWarning("Could not find the correct camera for weapon pickup.");
            yield break;
        }

        Vector3 direction =
            cam.transform.position -
            transform.position;

        direction.y = 0f;

        Quaternion cameraRotation =
            Quaternion.LookRotation(-direction);

        float faceCameraTime = 0.25f;

        timer = 0f;

        Quaternion currentRotation = transform.rotation;

        while (timer < faceCameraTime)
        {
            timer += Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(
                    currentRotation,
                    cameraRotation,
                    timer / faceCameraTime);

            yield return null;
        }

        transform.rotation = cameraRotation;

        //
        // Step 3 - Hold pose
        //
        animator.SetWeaponPickupPose(true);
        SpawnPickupStars();
        pickupAudioSource.PlayOneShot(pickupStarSFX);
        SetCombatTool(weapon);

        yield return new WaitForSeconds(1.00f);

        animator.SetWeaponPickupPose(false);

        RestoreMaterials();

        //
        // Step 4 - Face original direction
        //
        float rotateBackTime = 0.15f;

        timer = 0f;

        Quaternion startRotation = transform.rotation;

        while (timer < rotateBackTime)
        {
            timer += Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    originalRotation,
                    timer / rotateBackTime);

            yield return null;
        }

        transform.rotation = originalRotation;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        controller.SetMovementLocked(false);
    }

    void SpawnPickupStars()
    {
        if (pickupStarPrefab == null)
            return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

        // Top star
        SpawnStar(
            spawnPos,
            (Vector3.up + transform.forward * 0.25f).normalized);

        // Bottom left
        SpawnStar(
            spawnPos,
            (Vector3.left * 0.8f +
             Vector3.down * 0.4f +
             transform.forward * 0.3f).normalized);

        // Bottom right
        SpawnStar(
            spawnPos,
            (Vector3.right * 0.8f +
             Vector3.down * 0.4f +
             transform.forward * 0.3f).normalized);
    }

    void SpawnStar(Vector3 position, Vector3 direction)
    {
        GameObject star = Instantiate(
            pickupStarPrefab,
            position,
            Random.rotation);

        Rigidbody rb = star.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;

            rb.AddForce(
                direction * 5f,
                ForceMode.Impulse);

            rb.AddTorque(
                Random.onUnitSphere * 8f,
                ForceMode.Impulse);
        }

        Destroy(star, 1.5f);
    }

    public void PlayWeaponPickupAnimation(CombatTool weapon)
    {
        StartCoroutine(WeaponPickupAnimation(weapon));
    }

    void RestoreMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
        
    }

    void SetPickupMaterials(Material mat)
    {
        foreach (Renderer r in renderers)
        {
            Material[] mats = new Material[r.materials.Length];

            for (int i = 0; i < mats.Length; i++)
                mats[i] = mat;

            r.materials = mats;
        }
    }

    public void LoseWeapon()
    {
        StopAllCoroutines();

        // ---------- Reset combat state ----------
        isAttacking = false;

        isBatCharging = false;
        isBatSpinning = false;
        canChargeBat = true;

        isBoomerangCharging = false;
        isBoomerangAimMode = false;
        boomerangInFlight = false;

        isGloveCharging = false;
        slamCharged = false;
        isGroundSlamming = false;
        hasGroundSlamLanded = false;

        showGroundSlamUI = false;
        groundSlamChargeTimer = 0f;
        gloveChargeRoutine = null;

        animator.ResetAllAnimations();

        animator.SetWeaponPickupPose(false);

        batTrail.emitting = false;

        batAudio.Stop();
        batAudio.loop = false;
        batAudio.clip = null;
        batAudio.pitch = 1.3f;

        if (spinMeterUI != null)
            spinMeterUI.HideInstant();

        if (groundSlamChargeUI != null)
            groundSlamChargeUI.HideInstant();

        if (targetSelector != null)
            targetSelector.ResetBoomerang();

        playerController.SetBoomerangAim(false);

        SetCombatTool(CombatTool.Kick);
    }

    //=============== BAT ===================
    void StartBatCharge()
    {
        if (isAttacking)
            return;

        isBatCharging = true;

        animator.SetBatWindup(true);

        Debug.Log("Charging...");
    }

    void BeginBatSpin()
    {
        if (!canSpin)
            return;

        if (!canChargeBat)
            return;

        if (!isBatCharging)
            return;

        isBatCharging = false;
        isBatSpinning = true;
        batAudio.clip = batSwingSFX;
        batAudio.pitch = 1.5f;
        batAudio.loop = true;
        batAudio.Play();
        batTrail.emitting = true;
        animator.SetBatSpin(true);

        Debug.Log("Spin Started");
    }

    void ReleaseBat()
    {

        if (isBatCharging)
        {
            isBatCharging = false;

            animator.SetBatWindup(false);

            StartCoroutine(BatRoutine());
        }
    }

    void EndBatSpin()
    {
        isBatSpinning = false;

        animator.SetBatWindup(false);
        animator.SetBatSpin(false);
        batTrail.emitting = false;
        batAudio.Stop();
        batAudio.pitch = 1.3f;
        batAudio.loop = false;
        batAudio.clip = null;

        Debug.Log("Spin Ended");
    }

    void SpinAttack()
    {
        Collider[] hits = Physics.OverlapSphere(
            batSpinHitPoint.position,
            batSpinRadius);

        foreach (Collider hit in hits)
        {
            // Enemy
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 direction =
                    enemy.transform.position - transform.position;

                direction.y = 0f;
                direction.Normalize();

                if (!enemyHitCooldowns.ContainsKey(enemy))
                {
                    enemy.TakeBatHit(direction);

                    enemyHitCooldowns.Add(
                        enemy,
                        Time.time + 0.25f);
                }

                continue;
            }

            // Breakable Box
            BreakableBox box =
                hit.GetComponentInParent<BreakableBox>();

            if (box != null)
            {
                box.Break();
                continue;
            }

            // Player
            PlayerController3D player =
                hit.GetComponentInParent<PlayerController3D>();

            if (player != null &&
                player.gameObject != gameObject)
            {
                player.ApplyBatKnockback(transform.position);
            }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
                continue;
            }

            //---------- Boss Head ---------
            BossHead head = hit.GetComponentInParent<BossHead>();

            if (head != null)
            {
                BossController boss = head.GetComponentInParent<BossController>();

                if (boss != null)
                {
                    boss.DamageBoss();
                }

                continue;
            }
        }
    }

    public float SpinPercent
    {
        get
        {
            return currentSpinTime / maxSpinTime;
        }
    }

    public bool IsSpinning
    {
        get { return isBatSpinning; }
    }

    public bool CanSpin
    {
        get { return canSpin; }
    }

    //===================== BOOMERANG ===========================

    void StartBoomerangCharge()
    {
        if (boomerangInFlight)
            return;

        isBoomerangCharging = true;
        animator.SetBoomerangWindup(true);

        Debug.Log("Boomerang Charging");
    }

    void BeginBoomerangAim()
    {
        if (boomerangInFlight)
            return;

        if (!isBoomerangCharging)
            return;

        isBoomerangCharging = false;
        isBoomerangAimMode = true;
        playerController.SetBoomerangAim(true);
        targetSelector.BeginAim();
        Debug.Log("Aim Mode");
    }

    void ReleaseBoomerang()
    {
        if (isBoomerangCharging)
        {
            animator.SetBoomerangWindup(false);
            playerController.SetBoomerangAim(false);
            isBoomerangCharging = false;

            Debug.Log("Normal Throw");

            StartCoroutine(ThrowBoomerang());

            return;
        }

        if (isBoomerangAimMode)
        {
            targetSelector.EndAim();

            List<BoomerangTarget> targets =
                targetSelector.GetSelectedTargets();

            Debug.Log("Targets Selected: " + targets.Count);

            isBoomerangAimMode = false;

            playerController.SetBoomerangAim(false);

            animator.SetBoomerangWindup(false);

            if (targets.Count == 0)
            {
                StartCoroutine(ThrowBoomerang());
            }
            else
            {
                StartCoroutine(ThrowBoomerang(targets));
            }

            return;
        }
    }

    IEnumerator ThrowBoomerang()
    {
        animator.SetBoomerangWindup(true);

        yield return new WaitForSeconds(0.35f);

        animator.SetBoomerangWindup(false);
        animator.SetBoomerangThrow(true);

        if (boomerangInFlight)
        {
            animator.SetBoomerangThrow(false);
            yield break;
        }

        boomerangInFlight = true;

        boomerangObject.SetActive(false);

        Vector3 dir = -transform.forward;

        GameObject b = Instantiate(
            boomerangProjectilePrefab,
            transform.position + dir,
            Quaternion.identity);

        b.GetComponent<BoomerangProjectile>().Init(this, dir);

        yield return new WaitForSeconds(0.20f);

        animator.SetBoomerangThrow(false);
    }

    IEnumerator ThrowBoomerang(List<BoomerangTarget> targets)
    {
        animator.SetBoomerangWindup(true);

        yield return new WaitForSeconds(0.35f);

        animator.SetBoomerangWindup(false);
        animator.SetBoomerangThrow(true);

        if (boomerangInFlight)
        {
            animator.SetBoomerangThrow(false);
            yield break;
        }

        boomerangInFlight = true;

        boomerangObject.SetActive(false);

        Vector3 dir = -transform.forward;

        GameObject b = Instantiate(
     boomerangProjectilePrefab,
     transform.position + dir,
     Quaternion.identity);

        BoomerangProjectile projectile =
            b.GetComponent<BoomerangProjectile>();

        projectile.Init(this, dir, targets);

        yield return new WaitForSeconds(0.20f);

        animator.SetBoomerangThrow(false);
    }

    public void BoomerangReturned()
    {
        boomerangInFlight = false;
        boomerangObject.SetActive(true);
    }

    //======================== BOXING GLOVES ====================

    void StartGloveCharge()
    {
        if (isAttacking)
            return;

        isGloveCharging = true;
        slamCharged = false;
        groundSlamChargeTimer = 0f;
        animator.SetGloveWindup(true);
        showGroundSlamUI = false;
        StartCoroutine(ShowGroundSlamUICoroutine());
        gloveChargeRoutine =
            StartCoroutine(ChargeGlovesRoutine());
    }

    IEnumerator ShowGroundSlamUICoroutine()
    {
        yield return new WaitForSeconds(0.15f);

        if (isGloveCharging)
            showGroundSlamUI = true;
    }

    void ReleaseGloves()
    {
        if (!isGloveCharging)
            return;

        isGloveCharging = false;
        showGroundSlamUI = false;
        animator.SetGloveWindup(false);

        if (gloveChargeRoutine != null)
            StopCoroutine(gloveChargeRoutine);

        animator.SetRightPunch(punchRight);

        StartCoroutine(UppercutRoutine());
    }

    IEnumerator UppercutRoutine()
    {
        isAttacking = true;

        animator.SetUppercut(true);

        // One boost per airtime
        if (canUppercutLift)
        {
            canUppercutLift = false;

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z);

            rb.AddForce(
                Vector3.up * uppercutForce,
                ForceMode.Impulse);
        }

        // Wait until the fist reaches the hit frame
        yield return new WaitForSeconds(0.08f);

        if (glovePunchSFX != null)
        {
            glovePunchSFX.clip = punchRight
                ? rightPunchClip
                : leftPunchClip;

            glovePunchSFX.pitch = Random.Range(0.97f, 1.03f);
            glovePunchSFX.Play();
        }

        // Launch carried ball
        if (playerController.CarriedBall != null)
        {
            Vector3 launchDirection =
                (playerController.CarriedBall.transform.position - transform.position).normalized;

            playerController.CarriedBall.Launch(
                launchDirection,
                kickBallForce);
        }

        // Check everything inside the punch area
        Collider[] hits = Physics.OverlapSphere(
            gloveHitPoint.position,
            gloveRadius);

        foreach (Collider hit in hits)
        {
            // ---------- Enemy ----------
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 direction =
                    enemy.transform.position - transform.position;

                direction.y = 0f;
                direction.Normalize();

                // We'll make this function next
                enemy.TakeUppercut(direction);

                combatShake.Shake(0.10f, 0.12f);

                continue;
            }

            // ---------- Breakable Box ----------
            BreakableBox box =
                hit.GetComponentInParent<BreakableBox>();

            if (box != null)
            {
                box.Break();
                continue;
            }

            // ---------- Player ----------
            PlayerController3D player =
                hit.GetComponentInParent<PlayerController3D>();

            if (player != null && player.gameObject != gameObject)
            {
                // We'll make this function next
                player.ApplyUppercutKnockback(transform.position);

                combatShake.Shake(0.10f, 0.12f);

                continue;
            }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
                continue;
            }

            //---------- Boss Head ---------
            BossHead head = hit.GetComponentInParent<BossHead>();

            if (head != null)
            {
                BossController boss = head.GetComponentInParent<BossController>();

                if (boss != null)
                {
                    boss.DamageBoss();
                }

                continue;
            }
        }

        // Finish the animation
        yield return new WaitForSeconds(0.17f);

        animator.SetUppercut(false);

        yield return new WaitForSeconds(0.20f);

        punchRight = !punchRight;
        animator.SetRightPunch(punchRight);

        isAttacking = false;
    }

    IEnumerator ChargeGlovesRoutine()
    {
        groundSlamChargeTimer = 0f;

        while (groundSlamChargeTimer < gloveChargeTime)
        {
            groundSlamChargeTimer += Time.deltaTime;
            yield return null;
        }

        groundSlamChargeTimer = gloveChargeTime;

        slamCharged = true;
        isGloveCharging = false;

        animator.SetGloveWindup(false);
        showGroundSlamUI = false;
        StartCoroutine(GroundSlamRoutine());
    }

    IEnumerator GroundSlamRoutine()
    {
        isAttacking = true;

        isGroundSlamming = true;
        hasGroundSlamLanded = false;

        animator.SetGroundSlamJump(true);

        yield return null;

        playerController.BeginGroundSlamLeap();

        while (!hasGroundSlamLanded)
            yield return null;

        yield return new WaitForSeconds(0.25f);

        animator.SetGroundSlamImpact(false);

        isGroundSlamming = false;
        isAttacking = false;
    }

    public void ResetUppercutLift()
    {
        canUppercutLift = true;
    }

    public bool IsGroundSlamming()
    {
        return isGroundSlamming;
    }

    public void LaunchGroundSlam()
    {
        rb.linearVelocity = Vector3.zero;

        Vector3 leap =
            (-transform.forward * slamLeapForwardForce) +
            (Vector3.up * slamLeapUpForce);

        rb.AddForce(leap, ForceMode.Impulse);
    }

    public void OnGroundSlamLanded()
    {
        if (!isGroundSlamming)
            return;

        if (hasGroundSlamLanded)
            return;

        hasGroundSlamLanded = true;

        animator.SetGroundSlamJump(false);
        animator.SetGroundSlamImpact(true);

        GroundSlamAttack();
        
        StartCoroutine(GroundSlamDustWave());
        combatShake.Shake(0.18f, 0.25f);
        if (groundSlamSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(groundSlamSFX);
        }
    }

    void GroundSlamAttack()
    {
        Collider[] hits = Physics.OverlapSphere(
            slamHitPoint.position,
            slamRadius);

        foreach (Collider hit in hits)
        {
            // ---------- Enemy ----------
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 direction =
                    enemy.transform.position - transform.position;

                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                    direction.Normalize();

                enemy.TakeUppercut(direction);

                continue;
            }

            // ---------- Breakable Box ----------
            BreakableBox box =
                hit.GetComponentInParent<BreakableBox>();

            if (box != null)
            {
                box.Break();

                continue;
            }

            // ---------- Player ----------
            PlayerController3D player =
                hit.GetComponentInParent<PlayerController3D>();

            if (player != null &&
                player.gameObject != gameObject)
            {
                player.ApplyUppercutKnockback(transform.position);

                continue;
            }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
                continue;
            }

            //---------- Boss Head ---------
            BossHead head = hit.GetComponentInParent<BossHead>();

            if (head != null)
            {
                BossController boss = head.GetComponentInParent<BossController>();

                if (boss != null)
                {
                    boss.DamageBoss();
                }

                continue;
            }

        }
    }

    IEnumerator GroundSlamDustWave()
    {
        const int dustCount = 250;

        for (int i = 0; i < dustCount; i++)
        {
            Vector2 random = Random.insideUnitCircle * slamRadius;

            Vector3 spawnPos =
                slamHitPoint.position +
                new Vector3(
                    random.x,
                    0f,
                    random.y);

            GroundSlamDustPool.Instance.Spawn(
                spawnPos,
                Vector3.zero);
        }

        yield break;
    }

    public float GroundSlamChargePercent
    {
        get
        {
            return groundSlamChargeTimer / gloveChargeTime;
        }
    }

    public bool IsChargingGroundSlam
    {
        get
        {
            return isGloveCharging;
        }
    }

    public bool CanGroundSlam
    {
        get
        {
            return slamCharged;
        }
    }

    public bool ShowGroundSlamUI
    {
        get
        {
            return showGroundSlamUI;
        }
    }

    //======================== COROUTINES =======================
    IEnumerator KickRoutine()
    {
        isAttacking = true;
        Debug.Log("Kick started");

        animator.SetKick(true);
        Debug.Log("Kick Point: " + kickPoint);
        // Wind-up
        yield return new WaitForSeconds(0.10f);

        if (playerController.CarriedBall != null)
        {
            Vector3 launchDirection = (playerController.CarriedBall.transform.position - transform.position).normalized;

            playerController.CarriedBall.Launch(launchDirection,kickBallForce);

        }

        // Check everything inside the kick area
        Collider[] hits = Physics.OverlapSphere(kickPoint.position, kickRadius);

        foreach (Collider hit in hits)
        {
            // ---------- Enemy ----------
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 direction =
                    enemy.transform.position - transform.position;

                direction.y = 0f;
                direction.Normalize();

                enemy.TakeKick(direction);

                continue;
            }

            // ---------- Breakable Box ----------
            BreakableBox box =
                hit.GetComponentInParent<BreakableBox>();

            if (box != null)
            {
                box.Break();
                continue;
            }

            // ---------- Player ----------
            PlayerController3D player =
                hit.GetComponentInParent<PlayerController3D>();

            if (player != null && player.gameObject != gameObject)
            {
                player.ApplyKickKnockback(transform.position);

                continue;
            }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
                continue;
            }

            //---------- Boss Head ---------
            BossHead head = hit.GetComponentInParent<BossHead>();

            if (head != null)
            {
                BossController boss = head.GetComponentInParent<BossController>();

                if (boss != null)
                {
                    boss.DamageBoss();
                }

                continue;
            }

        }

        animator.SetKick(false);
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    IEnumerator BatRoutine()
    {
        isAttacking = true;
        canChargeBat = false;
        batAudio.pitch = 1.3f;
        batAudio.PlayOneShot(batSwingSFX);
        animator.SetBatWindup(true);
        batTrail.emitting = true;
        yield return new WaitForSeconds(0.12f);

        if (playerController.CarriedBall != null)
        {
            Vector3 launchDirection = (playerController.CarriedBall.transform.position - transform.position).normalized;

            playerController.CarriedBall.Launch(launchDirection,batBallForce);
        }

        
        
            Collider[] hits = Physics.OverlapSphere(batHitPoint.position,batRadius);

            foreach (Collider hit in hits)
            {
                // ---------- Enemy ----------
                Enemy enemy = hit.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    Vector3 direction =
                        enemy.transform.position - transform.position;

                    direction.y = 0f;
                    direction.Normalize();

                    enemy.TakeBatHit(direction);
                    combatShake.Shake(0.10f, 0.12f);
                    continue;
                }

                // ---------- Breakable Box ----------
                BreakableBox box =
                    hit.GetComponentInParent<BreakableBox>();

                if (box != null)
                {
                    box.Break();
                    continue;
                }

                // ---------- Player ----------
                PlayerController3D player =
                    hit.GetComponentInParent<PlayerController3D>();

                if (player != null && player.gameObject != gameObject)
                {
                    player.ApplyBatKnockback(transform.position);
                    combatShake.Shake(0.10f, 0.12f);
                    continue;
                }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
                continue;
            }

            //---------- Boss Head ---------
            BossHead head = hit.GetComponentInParent<BossHead>();

            if (head != null)
            {
                BossController boss = head.GetComponentInParent<BossController>();

                if (boss != null)
                {
                    boss.DamageBoss();
                }

                continue;
            }

            // ---------- Enemy Projectile ----------
            EnemyProjectile projectile =hit.GetComponentInParent<EnemyProjectile>();

                 if (projectile != null)
                 {
                
                
                if (projectileReflectSFX != null)
                projectileReflectSFX.Play();
                projectile.Reflect();
                    continue;
                 }

            }
        animator.SetBatWindup(false);

        animator.SetBatFollowThrough(true);

      
        yield return new WaitForSeconds(0.12f);

        animator.SetBatFollowThrough(false);
        batTrail.emitting = false;
        yield return new WaitForSeconds(0.15f);

        isAttacking = false;

        yield return new WaitForSeconds(0.2f);

        canChargeBat = true;
    }


    public void SetCombatTool(CombatTool tool)
    {
        currentTool = tool;

        UpdateWeaponVisuals();

        if (healthUIManager != null)
        {
            PlayerHealth health = GetComponent<PlayerHealth>();

            healthUIManager.SetPlayerWeapon(
                health,
                currentTool);
        }
    }

    void UpdateWeaponVisuals()
    {
        baseballBatObject.SetActive(false);
        boomerangObject.SetActive(false);
        boxingGlovesObject1.SetActive(false);
        boxingGlovesObject2.SetActive(false);

        switch (currentTool)
        {
            case CombatTool.BaseballBat:
                baseballBatObject.SetActive(true);
                break;

            case CombatTool.Boomerang:
                boomerangObject.SetActive(true);
                break;

            case CombatTool.BoxingGloves:
                boxingGlovesObject1.SetActive(true);
                boxingGlovesObject2.SetActive(true);
                break;
        }
    }

    void SetWeaponLayers()
    {
        int playerLayer = gameObject.layer;

        SetLayerRecursively(baseballBatObject, playerLayer);
        SetLayerRecursively(boomerangObject, playerLayer);
        SetLayerRecursively(boxingGlovesObject1, playerLayer);
        SetLayerRecursively(boxingGlovesObject2 , playerLayer);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null)
            return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (kickPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            kickPoint.position,
            kickRadius);

        if (slamHitPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slamHitPoint.position, slamRadius);

    }



    public int PlayerIndex
    {
        get
        {
            return PlayerInput.playerIndex;
        }
    }
}