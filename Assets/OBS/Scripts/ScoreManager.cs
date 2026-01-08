using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int currentScore = 0;
    public UIManager uiManager;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"Score Added! Total: {currentScore}");
        UpdateUI();
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
        }
    }
}
