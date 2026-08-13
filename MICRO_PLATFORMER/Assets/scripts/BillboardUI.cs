using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    CoopCameraController coopCam;

    void Awake()
    {
        coopCam = FindFirstObjectByType<CoopCameraController>();
    }

    void LateUpdate()
    {
        if (coopCam == null)
            return;

        Camera targetCamera = GetCorrectCamera();

        if (targetCamera == null)
            return;

        Vector3 direction =
            transform.position - targetCamera.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation =
            Quaternion.LookRotation(direction);
    }

    Camera GetCorrectCamera()
    {
        if (!coopCam.IsSplitScreen())
        {
            return coopCam.GetCameraForPlayer(0);
        }

        Camera player1Camera = coopCam.GetCameraForPlayer(0);
        Camera player2Camera = coopCam.GetCameraForPlayer(1);

        if (IsVisibleFromCamera(player1Camera))
            return player1Camera;

        if (IsVisibleFromCamera(player2Camera))
            return player2Camera;

        return null;
    }

    bool IsVisibleFromCamera(Camera cam)
    {
        if (cam == null || !cam.isActiveAndEnabled)
            return false;

        Vector3 viewportPosition =
            cam.WorldToViewportPoint(transform.position);

        return viewportPosition.z > 0f &&
               viewportPosition.x >= 0f &&
               viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f &&
               viewportPosition.y <= 1f;
    }
}