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

    private string HPKey        => "Wall_HP_"         + gameObject.name;
    private string DeadKey      => "Wall_Dead_"       + gameObject.name;
    private string DestroyedKey => "Wall_Destroyed_"  + gameObject.name;

    void Start()
    {
        initialMaxHealth = maxHealth;
        wallManager = FindObjectsOfType<WallBreakManager>()[0];

        if (PlayerPrefs.HasKey(HPKey))
        {
            currentHealth = PlayerPrefs.GetFloat(HPKey);
            hasBeenDestroyed = PlayerPrefs.GetInt(DestroyedKey, 0) == 1;

            if (PlayerPrefs.GetInt(DeadKey, 0) == 1)
            {
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            currentHealth = maxHealth;
        }

        UpdateHPUI();
    }

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
        {
            currentHealth = 0; // lock ที่ 0 กันติดลบ
            StartCoroutine(DestroyAndRespawn());
        }
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

        // เรียก OnWallDestroyed ก่อน disable เสมอ
        if (!hasBeenDestroyed)
        {
            hasBeenDestroyed = true;
            wallManager?.OnWallDestroyed(gameObject);
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        PlayerPrefs.DeleteKey(HPKey);
        PlayerPrefs.DeleteKey(DeadKey);
        PlayerPrefs.Save();

        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
        hasBeenDestroyed = false; // reset ให้ respawn รอบใหม่ได้
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        UpdateHPUI();
    }
}