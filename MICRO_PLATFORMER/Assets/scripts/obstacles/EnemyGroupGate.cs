using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemyGroupGate : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;
    [SerializeField] Door door;

    [Header("Camera Focus")]
    [SerializeField] Transform focusPoint;

    [Header("Kill Counter Popup")]
    [SerializeField] GameObject counterPopupPrefab;

    [Header("Door Counter")]
    [SerializeField] TextMeshProUGUI doorCounterText;

    DoorCameraFocus cameraFocus;

    HashSet<Enemy> countedEnemies = new HashSet<Enemy>();

    int killCount;
    bool opened;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    void Start()
    {
        UpdateDoorCounter();
    }

    void Update()
    {
        if (opened) return;

        foreach (Enemy e in enemies)
        {
            if (e == null) continue;

            if (e.IsDead && !countedEnemies.Contains(e))
            {
                countedEnemies.Add(e);
                killCount++;

                SpawnPopup(e.transform.position, killCount);
                UpdateDoorCounter();
            }
        }

        if (killCount >= enemies.Length)
        {
            opened = true;

            bool firstTime = door.Open();

            if (doorCounterText)
                doorCounterText.text = enemies.Length + " / " + enemies.Length;

            if (firstTime && cameraFocus && focusPoint)
                cameraFocus.FocusOn(focusPoint);
        }
    }

    void SpawnPopup(Vector3 position, int number)
    {
        if (!counterPopupPrefab) return;

        Vector3 spawnPos = position + Vector3.up * 1.5f;

        GameObject popup = Instantiate(counterPopupPrefab, spawnPos, Quaternion.identity);

        EnemyKillPopup popupScript = popup.GetComponent<EnemyKillPopup>();
        if (popupScript)
            popupScript.SetNumber(number);
    }

    void UpdateDoorCounter()
    {
        if (!doorCounterText) return;

        doorCounterText.text = killCount + " / " + enemies.Length;
    }
}