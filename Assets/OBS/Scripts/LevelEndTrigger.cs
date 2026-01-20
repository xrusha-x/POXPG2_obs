using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelEndTrigger : MonoBehaviour
{
    public Text victoryText;
    public string levelId;
    public bool pauseOnWin = true;
    public string levelMenuScene = "Level_menu";
    public string promptText = "Victory! Press any key";

    bool done = false;

    void Start()
    {
        if (string.IsNullOrEmpty(levelId))
        {
            levelId = SceneManager.GetActiveScene().name;
        }
        if (victoryText != null) victoryText.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (done) return;
        if (!IsPlayer(col.gameObject)) return;

        bool allItems = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length == 0;
        bool noDamage = !PlayerHealth.TookDamageThisLevel;

        LevelSave.Set(levelId, true, allItems, noDamage);

        if (victoryText != null)
        {
            victoryText.text = promptText;
            victoryText.enabled = true;
        }

        if (pauseOnWin) Time.timeScale = 0f;
        StartCoroutine(WaitForAnyKeyThenProceed());
        done = true;
    }

    IEnumerator WaitForAnyKeyThenProceed()
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(levelMenuScene))
        {
            SceneManager.LoadScene(levelMenuScene);
        }
    }

    bool IsPlayer(GameObject obj)
    {
        if (obj == null) return false;
        if (obj.CompareTag("Player")) return true;
        if (obj.GetComponent<PlayerControllerUpdate>() != null) return true;
        if (obj.GetComponentInParent<PlayerControllerUpdate>() != null) return true;
        Transform root = obj.transform != null ? obj.transform.root : null;
        if (root != null && root.CompareTag("Player")) return true;
        return obj.name.Contains("Player");
    }
}
