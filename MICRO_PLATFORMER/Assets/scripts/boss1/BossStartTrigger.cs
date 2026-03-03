using UnityEngine;

public class BossStartTrigger : MonoBehaviour
{
    [SerializeField] BossController boss;

    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag("Player")) return;

        triggered = true;

        boss.StartFight();

        Debug.Log("Boss Trigger Activated!");
    }
}