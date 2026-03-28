// AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ดนตรีเล่นต่อเนื่องข้ามฉาก
        }
        else
        {
            Destroy(gameObject); // ไม่ให้ซ้ำกัน
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public bool IsMuted => !audioSource.enabled || audioSource.mute;

    public void ToggleMusic()
    {
        audioSource.mute = !audioSource.mute;
    }

    public void SetMute(bool mute)
    {
        audioSource.mute = mute;
    }
}