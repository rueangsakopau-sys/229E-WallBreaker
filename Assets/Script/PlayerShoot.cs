using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour
{
    [Header("Gun Settings")]
    public GunData currentGun;
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public float bulletSpeed = 20f;
    public LayerMask wallLayer;

    [Header("UI")]
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI wallHPText;

    private float lastShotTime = -999f;
    private Camera cam;
    private LineRenderer lr;

    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();

        if (GameManager.Instance != null && GameManager.Instance.equippedGun != null)
            currentGun = GameManager.Instance.equippedGun;

        UpdateGunUI();
    }

    void Update()
    {
        UpdateCashUI();
        UpdateCooldownUI();
        DrawAimLine();

        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    void DrawAimLine()
    {
        if (lr == null || cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        lr.SetPosition(0, gunBarrel.position);
        lr.SetPosition(1, ray.GetPoint(200f));

        Wall[] allWalls = FindObjectsOfType<Wall>();

        Wall activeWall = null;
        float closest = float.MaxValue;

        foreach (Wall w in allWalls)
        {
            if (w.GetCurrentHP() <= 0) continue;
            float dist = Vector3.Distance(transform.position, w.transform.position);
            if (dist < closest)
            {
                closest = dist;
                activeWall = w;
            }
        }

        if (wallHPText != null)
            wallHPText.text = activeWall != null
                ? activeWall.GetCurrentHP().ToString("0")
                : "N/A";
    }

void TryShoot()
{
    if (currentGun == null) return;
    if (Time.time - lastShotTime < currentGun.cooldown) return;

    lastShotTime = Time.time;
    if (bulletPrefab == null || gunBarrel == null || cam == null) return;

    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    Vector3 targetPoint = ray.GetPoint(100f);

    if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        targetPoint = hit.point;

    Vector3 direction = (targetPoint - gunBarrel.position).normalized;

    GameObject b = Instantiate(bulletPrefab, gunBarrel.position, Quaternion.LookRotation(direction));
    Rigidbody rb = b.GetComponent<Rigidbody>();

    if (rb != null)
    {
        float damageBonus = currentGun != null ? currentGun.damage * 2f : 0f;
        float finalSpeed = bulletSpeed + damageBonus;


        rb.linearVelocity = direction * finalSpeed;
        rb.useGravity = true;
        

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    Bullet bullet = b.GetComponent<Bullet>();
    if (bullet != null)
        bullet.damage = currentGun.damage;

    Destroy(b, 5f);
}
    void UpdateGunUI()
    {
        if (currentGun == null) return;
        if (gunNameText) gunNameText.text = currentGun.gunName;
        if (damageText)  damageText.text  = currentGun.damage.ToString("0.0");
    }

    void UpdateCooldownUI()
    {
        if (cooldownText == null || currentGun == null) return;

        float remaining = currentGun.cooldown - (Time.time - lastShotTime);
        cooldownText.text = remaining <= 0 ? "Ready" : remaining.ToString("0.0") + "s";
    }

    void UpdateCashUI()
    {
        if (cashText && GameManager.Instance != null)
            cashText.text = "Cash: " + GameManager.Instance.cash.ToString("0");
    }

    public void OnShopClick()
    {
        // บันทึก HP ของ Wall ทุกตัวก่อนออกไป Shop
        foreach (Wall wall in FindObjectsOfType<Wall>())
            wall.SaveState();

        SceneManager.LoadScene("Shop");
    }

    public void SetGun(GunData gun)
    {
        currentGun = gun;
        if (GameManager.Instance != null)
            GameManager.Instance.equippedGun = gun;
        UpdateGunUI();
    }
}