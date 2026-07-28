using UnityEngine;

public class BoomerangTarget : MonoBehaviour
{
    [SerializeField] private GameObject blueArrow;
    [SerializeField] private GameObject redArrow;

    public void ShowMarker(int playerIndex)
    {
        if (playerIndex == 0)
        {
            blueArrow.SetActive(true);
        }
        else
        {
            redArrow.SetActive(true);
        }
    }

    public void HideMarker(int playerIndex)
    {
        if (playerIndex == 0)
        {
            blueArrow.SetActive(false);
        }
        else
        {
            redArrow.SetActive(false);
        }
    }
}
