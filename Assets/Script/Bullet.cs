using UnityEngine;

// ติด script นี้ที่ bulletPrefab
// ฟิสิกส์ที่ใช้:
//   A — Unity Physics 3D: OnCollisionEnter (Collision Detection)
//   B — แรงเสียดทาน: ใช้ Physic Material บน Bullet/Wall (ตั้งใน Inspector)
//   D — กฎความโน้มถ่วงสากล: F = GMm/r² คำนวณเองแทน Unity gravity
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage = 1f;

    [Header("Bullet Physics")]
    public float mass   = 0.01f;  // kg — มวลกระสุน
    public float radius = 0.05f;  // m  — รัศมีกระสุน

    [Header("Push Force")]
    [Tooltip("ตัวคูณแรงผลัก — ยิ่งมากยิ่งดัน wall แรง")]
    public float pushMultiplier = 5f;

    // ── D. กฎความโน้มถ่วงสากล (Newton's Law of Universal Gravitation)
    // F = G * M * m / r²  →  g = GM/R²  →  F_g = m * g
    const float G       = 6.674e-11f; // gravitational constant
    const float M_earth = 5.972e24f;  // มวลโลก (kg)
    const float R_earth = 6.371e6f;   // รัศมีโลก (m)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = mass;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // เพิ่มบรรทัดนี้
    }

    void FixedUpdate()
    {
        // ── D. คำนวณแรงโน้มถ่วงจากสูตรแล้ว AddForce ─────────────
        float   g         = (G * M_earth) / (R_earth * R_earth); // ≈ 9.81 m/s²
        Vector3 F_gravity = new Vector3(0f, -mass * g, 0f);      // F = mg (ลงด้านล่าง)
        rb.AddForce(F_gravity, ForceMode.Force);
    }

    // ── A. Collision Detection (OnCollisionEnter) ─────────────────
    // B. Physic Material (Friction) ตั้งบน Bullet Collider ใน Inspector
    //    → ให้กระสุนมีแรงเสียดทานตอนไถลบนพื้นหลังชน
    void OnCollisionEnter(Collision collision)
    {
        Wall wall = collision.collider.GetComponent<Wall>();
        if (wall != null)
        {
            wall.TakeDamage(damage);

            Rigidbody wallRb = collision.collider.GetComponent<Rigidbody>();
            if (wallRb != null)
            {
                Vector3 pushDir = -collision.contacts[0].normal;
                float speed = rb.linearVelocity.magnitude;
                float pushForce = damage * mass * speed * pushMultiplier;
                wallRb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }
            // ไม่ Destroy — กระสุนกระดอนต่อได้ตาม Physic Material
        }
    }
}