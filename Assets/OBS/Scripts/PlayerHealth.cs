using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    
    [Header("Invincibility")]
    public float iFrameDuration = 1.5f;
    public float flashInterval = 0.1f;
    private bool isInvincible = false;
    
    [Header("References")]
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerControllerUpdate playerController;
    public UIManager uiManager;

    [Header("Audio")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerControllerUpdate>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Try to find UIManager if not assigned
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
            
        UpdateUI();
    }

    // Handle collisions directly if Hazard script is missing on the other object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDamage(collision.gameObject);
    }

    private void CheckDamage(GameObject obj)
    {
        // Debug logging to verify collisions
        // Debug.Log($"Checking damage for: {obj.name} (Parent: {(obj.transform.parent != null ? obj.transform.parent.name : "null")})");

        // 1. Check for Hazard component on object OR parent
        Hazard hazard = obj.GetComponent<Hazard>();
        if (hazard == null)
        {
            hazard = obj.GetComponentInParent<Hazard>();
        }

        if (hazard != null)
        {
            TakeDamage(hazard.damageAmount);
            return;
        }

        // 2. Fallback: Name check (Safe, no errors) - Check object AND parent name
        string lowerName = obj.name.ToLower();
        string parentName = obj.transform.parent != null ? obj.transform.parent.name.ToLower() : "";
        
        if (lowerName.Contains("spike") || lowerName.Contains("shadow") || lowerName.Contains("trap") || lowerName.Contains("saw") || lowerName.Contains("ship") ||
            parentName.Contains("spike") || parentName.Contains("shadow") || parentName.Contains("trap") || parentName.Contains("saw") || parentName.Contains("ship"))
        {
            TakeDamage(1);
            return;
        }

        // 3. Unsafe Tag check removed to prevent "Tag not defined" errors
        // If you define tags "Trap" or "Enemy" in the editor later, you can re-enable:
        /*
        try {
            if (obj.CompareTag("Trap") || obj.CompareTag("Enemy")) TakeDamage(1);
        } catch {}
        */
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        isInvincible = true; // Set immediately to prevent double-hits in same frame
        currentHealth -= damage;
        Debug.Log($"Player took damage! Current Health: {currentHealth}");
        
        if (damageSound != null)
            audioSource.PlayOneShot(damageSound);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth);
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);
            
        if (animator != null)
            animator.SetTrigger("Die"); 

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        if (playerController != null) playerController.enabled = false;
        
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; 
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(1f); 

        currentHealth = maxHealth;
        UpdateUI();
        transform.position = startPosition;
        rb.simulated = true;
        spriteRenderer.enabled = true;
        
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        
        if (playerController != null) playerController.enabled = true;
        isInvincible = false; // Ensure invincibility is reset on respawn
    }

    IEnumerator InvincibilityRoutine()
    {
        // isInvincible = true; // Already set in TakeDamage
        float elapsed = 0f;
        
        while (elapsed < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        
        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
