// MusicToggleButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicToggleButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText; // ลาก Text ของปุ่มมาใส่

    void Start() => UpdateLabel();

    public void OnClick()
    {
        AudioManager.Instance?.ToggleMusic();
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (AudioManager.Instance == null) return;
        buttonText.text = AudioManager.Instance.IsMuted ? "Music OFF" : "Music ON";
    }
}