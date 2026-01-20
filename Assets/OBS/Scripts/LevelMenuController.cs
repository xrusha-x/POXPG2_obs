using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMenuController : MonoBehaviour
{
    [System.Serializable]
    public class LevelEntry
    {
        public string levelId;
        public Button levelButton;
        public Image lockOverlay;
        public Image[] stars;
    }

    public List<LevelEntry> levels = new List<LevelEntry>();
    public Sprite filledStar;
    public Sprite unfilledStar;
    public float starFadeDuration = 0.3f;
    public int starSpacingPx = 20;

    void Start()
    {
        UpdateMenu();
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            bool unlocked = i == 0 || LevelSave.GetCompleted(levels[i - 1].levelId);
            var entry = levels[i];

            if (entry.levelButton != null) entry.levelButton.interactable = unlocked;
            if (entry.lockOverlay != null) entry.lockOverlay.enabled = !unlocked;

            bool completed = LevelSave.GetCompleted(entry.levelId);
            bool allItems = LevelSave.GetAllItems(entry.levelId);
            bool noDamage = LevelSave.GetNoDamage(entry.levelId);

            SetStars(entry.stars, completed, allItems, noDamage);
            ApplyStarSpacing(entry.stars);
        }
    }

    void SetStars(Image[] stars, bool completed, bool allItems, bool noDamage)
    {
        if (stars == null || stars.Length == 0) return;
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            bool on = false;
            if (i == 0) on = completed;
            else if (i == 1) on = completed && allItems;
            else if (i == 2) on = completed && allItems && noDamage;
            var targetSprite = on ? filledStar : unfilledStar;
            if (stars[i].sprite != targetSprite)
            {
                StartCoroutine(FadeToSprite(stars[i], targetSprite, starFadeDuration));
            }
            else
            {
                stars[i].sprite = targetSprite;
                var c = stars[i].color;
                stars[i].color = new Color(c.r, c.g, c.b, 1f);
            }
        }
    }

    void ApplyStarSpacing(Image[] stars)
    {
        if (stars == null || stars.Length == 0) return;
        Transform parent = stars[0].transform.parent;
        var layout = parent != null ? parent.GetComponent<HorizontalLayoutGroup>() : null;
        if (layout != null)
        {
            layout.spacing = starSpacingPx;
        }
    }

    System.Collections.IEnumerator FadeToSprite(Image img, Sprite target, float duration)
    {
        if (img == null) yield break;
        float t = 0f;
        var c = img.color;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / (duration * 0.5f));
            img.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        img.sprite = target;
        t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0f, 1f, t / (duration * 0.5f));
            img.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 1f);
    }
}
