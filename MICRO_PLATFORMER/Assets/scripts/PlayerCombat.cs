using UnityEngine;
using System.Collections;
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
    bool isAttacking;
    bool isBatCharging;
    bool isBatSpinning;

    [Header("Kick")]
    [SerializeField] Transform kickPoint;
    [SerializeField] float kickRadius = 1f;

    [Header("Baseball Bat")]
    [SerializeField] Transform batHitPoint;
    [SerializeField] float batRadius = 1.8f;

    [Header("Weapon Models")]
    [SerializeField] GameObject baseballBatObject;
    [SerializeField] GameObject boomerangObject;
    [SerializeField] GameObject boxingGlovesObject1;
    [SerializeField] GameObject boxingGlovesObject2;


    void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimator>();
        combatShake = FindFirstObjectByType<CombatCameraShake>();
    }

    void Start()
    {
        UpdateWeaponVisuals();
        SetWeaponLayers();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
       

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
                Debug.Log("Boomerang!");
                break;

            case CombatTool.BoxingGloves:
                Debug.Log("Punch!");
                break;
        }
    }

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
        if (!isBatCharging)
            return;

        isBatCharging = false;
        isBatSpinning = true;

        Debug.Log("Spin Started");
    }

    void ReleaseBat()
    {
        if (isBatSpinning)
        {
            isBatSpinning = false;

            animator.SetBatWindup(false);

            Debug.Log("Spin Ended");

            return;
        }

        if (isBatCharging)
        {
            isBatCharging = false;

            animator.SetBatWindup(false);

            StartCoroutine(BatRoutine());
        }
    }

    IEnumerator KickRoutine()
    {
        isAttacking = true;
        Debug.Log("Kick started");

        animator.SetKick(true);
        Debug.Log("Kick Point: " + kickPoint);
        // Wind-up
        yield return new WaitForSeconds(0.10f);

        // Check everything inside the kick area
        Collider[] hits =
     Physics.OverlapSphere(
         kickPoint.position,
         kickRadius);

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

        animator.SetBatWindup(true);

        yield return new WaitForSeconds(0.12f);

        animator.SetBatWindup(false);

        animator.SetBatFollowThrough(true);

        Collider[] hits =
    Physics.OverlapSphere(
        batHitPoint.position,
        batRadius);

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
        }

       

        yield return new WaitForSeconds(0.12f);

        animator.SetBatFollowThrough(false);

        yield return new WaitForSeconds(0.15f);

        isAttacking = false;
    }

    IEnumerator BatSpinRoutine()
    {
        isAttacking = true;

       // animator.SetBatSpin(true);

        yield return new WaitForSeconds(5f);

        //animator.SetBatSpin(false);

        isAttacking = false;
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