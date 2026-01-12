using UnityEngine;

public class EnemyStompDetector : MonoBehaviour
{
    [SerializeField] float stompTolerance = 0.05f;

    Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        Collider playerCol = player.GetComponent<Collider>();

        // Get player's feet position
        float playerFeetY = playerCol.bounds.min.y;

        // Get stomp trigger position (enemy head)
        float stompY = transform.position.y;

        bool stomped = playerFeetY > stompY - stompTolerance;
        bool groundPound = player.IsGroundPounding();

        if (stomped || groundPound)
        {
            enemy.TakeHit();
            BouncePlayer(rb);
        }
    }

    void BouncePlayer(Rigidbody rb)
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            7f,
            rb.linearVelocity.z
        );
    }
}
