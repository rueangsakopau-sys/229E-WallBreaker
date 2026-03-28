using UnityEngine;

// ติด script นี้ที่ Main Camera
public class CameraController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float sensitivity = 2f;     // ความไวเมาส์
    public float minPitch    = -30f;   // มุมก้มต่ำสุด
    public float maxPitch    = 60f;    // มุมเงยสูงสุด

    private float yaw;    // หมุนซ้าย-ขวา (แกน Y)
    private float pitch;  // หมุนขึ้น-ลง  (แกน X)

    void Start()
    {
        // เริ่มต้นจากมุมกล้องปัจจุบัน
        yaw   = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // กดคลิกขวาค้างไว้แล้วลากเมาส์เพื่อหมุนกล้อง
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            yaw   += mouseX;
            pitch -= mouseY;
            pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}