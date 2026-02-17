using UnityEngine;

public class BlueCoinChallengeButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BlueCoinChallenge challenge;
    [SerializeField] ButtonVisual visual;
    [SerializeField] AudioSource buttonSFX;

    [Header("Optional")]
    [SerializeField] bool oneTimeOnly = true;

    bool used;


    void Awake()
    {
        if (visual == null)
            visual = GetComponent<ButtonVisual>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (oneTimeOnly && used) return;

        used = true;

        visual?.Press();
        buttonSFX?.Play();

        challenge?.StartChallenge();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        visual?.Release();
    }

    void OnEnable()
    {
        if (challenge != null)
            challenge.OnChallengeReset += ResetButton;
    }

    void OnDisable()
    {
        if (challenge != null)
            challenge.OnChallengeReset -= ResetButton;
    }

    void ResetButton()
    {
        used = false;
    }

}
