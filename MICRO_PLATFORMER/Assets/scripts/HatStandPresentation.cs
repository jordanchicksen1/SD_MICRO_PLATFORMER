using UnityEngine;

public class HatStandPresentation : MonoBehaviour
{
    [Header("Gameplay Visuals")]
    HubPlayerController3D player;
    HubFollower follower;
    GameObject playerVisualRoot;
    GameObject followerVisualRoot;

    [Header("Presentation")]
    [SerializeField] GameObject mannequinPlayer;
    [SerializeField] GameObject mannequinFollower;

    void Awake()
    {
        mannequinPlayer.SetActive(false);
        mannequinFollower.SetActive(false);
    }

    public void BeginPresentation()
    {
        player = FindFirstObjectByType<HubPlayerController3D>();
        follower = FindFirstObjectByType<HubFollower>();

        playerVisualRoot =
            player.transform.Find("model").gameObject;

        followerVisualRoot =
            follower.transform.Find("model").gameObject;

        playerVisualRoot.SetActive(false);
        followerVisualRoot.SetActive(false);

        mannequinPlayer.SetActive(true);
        mannequinFollower.SetActive(true);
    }

    public void EndPresentation()
    {
        mannequinPlayer.SetActive(false);
        mannequinFollower.SetActive(false);

        playerVisualRoot.SetActive(true);
        followerVisualRoot.SetActive(true);
    }
}