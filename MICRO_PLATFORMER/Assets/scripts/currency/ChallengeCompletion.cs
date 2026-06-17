using UnityEngine;

public class ChallengeCompletion : MonoBehaviour
{
    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        PlayerController3D player =
            other.GetComponentInParent<PlayerController3D>();

        if (!player) return;

        triggered = true;

        ChallengeCompletionManager manager =
            FindFirstObjectByType<ChallengeCompletionManager>();

        if (manager)
        {
            manager.CompleteChallenge(player);
        }
    }
}