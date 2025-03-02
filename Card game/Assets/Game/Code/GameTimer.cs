using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLeft = 90f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI secondsText; // ✅ เพิ่มตัวแปรสำหรับแสดงเวลาที่ใช้
    public GameObject timeUpPanel;

    private bool isGameOver = false;

    void Start()
    {
        timeUpPanel.SetActive(false);
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerDisplay();

            // ✅ หยุดเวลาเมื่อชนะเกม และบันทึกเวลาที่ใช้
            if (GameManagerNew.Instance != null && GameManagerNew.Instance.winPanel.activeSelf)
            {
                StopTimer();
            }

            if (timeLeft <= 0)
            {
                GameOver();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = Mathf.Ceil(timeLeft).ToString();
    }

    void GameOver()
    {
        if (GameManagerNew.Instance.winPanel.activeSelf)
        {
            return; // ✅ ถ้าชนะแล้ว ไม่ต้องแสดง Game Over
        }

        isGameOver = true;
        timerText.text = "0";
        timeUpPanel.SetActive(true);
        Debug.Log("⏳ เวลา หมดแล้ว! เกมจบ!");
    }

    public void StopTimer()
    {
        isGameOver = true;
        if (timeLeft < 0) timeLeft = 0;

        timerText.text = "0";

        // ✅ แสดงเวลาที่ใช้ในการเล่น
        if (secondsText != null)
        {
            float timeUsed = 90f - timeLeft;
            secondsText.text = Mathf.Ceil(timeUsed).ToString();
        }
    }

    public void RestartGame()
    {
        Debug.Log("🔄 กำลังเริ่มเกมใหม่...");
        isGameOver = false;
        timeLeft = 90f;
        timeUpPanel.SetActive(false);
        UpdateTimerDisplay();

        // ✅ รีเซ็ตเวลาที่ใช้เมื่อเริ่มใหม่
        if (secondsText != null)
        {
            secondsText.text = "0";
        }

        GameManagerNew.Instance.RestartGame();
    }
    public void ResetTimer()
{
    isGameOver = false;
    timeLeft = 90f; // รีเซ็ตเวลาเป็นค่าเริ่มต้น
    UpdateTimerDisplay();
}
}


