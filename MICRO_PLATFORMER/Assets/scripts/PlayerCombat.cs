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

    public CombatTool CurrentTool => currentTool;


    PlayerAnimator animator;
    CombatCameraShake combatShake;
    PlayerController3D playerController;
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

    [Header("Boomerang")]
    [SerializeField] GameObject boomerangProjectilePrefab;
    bool boomerangInFlight;

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

    void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimator>();
        combatShake = FindFirstObjectByType<CombatCameraShake>();
        playerController = GetComponent<PlayerController3D>();
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
                StartCoroutine(KickRoutine());
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

                Debug.Log("Before Throw");

                ThrowBoomerang();

                Debug.Log("After Throw");

                break;

            case CombatTool.BoxingGloves:
                Debug.Log("Punch!");
                break;
        }
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

    void ThrowBoomerang()
    {
        Debug.Log("INSIDE THROW");
        boomerangObject.SetActive(false);
        if (boomerangInFlight)
        {
            Debug.Log("Already in flight");
            return;
        }

        boomerangInFlight = true;

        Debug.Log("Prefab = " + boomerangProjectilePrefab);

        Vector3 dir = -transform.forward;

        GameObject b = Instantiate(
            boomerangProjectilePrefab,
            transform.position + dir,
            Quaternion.identity);

        Debug.Log("Spawned = " + b.name);

        b.GetComponent<BoomerangProjectile>().Init(this, dir);
    }

    public void BoomerangReturned()
    {
        boomerangInFlight = false;
        boomerangObject.SetActive(true);
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
    }
}