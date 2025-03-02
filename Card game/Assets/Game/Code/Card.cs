using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage;
    public Sprite frontImage;
    public Sprite backImage;

    private bool isFlipped = false;
    private bool isMatched = false; // เช็คว่าไพ่จับคู่แล้วหรือยัง

    private GameManagerNew gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManagerNew>();

        if (cardImage == null)
        {
            Debug.LogError("[Card] ❌ ไม่พบ Image Component!");
            return;
        }

        if (backImage == null)
        {
            Debug.LogError("[Card] ❌ ไม่พบ backImage!");
        }
        else
        {
            cardImage.sprite = backImage;
        }
    }

    public void FlipCard()
    {
        if (isMatched || gameManager.IsChecking) return; // ป้องกันกดไพ่ที่หายไป หรือกดเพิ่มขณะเช็คคู่

        isFlipped = !isFlipped;
        cardImage.sprite = isFlipped ? frontImage : backImage;

        gameManager.AddFlippedCard(this);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }

    public void ResetCard()
    {
        isFlipped = false;
        cardImage.sprite = backImage;
    }

    public void SetMatched()
    {
        isMatched = true;
    }
}
