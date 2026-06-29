using UnityEngine;

public class KickHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.Log("No Enemy component.");
            return;
        }

        Debug.Log("Enemy Found!");

        enemy.TakeHit();
    }
}