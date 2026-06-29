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

    [Header("Kick")]
    [SerializeField] Transform kickPoint;
    [SerializeField] float kickRadius = 1f;
    [SerializeField] LayerMask enemyLayer;

    void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
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
        Debug.Log("Kick started");

        animator.SetKick(true);
        Debug.Log("Kick Point: " + kickPoint);
        // Wind-up
        yield return new WaitForSeconds(0.10f);

        // Check everything inside the kick area
        Collider[] hits = Physics.OverlapSphere(
            kickPoint.position,
            kickRadius,
            enemyLayer);

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeHit();
            }
        }

        animator.SetKick(false);
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