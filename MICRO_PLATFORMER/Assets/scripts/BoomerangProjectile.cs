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

    }
}