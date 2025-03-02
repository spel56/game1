using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; // ใช้สำหรับ TextMeshPro

public class GameManagerNew : MonoBehaviour
{
    public static GameManagerNew Instance;
    public GameObject cardPrefab;
    public Transform gameBoard;
    public List<Sprite> fruitImages;

    public Image scoreBarFill;
    public Image scoreBarAccuracy; // ✅ เพิ่มแถบความแม่นยำ
    public GameObject winPanel;
    public GameObject timeUpPanel;

    public TextMeshProUGUI accuracyText; // ✅ เพิ่มตัวแปรแสดงค่าความแม่นยำ
    public TextMeshProUGUI flipCountTotalText; // ✅ เพิ่มตัวแปรสำหรับ Flip Count (รวม)
    public TextMeshProUGUI flipCountRealText; // ✅ เพิ่มตัวแปรสำหรับ Flip Count (จริง)
    public TextMeshProUGUI flipCountWrongText; // ✅ เพิ่มตัวแปรสำหรับ Flip Count (ผิด)

    public int totalFlips = 0; // ✅ ตัวแปรนับจำนวนครั้งที่เปิดไพ่ทั้งหมด
    public int realFlips = 0; // ✅ ตัวแปรนับจำนวนครั้งที่จับคู่ได้ถูกต้องจริงๆ
    public int wrongFlips = 0; // ✅ ตัวแปรนับจำนวนครั้งที่จับคู่ผิดพลาด
    private float accuracy = 0f; // ✅ ตัวแปรเก็บค่าความแม่นยำ

    public float maxTime = 90f;
    private float currentTime;
    private List<Sprite> shuffledImages = new List<Sprite>();
    private List<Card> allCards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();

    public bool IsChecking { get; private set; } = false;
    private int matchedPairs = 0;
    private Sprite backImage;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("[GameManager] 🎴 กำลังสร้างไพ่...");
        currentTime = maxTime;
        winPanel.SetActive(false);
        timeUpPanel.SetActive(false);
        InitializeGame();
        UpdateScoreBarColor();
        UpdateAccuracy(); // ✅ อัปเดตค่าความแม่นยำ
        UpdateFlipCountTotal(); // ✅ อัปเดตจำนวน Flip Count รวม
        UpdateFlipCountReal(); // ✅ อัปเดตจำนวน Flip Count จริง
        UpdateFlipCountWrong(); // ✅ อัปเดตจำนวน Flip Count ผิด
    }

    private void Update()
    {
        if (!winPanel.activeSelf && !timeUpPanel.activeSelf)
        {
            currentTime -= Time.deltaTime;
            UpdateScoreBarColor();

            if (currentTime <= 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateScoreBarColor()
    {
        if (scoreBarFill == null) return;

        if (winPanel.activeSelf)
        {
            scoreBarFill.color = Color.blue;
        }
        else if (timeUpPanel.activeSelf)
        {
            scoreBarFill.color = Color.gray;
        }
        else if (currentTime > maxTime * 0.6f)
        {
            scoreBarFill.color = Color.green;
        }
        else if (currentTime > maxTime * 0.3f)
        {
            scoreBarFill.color = Color.yellow;
        }
        else
        {
            scoreBarFill.color = Color.red;
        }
    }

    private void InitializeGame()
    {
        backImage = fruitImages.Find(sprite => sprite.name == "white background");
        if (backImage == null)
        {
            Debug.LogError("[GameManager] ❌ ไม่พบ white background!");
            return;
        }

        GenerateCards();
    }

    private void GenerateCards()
    {
        if (fruitImages == null || fruitImages.Count < 9 || cardPrefab == null || gameBoard == null)
        {
            Debug.LogError("[GameManager] ❌ ค่า null หรือรูปไม่พอ!");
            return;
        }

        List<Sprite> selectedFruits = new List<Sprite>(fruitImages.GetRange(0, 8));
        shuffledImages.Clear();
        shuffledImages.AddRange(selectedFruits);
        shuffledImages.AddRange(selectedFruits);
        shuffledImages.Sort((a, b) => Random.Range(-1, 2));

        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gameBoard);
            Card cardScript = newCard.GetComponent<Card>();

            if (cardScript != null)
            {
                cardScript.frontImage = shuffledImages[i];
                cardScript.backImage = backImage;
                cardScript.cardImage.sprite = cardScript.backImage;
                allCards.Add(cardScript);
            }
            else
            {
                Debug.LogError("[GameManager] ❌ ไม่พบสคริปต์ Card.cs บน CardPrefab!");
            }
        }
    }

    public void AddFlippedCard(Card card)
    {
        if (flippedCards.Contains(card)) return;
        flippedCards.Add(card);
        totalFlips++; // ✅ นับจำนวนครั้งที่เปิดไพ่
        UpdateFlipCountTotal(); // ✅ อัปเดตค่า Flip Count รวม

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }

    }

    private IEnumerator CheckMatch()
    {
        IsChecking = true;
        yield return new WaitForSeconds(1f);

        if (flippedCards[0].frontImage == flippedCards[1].frontImage)
        {
            flippedCards[0].SetMatched();
            flippedCards[1].SetMatched();
            flippedCards[0].HideCard();
            flippedCards[1].HideCard();
            matchedPairs++;

            realFlips++; // ✅ เพิ่มจำนวน Flip Count จริง
            UpdateFlipCountReal(); // ✅ อัปเดตค่า Flip Count จริง

            if (matchedPairs == 8)
            {
                ShowWinPanel();
            }
        }
        else
        {
            flippedCards[0].ResetCard();
            flippedCards[1].ResetCard();

            wrongFlips++; // ✅ เพิ่มจำนวนครั้งที่จับคู่ผิด
            UpdateFlipCountWrong(); // ✅ อัปเดตค่า Flip Count ผิด
        }

        flippedCards.Clear();
        IsChecking = false;
        UpdateAccuracy(); // ✅ อัปเดตค่าความแม่นยำทุกครั้งที่เปิดไพ่
    }

    private void UpdateAccuracy()
    {
        if (accuracyText == null || scoreBarAccuracy == null) return;

        if (totalFlips == 0)
        {
            accuracy = 0f;
        }
        else
        {
            accuracy = ((float)matchedPairs * 2 / totalFlips) * 100f;
        }

        accuracyText.text = accuracy.ToString("F1"); // ✅ แสดงทศนิยม 1 ตำแหน่ง

        // ✅ เปลี่ยนสี ScoreBarAccuracy ตามค่าความแม่นยำ
        if (accuracy > 60f)
        {
            scoreBarAccuracy.color = Color.green;
        }
        else if (accuracy > 40f)
        {
            scoreBarAccuracy.color = Color.yellow;
        }
        else
        {
            scoreBarAccuracy.color = Color.red;
        }
    }

    private void UpdateFlipCountTotal()
    {
        if (flipCountTotalText == null) return;
        flipCountTotalText.text = totalFlips.ToString();
    }

    private void UpdateFlipCountReal()
    {
        if (flipCountRealText == null) return;
        flipCountRealText.text = realFlips.ToString();
    }

    private void UpdateFlipCountWrong()
    {
        if (flipCountWrongText == null) return;
        flipCountWrongText.text = wrongFlips.ToString();
    }

    private void ShowWinPanel()
    {
        Debug.Log("🎉 คุณชนะแล้ว!");
        winPanel.SetActive(true);
        timeUpPanel.SetActive(false);
        UpdateAccuracy();
    }

    private void GameOver()
    {
        Debug.Log("⏳ เวลา หมดแล้ว! เกมจบ!");
        timeUpPanel.SetActive(true);
        winPanel.SetActive(false);
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] 🔄 รีสตาร์ทเกม...");

        matchedPairs = 0;
        flippedCards.Clear();
        IsChecking = false;
        totalFlips = 0;
        realFlips = 0;
        wrongFlips = 0;
        accuracy = 0f;
        currentTime = maxTime;

        foreach (Transform child in gameBoard)
        {
            Destroy(child.gameObject);
        }

        winPanel.SetActive(false);
        timeUpPanel.SetActive(false);
        GenerateCards();
        UpdateScoreBarColor();
        UpdateAccuracy();
        UpdateFlipCountTotal();
        UpdateFlipCountReal();
        UpdateFlipCountWrong();

        Debug.Log("[GameManager] ✅ รีสตาร์ทเกมเสร็จสิ้น!");
    }

}
