using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WallBreakManager : MonoBehaviour
{
    private int totalWalls;
    private HashSet<string> destroyedWallNames = new HashSet<string>();

    const string DestroyedKey = "WBM_DestroyedWalls";
    const string TotalKey     = "WBM_TotalWalls";

    void Start()
    {
        totalWalls = FindObjectsOfType<Wall>().Length;

        string saved = PlayerPrefs.GetString(DestroyedKey, "");
        if (!string.IsNullOrEmpty(saved))
        {
            foreach (string name in saved.Split(','))
                if (!string.IsNullOrEmpty(name))
                    destroyedWallNames.Add(name);
        }

        
        if (PlayerPrefs.HasKey(TotalKey))
            totalWalls = PlayerPrefs.GetInt(TotalKey);
        else
            PlayerPrefs.SetInt(TotalKey, totalWalls);

        PlayerPrefs.Save();

        Debug.Log($"Wall ทั้งหมด: {totalWalls} | ทำลายไปแล้ว: {destroyedWallNames.Count}");


        CheckAllDestroyed();
    }

    public void OnWallDestroyed(GameObject wallObject)
    {
        if (destroyedWallNames.Contains(wallObject.name)) return;

        destroyedWallNames.Add(wallObject.name);

        PlayerPrefs.SetString(DestroyedKey, string.Join(",", destroyedWallNames));
        PlayerPrefs.Save();

        Debug.Log($"Wall ถูกทำลาย: {wallObject.name} | รวม: {destroyedWallNames.Count}/{totalWalls}");

        CheckAllDestroyed();
    }

    void CheckAllDestroyed()
    {
        if (destroyedWallNames.Count >= totalWalls)
        {
            Debug.Log("ครบแล้ว — กำลังโหลด Credit");

            PlayerPrefs.DeleteKey(DestroyedKey);
            PlayerPrefs.DeleteKey(TotalKey);
            PlayerPrefs.Save();

            if (Application.CanStreamedLevelBeLoaded("Credit"))
                SceneManager.LoadScene("Credit");
            else
                Debug.LogError("Scene 'Credit' ไม่อยู่ใน Build Settings!");
        }
    }
}