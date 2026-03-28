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

    // บันทึกว่า wall นี้เคยถูกทำลายไปแล้วหรือยัง (ครั้งแรก)
    private bool hasBeenDestroyed = false;

    private float initialMaxHealth; // เก็บค่า maxHealth ที่ตั้งไว้ตอนแรก

    void Start()
    {
        initialMaxHealth = maxHealth;  // จำค่าเริ่มต้นไว้
        currentHealth = maxHealth;
        wallManager = FindObjectsOfType<WallBreakManager>()[0];
        UpdateHPUI();
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) return; // กำลัง respawn อยู่ ยิงไม่โดน

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

        // ส่ง gameObject ไปด้วย เพื่อให้ WallBreakManager รู้ว่าเป็น wall ตัวไหน
        // และนับเฉพาะครั้งแรกที่ถูกทำลาย
        if (!hasBeenDestroyed)
        {
            hasBeenDestroyed = true;
            wallManager?.OnWallDestroyed(gameObject);
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // Respawn — reset กลับค่า maxHealth ที่ตั้งไว้ตอนแรก ไม่บวกเพิ่ม
        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        UpdateHPUI();
    }
}