using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int currentScore = 0;
    public Text scoreText;
    public bool createUIIfMissing = true;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (scoreText == null && createUIIfMissing)
        {
            var canvas = Object.FindObjectOfType<UnityEngine.Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<UnityEngine.Canvas>();
                canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            EnsureScoreText(canvas.gameObject);
        }
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
    }

    void EnsureScoreText(GameObject canvasGO = null)
    {
        if (scoreText != null) return;
        if (canvasGO == null || canvasGO.GetComponent<UnityEngine.Canvas>() == null)
        {
            var canvas = Object.FindObjectOfType<UnityEngine.Canvas>();
            if (canvas == null)
            {
                canvasGO = new GameObject("Canvas");
                var cv = canvasGO.AddComponent<UnityEngine.Canvas>();
                cv.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            else
            {
                canvasGO = canvas.gameObject;
            }
        }

        GameObject scoreTextGO = new GameObject("ScoreText");
        scoreTextGO.transform.SetParent(canvasGO.transform, false);

        var text = scoreTextGO.AddComponent<UnityEngine.UI.Text>();
        text.text = "Score: 0";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperRight;

        var rt = scoreTextGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-20, -20);
        rt.sizeDelta = new Vector2(200, 50);

        scoreText = text;
    }
}
