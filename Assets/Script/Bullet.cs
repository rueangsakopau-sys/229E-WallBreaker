using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage = 1f;

    [Header("Bullet Physics")]
    public float mass   = 0.01f;
    public float radius = 0.05f;

    [Header("Push Force")]
    public float pushMultiplier = 5f;

    [Header("Friction (ตอนไถลพื้น)")]
    public float frictionCoefficient = 0.5f; // μ — ยิ่งมากหยุดเร็ว

    const float G       = 6.674e-11f;
    const float M_earth = 5.972e24f;
    const float R_earth = 6.371e6f;

    private Rigidbody rb;
    private HashSet<Wall> hitWalls = new HashSet<Wall>();
    private bool isOnGround = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = mass;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        // แรงโน้มถ่วง
        float g = (G * M_earth) / (R_earth * R_earth);
        Vector3 F_gravity = new Vector3(0f, -mass * g, 0f);
        rb.AddForce(F_gravity, ForceMode.Force);

        if (isOnGround)
        {
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVel.magnitude > 0.05f)
            {
                float N = mass * g;
                Vector3 F_friction = -horizontalVel.normalized * (frictionCoefficient * N);
                rb.AddForce(F_friction, ForceMode.Force);
            }
            else
            {
                // หยุดสนิทแล้ว — Destroy
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        Wall wall = collision.collider.GetComponent<Wall>();
        if (wall != null)
        {
            if (!hitWalls.Contains(wall))
            {
                hitWalls.Add(wall);
                wall.TakeDamage(damage);

                Rigidbody wallRb = collision.collider.GetComponent<Rigidbody>();
                if (wallRb != null)
                {
                    Vector3 pushDir   = -collision.contacts[0].normal;
                    float   speed     = rb.linearVelocity.magnitude;
                    float   pushForce = damage * mass * speed * pushMultiplier;
                    wallRb.AddForce(pushDir * pushForce, ForceMode.Impulse);
                }
            }
            return; 
        }


        isOnGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        // ลอยขึ้นจากพื้นอีกรอบ (กระดอน)
        if (collision.collider.GetComponent<Wall>() == null)
            isOnGround = false;
    }
}