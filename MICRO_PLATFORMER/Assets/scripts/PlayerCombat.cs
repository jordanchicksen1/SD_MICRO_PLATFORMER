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
    bool isAttacking;

    [Header("Kick")]
    [SerializeField] Transform kickPoint;
    [SerializeField] float kickRadius = 1f;
   

    void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isAttacking)
            return;

        switch (currentTool)
        {
            case CombatTool.Kick:
                StartCoroutine(KickRoutine());
                break;

            case CombatTool.BaseballBat:
                Debug.Log("Bat!");
                break;

            case CombatTool.Boomerang:
                Debug.Log("Boomerang!");
                break;

            case CombatTool.BoxingGloves:
                Debug.Log("Punch!");
                break;
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