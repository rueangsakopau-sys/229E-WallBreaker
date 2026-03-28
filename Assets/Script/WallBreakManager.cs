using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WallBreakManager : MonoBehaviour
{
    public int totalWalls = 3;

    // เก็บ wall ที่ถูกทำลายไปแล้ว (ไม่นับซ้ำเมื่อ respawn แล้วถูกทำลายอีก)
    private HashSet<GameObject> destroyedWalls = new HashSet<GameObject>();

    public void OnWallDestroyed(GameObject wallObject)
    {
        // ถ้า wall นี้เคยถูกทำลายไปแล้ว (รอบ respawn) ไม่นับ
        if (destroyedWalls.Contains(wallObject)) return;

        destroyedWalls.Add(wallObject);
        Debug.Log($"Wall ถูกทำลาย: {wallObject.name} | รวม: {destroyedWalls.Count}/{totalWalls}");

        if (destroyedWalls.Count >= totalWalls)
            SceneManager.LoadScene("Credit");
    }
}