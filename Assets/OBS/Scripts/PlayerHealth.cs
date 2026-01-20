using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static bool TookDamageThisLevel = false;
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
    public CustomHeartsUI heartsUI;

    [Header("Audio")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        TookDamageThisLevel = false;
        currentHealth = maxHealth;
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerControllerUpdate>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        if (heartsUI == null) heartsUI = FindObjectOfType<CustomHeartsUI>();
        if (heartsUI != null) heartsUI.Initialize(maxHealth);
        UpdateUI();
    }

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

        string lowerName = obj.name.ToLower();
        string parentName = obj.transform.parent != null ? obj.transform.parent.name.ToLower() : "";
        
        if (lowerName.Contains("spike") || lowerName.Contains("shadow") || lowerName.Contains("trap") || lowerName.Contains("saw") || lowerName.Contains("ship") ||
            parentName.Contains("spike") || parentName.Contains("shadow") || parentName.Contains("trap") || parentName.Contains("saw") || parentName.Contains("ship"))
        {
            TakeDamage(1);
            return;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        isInvincible = true;
        int oldHealth = currentHealth;
        currentHealth -= damage;
        if (damage > 0) TookDamageThisLevel = true;
        if (damageSound != null)
            audioSource.PlayOneShot(damageSound);

        UpdateUI();
        if (heartsUI != null) heartsUI.PlayLoseFeedbackRange(oldHealth, currentHealth);

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
        if (heartsUI != null)
        {
            heartsUI.SetHealth(currentHealth);
        }
    }

    void Die()
    {
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
        isInvincible = false;
    }

    IEnumerator InvincibilityRoutine()
    {
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
