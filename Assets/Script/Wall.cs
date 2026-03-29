using UnityEngine;
using TMPro;
using System.Collections;

public class Wall : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float cashPerHit = 10f;
    public float cashOnDestroy = 50f;
    public float respawnTime = 120f;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    private float currentHealth;
    private WallBreakManager wallManager;

    private bool hasBeenDestroyed = false;
    private float initialMaxHealth;

    // Key สำหรับ PlayerPrefs — ใช้ชื่อ GameObject เป็น unique key
    private string HPKey       => "Wall_HP_"          + gameObject.name;
    private string DeadKey     => "Wall_Dead_"        + gameObject.name;
    private string DestroyedKey => "Wall_Destroyed_"  + gameObject.name;

    void Start()
    {
        initialMaxHealth = maxHealth;
        wallManager = FindObjectsOfType<WallBreakManager>()[0];

        // โหลด HP ที่บันทึกไว้ก่อนออกไป Shop
        // ถ้าไม่มีค่า (ขึ้นเกมใหม่) ให้ใช้ maxHealth ปกติ
        if (PlayerPrefs.HasKey(HPKey))
        {
            currentHealth = PlayerPrefs.GetFloat(HPKey);
            hasBeenDestroyed = PlayerPrefs.GetInt(DestroyedKey, 0) == 1;

            // Wall ตายอยู่ตอนออกไป Shop → ซ่อน renderer/collider ไว้
            if (PlayerPrefs.GetInt(DeadKey, 0) == 1)
            {
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                // ไม่ต้อง StartCoroutine respawn ซ้ำ เพราะ respawnTime นับใหม่ไม่ได้
                // (ถ้าอยากให้ respawn ต่อได้ต้องบันทึก timestamp ด้วย — ดูหมายเหตุล่าง)
            }
        }
        else
        {
            currentHealth = maxHealth;
        }

        UpdateHPUI();
    }

    /// <summary>
    /// เรียกจาก PlayerShoot.OnShopClick() ก่อน LoadScene("Shop")
    /// เพื่อบันทึก HP ปัจจุบันลง PlayerPrefs
    /// </summary>
    public void SaveState()
    {
        PlayerPrefs.SetFloat(HPKey, currentHealth);
        PlayerPrefs.SetInt(DeadKey, currentHealth <= 0 ? 1 : 0);
        PlayerPrefs.SetInt(DestroyedKey, hasBeenDestroyed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) return;

        currentHealth -= dmg;
        if (GameManager.Instance != null)
            GameManager.Instance.cash += cashPerHit * dmg;
        UpdateHPUI();

        if (currentHealth <= 0)
            StartCoroutine(DestroyAndRespawn());
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = Mathf.Max(0, currentHealth).ToString("0");
    }

    IEnumerator DestroyAndRespawn()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.cash += cashOnDestroy;

        if (!hasBeenDestroyed)
        {
            hasBeenDestroyed = true;
            wallManager?.OnWallDestroyed(gameObject);
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // Respawn — ล้าง saved state ด้วยเพื่อไม่ให้โหลดกลับมาเป็น dead อีก
        PlayerPrefs.DeleteKey(HPKey);
        PlayerPrefs.DeleteKey(DeadKey);
        PlayerPrefs.Save();

        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        UpdateHPUI();
    }
}