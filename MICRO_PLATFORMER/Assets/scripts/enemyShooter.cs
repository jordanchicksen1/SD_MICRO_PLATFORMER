using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float patrolDistance = 3f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float shootInterval = 2f;
    [SerializeField] float detectionRange = 12f;
    public AudioSource shootSFX;

    Rigidbody rb;
    Transform target;
    Vector3 startPos;
    float shootTimer;
    int direction = 1;


    EnemyBipedAnimator animator;
    Enemy enemy;

    void Start()
    {
        animator = GetComponentInChildren<EnemyBipedAnimator>();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        enemy = GetComponent<Enemy>();
    }


    void Update()
    {
        if (enemy != null && enemy.IsDead)
            return;

        FindTarget();

        if (target != null)
            FaceTarget();

        HandleShooting();
    }



    void FixedUpdate()
    {
        if (enemy != null && enemy.IsDead)
            return;

        if (animator != null && animator.IsShooting())
            return;

        Patrol();
    }


    void Patrol()
    {
        float offset = transform.position.x - startPos.x;

        if (Mathf.Abs(offset) > patrolDistance)
            direction *= -1;

        rb.linearVelocity = new Vector3(
            direction * moveSpeed,
            rb.linearVelocity.y,
            0
        );
    }

    void FaceTarget()
    {
        if (target == null)
            return;

        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f; // prevent tilting

        if (lookDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = targetRot;
    }


    void FindTarget()
    {
        float closest = detectionRange;
        target = null;

        foreach (PlayerController3D player in FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None))
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = player.transform;
            }
        }
    }

    void HandleShooting()
    {
        if (enemy != null && enemy.IsDead)
            return;

        if (target == null)
            return;

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f)
            return;

        shootTimer = shootInterval;

        if (animator != null)
            animator.PlayShoot();

        StartCoroutine(SpawnProjectileDelayed(0.3f));
    }


    IEnumerator SpawnProjectileDelayed(float delay)
    {
        // Stop movement while shooting
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(delay);

        if (target == null)
            yield break;

        // Spawn projectile
        Vector3 dir = (target.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        ep.Init(dir);
        shootSFX.Play();

        // Ignore collision with self
        Collider projCol = proj.GetComponent<Collider>();
        Collider enemyCol = GetComponent<Collider>();
        if (projCol != null && enemyCol != null)
            Physics.IgnoreCollision(projCol, enemyCol);
    }



}
