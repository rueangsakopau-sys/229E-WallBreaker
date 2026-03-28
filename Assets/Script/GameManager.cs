using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Data")]
    public float cash = 0f;
    public GunData equippedGun;
    public GunData[] allGuns;

    // เก็บ index ของปืนที่ซื้อแล้ว (แทนการใช้ owned ใน ScriptableObject)
    public List<int> ownedGunIndexes = new List<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ปืนตัวแรก (index 0) ได้ฟรีเสมอ
            if (!ownedGunIndexes.Contains(0))
                ownedGunIndexes.Add(0);

            // ถ้ายังไม่มีปืน equipped ให้ใช้ตัวแรก
            if (equippedGun == null && allGuns != null && allGuns.Length > 0)
                equippedGun = allGuns[0];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsOwned(int gunIndex)
    {
        return ownedGunIndexes.Contains(gunIndex);
    }

    public void BuyGun(int gunIndex)
    {
        if (!ownedGunIndexes.Contains(gunIndex))
            ownedGunIndexes.Add(gunIndex);
    }
}