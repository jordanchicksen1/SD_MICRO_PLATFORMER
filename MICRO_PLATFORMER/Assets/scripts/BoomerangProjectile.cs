using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    PlayerCombat owner;

    Vector3 startPosition;
    Vector3 direction;

    [SerializeField] float speed = 20f;
    [SerializeField] float maxDistance = 10f;

    bool returning;

    public void Init(PlayerCombat combat, Vector3 throwDirection)
    {
        owner = combat;

        startPosition = transform.position;

        direction = throwDirection.normalized;
    }

    void Update()
    {
        if (!returning)
        {
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                returning = true;
            }
        }
        else
        {
            Vector3 returnDir =
                (owner.transform.position - transform.position).normalized;

            transform.position +=
                returnDir * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, owner.transform.position) < 1f)
            {
                owner.BoomerangReturned();

                Destroy(gameObject);
            }
        }

        transform.Rotate(0f, 1080f * Time.deltaTime, 0f);
    }
}