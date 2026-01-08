using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart; // Optional

    [Header("Score UI")]
    public Text scoreText;

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                if (fullHeart != null) hearts[i].sprite = fullHeart;
                hearts[i].enabled = true;
            }
            else
            {
                if (emptyHeart != null) 
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }
        }
    }
}
