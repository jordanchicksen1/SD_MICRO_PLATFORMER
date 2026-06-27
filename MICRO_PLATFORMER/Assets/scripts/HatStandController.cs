using System.Collections;
using UnityEngine;

public class HatStandController : MonoBehaviour
{
    [SerializeField] Transform player1StandPoint;
    [SerializeField] Transform player2StandPoint;

    HubPlayerController3D player;
    HubFollower follower;
    Rigidbody playerRb;
    Rigidbody followerRb;

    Vector3 playerStartPos;
    Quaternion playerStartRot;

    Vector3 followerStartPos;
    Quaternion followerStartRot;

    public void BeginPresentation(
        HubPlayerController3D p,
        HubFollower f)
    {
        player = p;
        follower = f;

        playerRb = player.GetComponent<Rigidbody>();
        followerRb = follower.GetComponent<Rigidbody>();

        playerStartPos = p.transform.position;
        playerStartRot = p.transform.rotation;

        followerStartPos = f.transform.position;
        followerStartRot = f.transform.rotation;

        PresentCharacters();
    }

    void PresentCharacters()
    {
        // Stop physics
        if (playerRb)
            playerRb.isKinematic = true;

        if (followerRb)
            followerRb.isKinematic = true;

        // Stop follower AI
        follower.enabled = false;

        // Move instantly
        player.transform.SetPositionAndRotation(
            player1StandPoint.position,
            player1StandPoint.rotation);

        follower.transform.SetPositionAndRotation(
            player2StandPoint.position,
            player2StandPoint.rotation);
    }

    public void EndPresentation()
    {
        player.transform.SetPositionAndRotation(
            playerStartPos,
            playerStartRot);

        follower.transform.SetPositionAndRotation(
            followerStartPos,
            followerStartRot);

        if (playerRb)
            playerRb.isKinematic = false;

        if (followerRb)
            followerRb.isKinematic = false;

        follower.enabled = true;
    }

   
}