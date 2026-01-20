using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth = 3;
    public float hitFlashDuration = 0.15f;
    public Color hitColor = new Color(1f, 0.5f, 0.5f, 1f);
    public AudioClip hitSound;
    public AudioClip deathSound;

    SpriteRenderer sr;
    AudioSource audioSource;
    Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        originalColor = sr != null ? sr.color : Color.white;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        if (hitSound != null) audioSource.PlayOneShot(hitSound);
        if (sr != null) StartCoroutine(FlashRoutine());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRoutine()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        sr.color = originalColor;
    }

    void Die()
    {
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
        Destroy(gameObject);
    }
}
