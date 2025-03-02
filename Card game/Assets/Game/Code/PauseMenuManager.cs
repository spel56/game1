using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu; // Panel ของ Pause Menu
    public GameObject confirmPopup; // Panel ของ Confirm Popup
    public Button closeButton; // ปุ่มปิด Pause Menu (❌)

    public Button stopButton; // ปุ่ม Stop
    public Button homeButton; // ปุ่มกลับหน้าโฮม
    public Button restartButton; // ปุ่มเริ่มใหม่
    public Button confirmYesButton; // ปุ่มยืนยันรีสตาร์ท
    public Button confirmNoButton; // ปุ่มยกเลิก

    void Start()
    {
        // ซ่อนเมนูเมื่อเริ่มเกม
        pauseMenu.SetActive(false);
        confirmPopup.SetActive(false);

        // กำหนดให้ปุ่มทำงานเมื่อกด
        stopButton.onClick.AddListener(OpenPauseMenu);
        homeButton.onClick.AddListener(GoToHome);
        restartButton.onClick.AddListener(OpenConfirmPopup);
        confirmYesButton.onClick.AddListener(ConfirmRestart);
        confirmNoButton.onClick.AddListener(CloseConfirmPopup);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePauseMenu); // ✅ กำหนดให้ปุ่ม ❌ ใช้งานได้
        }
    }

    // เปิด Pause Menu
    void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0; // หยุดเวลาในเกม
    }

    // ปิด Pause Menu
    void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1; // เล่นเกมต่อ
    }

    // กลับหน้าโฮม
    void GoToHome()
    {
        Debug.Log("🏠 กลับไปหน้าโฮม (ยังไม่ทำ)");
        // ใส่ SceneManager.LoadScene("ชื่อScene") ถ้ามีหน้าโฮม
    }

    // เปิด Confirm Popup และซ่อน Pause Menu
    void OpenConfirmPopup()
    {
        pauseMenu.SetActive(false); // ซ่อน Pause Menu
        confirmPopup.SetActive(true);
    }

    void ConfirmRestart()
    {
        Debug.Log("🔄 รีสตาร์ทเกม...");
        Time.timeScale = 1; // กลับมาเล่นเกมปกติ
        confirmPopup.SetActive(false); // ปิด Confirm Popup

        // ✅ รีเซ็ตเวลาเมื่อกดเริ่มใหม่
        FindObjectOfType<GameTimer>().ResetTimer();

        GameManagerNew.Instance.RestartGame(); // รีสตาร์ทเกม
    }

    // ปิด Confirm Popup และแสดง Pause Menu กลับมา
    void CloseConfirmPopup()
    {
        confirmPopup.SetActive(false);
        pauseMenu.SetActive(true); // แสดง Pause Menu กลับมา
    }
}
