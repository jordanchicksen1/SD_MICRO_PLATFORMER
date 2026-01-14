using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float speed = 2f;

    Vector3 target;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
            col.transform.SetParent(transform);
    }

    void OnCollisionExit(Collision col)
    {
        if (col.transform.CompareTag("Player"))
            col.transform.SetParent(null);
    }
}
