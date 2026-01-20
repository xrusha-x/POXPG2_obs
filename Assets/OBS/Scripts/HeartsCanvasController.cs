using UnityEngine;
using UnityEngine.UI;

public class HeartsCanvasController : MonoBehaviour
{
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite heartFull;
    public Sprite heartEmpty;
    public float heartSize = 32f;
    public bool disableOtherHearts = true;

    CustomHeartsUI ui;

    void Awake()
    {
        ui = GetComponent<CustomHeartsUI>();
        if (ui == null) ui = gameObject.AddComponent<CustomHeartsUI>();
        ui.container = transform;
        ui.heartFull = heartFull != null ? heartFull : (heart1 != null ? heart1.sprite : null);
        ui.heartEmpty = heartEmpty;
        ui.heartSize = heartSize;
    }

    void Start()
    {
        if (disableOtherHearts)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var rows = canvas.GetComponentsInChildren<Transform>(true);
                foreach (var t in rows)
                {
                    if (t != null && t.gameObject != this.gameObject && t.name == "HeartsRow")
                    {
                        t.gameObject.SetActive(false);
                    }
                }
            }
        }
        var ph = FindObjectOfType<PlayerHealth>();
        if (ph != null)
        {
            ui.Initialize(ph.maxHealth);
            ph.heartsUI = ui;
            ui.SetHealth(ph.currentHealth);
        }
        else
        {
            ui.Initialize(3);
        }
    }
}
