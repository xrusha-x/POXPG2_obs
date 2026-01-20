using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CustomHeartsUI : MonoBehaviour
{
    public Transform container;
    public Sprite heartFull;
    public Sprite heartEmpty;
    public float loseAnimScale = 1.3f;
    public float loseAnimDuration = 0.2f;
    public float spacing = 10f;
    public float heartSize = 32f;

    List<Image> hearts = new List<Image>();
    int maxHearts;

    public void Initialize(int max)
    {
        maxHearts = Mathf.Max(1, max);
        BuildHearts();
        SetHealth(maxHearts);
    }

    public void SetHealth(int current)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            bool filled = i < current;
            if (heartFull != null && filled) hearts[i].sprite = heartFull;
            else if (heartEmpty != null && !filled) hearts[i].sprite = heartEmpty;
            hearts[i].enabled = true;
            hearts[i].color = Color.white;
        }
        if (current <= 0)
        {
            for (int i = 0; i < hearts.Count; i++) hearts[i].enabled = false;
        }
    }

    public void PlayLoseFeedback(int newHealth)
    {
        int idx = Mathf.Clamp(newHealth, 0, hearts.Count - 1);
        if (idx < hearts.Count && idx >= 0)
        {
            StartCoroutine(ScaleFlash(hearts[idx]));
        }
    }

    public void PlayLoseFeedbackRange(int oldHealth, int newHealth)
    {
        int start = Mathf.Clamp(oldHealth - 1, 0, hearts.Count - 1);
        int end = Mathf.Clamp(newHealth, 0, hearts.Count - 1);
        for (int i = start; i >= end; i--)
        {
            if (i >= 0 && i < hearts.Count)
            {
                StartCoroutine(ScaleFlash(hearts[i]));
            }
        }
    }

    void BuildHearts()
    {
        if (container == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            GameObject row = new GameObject("HeartsRow");
            if (canvas != null) row.transform.SetParent(canvas.transform, false);
            container = row.transform;
            var rt = row.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(20, -20);
            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = spacing;
            hlg.childAlignment = TextAnchor.UpperLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            row.transform.localScale = Vector3.one;
        }
        hearts.Clear();
        var existing = container.GetComponentsInChildren<Image>(true);
        if (existing != null && existing.Length > 0)
        {
            for (int i = 0; i < existing.Length; i++)
            {
                var img = existing[i];
                var rt = img.rectTransform;
                rt.sizeDelta = new Vector2(heartSize, heartSize);
                if (heartFull != null) img.sprite = heartFull;
                hearts.Add(img);
            }
            while (hearts.Count < maxHearts)
            {
                GameObject h = new GameObject("Heart_" + (hearts.Count + 1));
                h.transform.SetParent(container, false);
                var img = h.AddComponent<Image>();
                var rt = h.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(heartSize, heartSize);
                if (heartFull != null) img.sprite = heartFull;
                hearts.Add(img);
            }
        }
        else
        {
            for (int i = 0; i < maxHearts; i++)
            {
                GameObject h = new GameObject("Heart_" + (i + 1));
                h.transform.SetParent(container, false);
                var img = h.AddComponent<Image>();
                var rt = h.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(heartSize, heartSize);
                if (heartFull != null) img.sprite = heartFull;
                hearts.Add(img);
            }
        }
    }

    IEnumerator ScaleFlash(Image img)
    {
        if (img == null) yield break;
        var rt = img.rectTransform;
        Vector3 orig = rt.localScale;
        Vector3 target = orig * loseAnimScale;
        float half = loseAnimDuration * 0.5f;
        float t = 0f;
        while (t < half)
        {
            rt.localScale = Vector3.Lerp(orig, target, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            rt.localScale = Vector3.Lerp(target, orig, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        rt.localScale = orig;
    }
}
