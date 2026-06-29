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
    [SerializeField] GameObject kickHitboxPrefab;

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

        yield return new WaitForSeconds(0.10f);

        Debug.Log("Spawning hitbox");

        GameObject hitbox =
            Instantiate(
                kickHitboxPrefab,
                kickPoint.position,
                kickPoint.rotation);

        Debug.Log("Hitbox spawned: " + hitbox.name);

        yield return new WaitForSeconds(0.5f);

        Destroy(hitbox);

        animator.SetKick(false);
    }
}