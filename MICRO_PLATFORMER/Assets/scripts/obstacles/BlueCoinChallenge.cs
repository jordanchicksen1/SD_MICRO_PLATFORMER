using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCoinChallenge : MonoBehaviour
{
    [Header("Challenge Settings")]
    [SerializeField] float timeLimit = 10f;
    [SerializeField] GameObject gemRewardPrefab;
    [SerializeField] Transform gemSpawnPoint;

    [Header("Blue Coins")]
    [SerializeField] List<GameObject> blueCoinPrefabs;
    [SerializeField] List<Transform> spawnPoints;

    [Header("UI")]
    [SerializeField] GameObject timerUIRoot;
    [SerializeField] TMPro.TextMeshProUGUI timerText;

    [Header("Blink Settings")]
    [SerializeField] float blinkStartTime = 3f;
    [SerializeField] float blinkSpeed = 8f;

    [Header("Timer Audio")]
    [SerializeField] AudioSource timerAudioSource;
    [SerializeField] AudioClip slowTimerClip;  // 7 sec
    [SerializeField] AudioClip fastTimerClip;  // 3 sec

    bool fastPhaseStarted;

    List<GameObject> spawnedCoins = new();
    int coinsCollected;
    bool challengeActive;

    Coroutine timerRoutine;

    public System.Action OnChallengeReset;

    public void StartChallenge()
    {
        if (challengeActive) return;

        challengeActive = true;
        coinsCollected = 0;
        fastPhaseStarted = false;

        SpawnCoins();
        timerRoutine = StartCoroutine(TimerRoutine());

        // Start slow timer sound
        if (timerAudioSource && slowTimerClip)
        {
            timerAudioSource.Stop();
            timerAudioSource.clip = slowTimerClip;
            timerAudioSource.loop = false;
            timerAudioSource.Play();
        }
    }

    void SpawnCoins()
    {
        spawnedCoins.Clear();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject coin = Instantiate(
                blueCoinPrefabs[i],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            BlueCoin bc = coin.GetComponent<BlueCoin>();
            bc.SetManager(this);

            spawnedCoins.Add(coin);
        }
    }

    public void RegisterCoinCollected()
    {
        if (!challengeActive) return;

        coinsCollected++;

        if (coinsCollected >= spawnPoints.Count)
        {
            ChallengeSuccess();
        }
    }

    IEnumerator TimerRoutine()
    {
        float timer = timeLimit;

        if (timerUIRoot) timerUIRoot.SetActive(true);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timerText)
                timerText.text = Mathf.CeilToInt(timer).ToString();

            // Switch to fast ticking at 3 seconds remaining
            if (!fastPhaseStarted && timer <= 3f)
            {
                fastPhaseStarted = true;

                if (timerAudioSource && fastTimerClip)
                {
                    timerAudioSource.Stop();
                    timerAudioSource.clip = fastTimerClip;
                    timerAudioSource.loop = false;
                    timerAudioSource.Play();
                }
            }

            // Blink coins near timeout
            if (timer <= blinkStartTime)
            {
                foreach (var coin in spawnedCoins)
                {
                    if (coin != null)
                    {
                        bool visible = Mathf.Sin(Time.time * blinkSpeed) > 0f;
                        coin.SetActive(visible);
                    }
                }
            }

            yield return null;
        }

        if (timerUIRoot) timerUIRoot.SetActive(false);

        ChallengeFail();
    }

    void ChallengeSuccess()
    {
        StopTimerAudio();
        StopTimer();
        ClearCoins();

        if (gemRewardPrefab && gemSpawnPoint)
            Instantiate(gemRewardPrefab, gemSpawnPoint.position,gemRewardPrefab.transform.rotation);

        challengeActive = false;

        if (timerUIRoot) timerUIRoot.SetActive(false);
    }

    void ChallengeFail()
    {
        StopTimerAudio();
        StopTimer();
        ClearCoins();

        challengeActive = false;

        OnChallengeReset?.Invoke();

        if (timerUIRoot) timerUIRoot.SetActive(false);
    }

    void StopTimer()
    {
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
    }

    void StopTimerAudio()
    {
        if (timerAudioSource && timerAudioSource.isPlaying)
            timerAudioSource.Stop();
    }

    void ClearCoins()
    {
        foreach (var coin in spawnedCoins)
        {
            if (coin != null)
                Destroy(coin);
        }

        spawnedCoins.Clear();
    }
}
