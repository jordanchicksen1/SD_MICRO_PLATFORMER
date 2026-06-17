using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeCompletionManager : MonoBehaviour
{
    [SerializeField] string hubSceneName = "HubWorld";
    [SerializeField] AudioSource victorySFX;

    DoorCameraFocus cameraFocus;
    GameObject challengeCompleteUI;

    bool challengeFinished;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();

        ChallengeCompleteUI ui =
            FindFirstObjectByType<ChallengeCompleteUI>();

        if (ui != null)
            challengeCompleteUI = ui.gameObject;
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

        yield return new WaitForSeconds(1.3f);

        if (challengeCompleteUI)
            challengeCompleteUI.SetActive(true);

        if (victorySFX)
            victorySFX.Play();

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(hubSceneName);
    }
}