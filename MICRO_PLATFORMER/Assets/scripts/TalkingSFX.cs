using UnityEngine;

public class TalkingSFX : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] talkingClips;

    [Header("Pitch")]
    [SerializeField] float minPitch = 0.9f;
    [SerializeField] float maxPitch = 1.15f;

    [Header("Chance")]
    [Range(0f, 1f)]
    [SerializeField] float playChance = 0.35f;
    float nextTalkTime;
    int lastClipIndex = -1;
    public void TryPlaySound(char character)
    {
        if (Time.unscaledTime < nextTalkTime)
            return; 

        // Don't make noises for spaces.
        if (character == ' ')
            return;

        // Random chance so it doesn't become machine-gun audio.
        if (Random.value > playChance)
            return;

        if (talkingClips.Length == 0)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        int clipIndex;

        do
        {
            clipIndex = Random.Range(0, talkingClips.Length);
        }
        while (talkingClips.Length > 1 && clipIndex == lastClipIndex);

        lastClipIndex = clipIndex;

        AudioClip clip = talkingClips[clipIndex];

        nextTalkTime = Time.unscaledTime + Random.Range(0.09f, 0.16f);
        audioSource.PlayOneShot(clip);
    }
}