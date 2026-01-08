using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RunAutoSetup()
    {
        Debug.Log("Starting Auto Setup for Health System...");

        // 1. Setup Player
        SetupPlayer();

        // 2. Setup UI
        UIManager uiManager = SetupUI();

        // 3. Link UI to Player
        if (uiManager != null)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.uiManager = uiManager;
                // Force UI update
                playerHealth.Invoke("UpdateUI", 0.1f);
            }
        }

        // 4. Setup Hazards
        SetupHazards();

        // 5. Setup Score System
        SetupScoreSystem(uiManager);

        // 6. Setup Collectibles
        SetupCollectibles();

        // 7. Setup Disappearing Platforms
        SetupDisappearingPlatforms();
        
        Debug.Log("Auto Setup Complete!");
    }

    static void SetupDisappearingPlatforms()
    {
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.name.Contains("podol_notRB"))
            {
                if (go.GetComponent<DisappearingPlatform>() == null)
                {
                    go.AddComponent<DisappearingPlatform>();
                    count++;
                }
            }
        }
        if (count > 0) Debug.Log($"Setup {count} disappearing platforms.");
    }

    static void SetupScoreSystem(UIManager uiManager)
    {
        // 1. Create ScoreManager
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            GameObject scoreGO = new GameObject("ScoreManager");
            scoreManager = scoreGO.AddComponent<ScoreManager>();
            Debug.Log("Created ScoreManager.");
        }
        scoreManager.uiManager = uiManager;

        // 2. Create Score UI if missing
        if (uiManager != null && uiManager.scoreText == null)
        {
            GameObject canvasGO = uiManager.transform.parent.gameObject; // Assumes HealthManager is child of Canvas
            if (canvasGO.GetComponent<Canvas>() == null) canvasGO = FindObjectOfType<Canvas>().gameObject;

            GameObject scoreTextGO = new GameObject("ScoreText");
            scoreTextGO.transform.SetParent(canvasGO.transform, false);

            Text text = scoreTextGO.AddComponent<Text>();
            text.text = "Score: 0";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Fallback font
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperRight;

            // Position Top-Right
            RectTransform rt = scoreTextGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-20, -20);
            rt.sizeDelta = new Vector2(200, 50);

            uiManager.scoreText = text;
            Debug.Log("Created Score UI Text.");
        }
    }

    static void SetupCollectibles()
    {
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            string lowerName = go.name.ToLower();
            
            // Check if any parent up the chain is named "Items"
            bool isUnderItems = false;
            Transform current = go.transform.parent;
            while (current != null)
            {
                if (current.name == "Items")
                {
                    isUnderItems = true;
                    break;
                }
                current = current.parent;
            }

            // Expanded name check to include "banan" and other variations
            if (lowerName.Contains("apple") || lowerName.Contains("banan") || lowerName.Contains("fruit") || 
                lowerName.Contains("cherry") || lowerName.Contains("melon") || lowerName.Contains("pineapple") || 
                lowerName.Contains("kiwi") || lowerName.Contains("orange") || isUnderItems)
            {
                if (go.GetComponent<RectTransform>() != null) continue; // Skip UI
                if (go.GetComponent<ParticleSystem>() != null) continue; // Skip particles

                // Add Collectible script
                if (go.GetComponent<Collectible>() == null)
                {
                    go.AddComponent<Collectible>();
                    count++;
                }

                // Ensure Collider is Trigger
                Collider2D col = go.GetComponent<Collider2D>();
                if (col == null)
                {
                    // Try to fit the collider to the sprite if possible
                    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        if (go.GetComponent<CircleCollider2D>() == null && go.GetComponent<BoxCollider2D>() == null && go.GetComponent<CapsuleCollider2D>() == null)
                        {
                            col = go.AddComponent<CircleCollider2D>();
                        }
                    }
                    else
                    {
                         col = go.AddComponent<BoxCollider2D>();
                    }
                }
                
                if (col != null)
                {
                     col.isTrigger = true;
                }
            }
        }
        if (count > 0) Debug.Log($"Setup {count} collectibles.");
    }

    static void SetupPlayer()
    {
        // Find player by the controller script we know exists
        PlayerControllerUpdate playerController = FindObjectOfType<PlayerControllerUpdate>();
        if (playerController != null)
        {
            GameObject playerGO = playerController.gameObject;
            
            // Ensure Player tag is set (if not already)
            if (playerGO.CompareTag("Untagged"))
            {
                playerGO.tag = "Player";
                Debug.Log("Set Player tag to 'Player'.");
            }

            PlayerHealth health = playerGO.GetComponent<PlayerHealth>();
            if (health == null)
            {
                health = playerGO.AddComponent<PlayerHealth>();
                Debug.Log("Added PlayerHealth component to Player.");
            }
            
            // Try to find audio clips if possible (Best effort)
            // In a real editor script we would use AssetDatabase, but here we are at runtime.
            // We'll leave them null and warn the user.
            if (health.damageSound == null || health.deathSound == null)
            {
                Debug.LogWarning("Please assign Damage and Death sounds to PlayerHealth component in Inspector.");
            }
        }
        else
        {
            Debug.LogError("Could not find Player with PlayerControllerUpdate script!");
        }
    }

    static UIManager SetupUI()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null) return uiManager;

        // Check for Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            Debug.Log("Created new Canvas.");
        }

        // Create HealthManager
        GameObject healthManagerGO = new GameObject("HealthManager");
        healthManagerGO.transform.SetParent(canvas.transform, false);
        
        // Position top-left
        RectTransform rt = healthManagerGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        
        // Add Horizontal Layout Group for hearts
        HorizontalLayoutGroup layout = healthManagerGO.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.spacing = 10;

        // Add Content Size Fitter
        ContentSizeFitter csf = healthManagerGO.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        csf.verticalFit = ContentSizeFitter.FitMode.MinSize;

        uiManager = healthManagerGO.AddComponent<UIManager>();
        uiManager.hearts = new Image[3];

        // Create 3 Hearts
        for (int i = 0; i < 3; i++)
        {
            GameObject heartGO = new GameObject($"Heart_{i}");
            heartGO.transform.SetParent(healthManagerGO.transform, false);
            
            Image heartImg = heartGO.AddComponent<Image>();
            heartImg.color = Color.red; // Default to red square if no sprite
            
            RectTransform heartRT = heartGO.GetComponent<RectTransform>();
            heartRT.sizeDelta = new Vector2(32, 32);

            uiManager.hearts[i] = heartImg;
        }
        
        Debug.Log("Created Health UI.");
        return uiManager;
    }

    static void SetupHazards()
    {
        // Find all objects in scene
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int hazardCount = 0;

        foreach (GameObject go in allObjects)
        {
            string lowerName = go.name.ToLower();
            bool isTrapParent = go.transform.parent != null && go.transform.parent.name == "Traps";
            
            if (lowerName.Contains("spike") || lowerName.Contains("shadow") || lowerName.Contains("trap") || lowerName.Contains("saw") || lowerName.Contains("ship") || isTrapParent)
            {
                // Avoid adding to self or UI
                if (go.GetComponent<RectTransform>() != null) continue;

                // Special handling for Tilemaps
                UnityEngine.Tilemaps.Tilemap tilemap = go.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                if (tilemap != null)
                {
                    if (go.GetComponent<Collider2D>() == null)
                    {
                        go.AddComponent<UnityEngine.Tilemaps.TilemapCollider2D>();
                        Debug.Log($"Added TilemapCollider2D to {go.name}");
                    }
                }

                // Add to main object if it has collider
                if (go.GetComponent<Collider2D>() != null)
                {
                    if (go.GetComponent<Hazard>() == null) go.AddComponent<Hazard>();
                    hazardCount++;
                }

                // ALSO check children for colliders (e.g. Saw -> Sprite with collider)
                foreach(Collider2D childCol in go.GetComponentsInChildren<Collider2D>())
                {
                    if (childCol.gameObject.GetComponent<Hazard>() == null)
                    {
                        childCol.gameObject.AddComponent<Hazard>();
                        hazardCount++;
                    }
                }
            }
        }
        
        if (hazardCount > 0)
            Debug.Log($"Added Hazard component to {hazardCount} objects.");
    }
}
