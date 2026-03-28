using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnStartClick()
    {
        // ล้าง HP ที่ save ไว้ทั้งหมดเมื่อเริ่มเกมใหม่
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainGame");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}