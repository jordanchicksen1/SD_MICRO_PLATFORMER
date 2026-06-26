using System.Collections;
using UnityEngine;

public class HatStandController : MonoBehaviour
{
    [SerializeField] Transform player1StandPoint;
    [SerializeField] Transform player2StandPoint;

    HubPlayerController3D player;
    HubFollower follower;

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

        playerStartPos = p.transform.position;
        playerStartRot = p.transform.rotation;

        followerStartPos = f.transform.position;
        followerStartRot = f.transform.rotation;

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (
            Vector3.Distance(player.transform.position, player1StandPoint.position) > 0.05f ||
            Vector3.Distance(follower.transform.position, player2StandPoint.position) > 0.05f)
        {
            player.transform.position =
                Vector3.MoveTowards(
                    player.transform.position,
                    player1StandPoint.position,
                    5f * Time.deltaTime);

            follower.transform.position =
                Vector3.MoveTowards(
                    follower.transform.position,
                    player2StandPoint.position,
                    5f * Time.deltaTime);

            player.transform.rotation =
                Quaternion.RotateTowards(
                    player.transform.rotation,
                    player1StandPoint.rotation,
                    360f * Time.deltaTime);

            follower.transform.rotation =
                Quaternion.RotateTowards(
                    follower.transform.rotation,
                    player2StandPoint.rotation,
                    360f * Time.deltaTime);

            yield return null;
        }
    }

    public void EndPresentation()
    {
        StartCoroutine(ReturnRoutine());
    }

    IEnumerator ReturnRoutine()
    {
        while (
            Vector3.Distance(player.transform.position, playerStartPos) > 0.05f ||
            Vector3.Distance(follower.transform.position, followerStartPos) > 0.05f)
        {
            player.transform.position =
                Vector3.MoveTowards(
                    player.transform.position,
                    playerStartPos,
                    5f * Time.deltaTime);

            follower.transform.position =
                Vector3.MoveTowards(
                    follower.transform.position,
                    followerStartPos,
                    5f * Time.deltaTime);

            yield return null;
        }
    }
}