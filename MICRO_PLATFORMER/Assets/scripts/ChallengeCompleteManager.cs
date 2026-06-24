using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeCompletionManager : MonoBehaviour
{
    [SerializeField] string hubSceneName = "HubWorld";
    [SerializeField] AudioSource victorySFX;

    DoorCameraFocus cameraFocus;
    GameObject challengeCompleteUI;
    ScreenFader screenFader;

    bool challengeFinished;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
        screenFader = FindFirstObjectByType<ScreenFader>();

        ChallengeCompleteUI ui =
            FindFirstObjectByType<ChallengeCompleteUI>(
                FindObjectsInactive.Include
            );

        if (ui != null)
        {
            challengeCompleteUI = ui.gameObject;
            Debug.Log("Found UI: " + challengeCompleteUI.name);
        }
        else
        {
            Debug.LogError("Challenge UI NOT FOUND");
        }
    }

    public void CompleteChallenge(PlayerController3D player)
    {
        if (challengeFinished) return;

        challengeFinished = true;

        StartCoroutine(CompleteRoutine(player));
    }

    IEnumerator CompleteRoutine(PlayerController3D player)
    {
        if (cameraFocus)
            cameraFocus.FocusOn(player.ChallengeFocusPoint);

        // Show immediately
        Debug.Log("Trying to show UI");

        if (challengeCompleteUI)
        {
            Debug.Log("UI FOUND");
            challengeCompleteUI.SetActive(true);
            Debug.Log("Panel active: " + challengeCompleteUI.activeSelf);
        }
        else
        {
            Debug.LogError("UI IS NULL");
        }

        if (victorySFX)
            victorySFX.Play();

        yield return new WaitForSeconds(3f);

    // Save currency
    RunCurrency.Instance?.CommitToBank();

        // Save gems permanently
        if (RunLevelInfo.Instance != null &&
            PersistentGemProgress.Instance != null)
        {
            PersistentGemProgress.Instance.CommitPendingFromRun(
                RunLevelInfo.Instance.LevelId
            );
        }

        // Fade out first
        if (screenFader)
        {
            yield return screenFader.FadeTo(1f, 0.5f);
        }

        SceneManager.LoadScene(hubSceneName);
    }
}