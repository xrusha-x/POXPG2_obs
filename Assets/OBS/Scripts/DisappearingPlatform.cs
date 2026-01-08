using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    private Renderer rend;
    private bool isPlayerOn = false;
    
    [Header("Settings")]
    public float toggleInterval = 5f; // Time between toggles
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            // Try in children if not on root (e.g. if script is on parent of tilemap)
            rend = GetComponentInChildren<Renderer>();
        }
        
        StartCoroutine(ToggleRoutine());
    }

    IEnumerator ToggleRoutine()
    {
        // Random start delay so they don't all blink in perfect sync
        // "random order" simulation
        yield return new WaitForSeconds(Random.Range(0f, toggleInterval));

        while (true)
        {
            // Phase 1: Visible
            SetVisibility(true);
            
            // Wait for interval
            yield return new WaitForSeconds(toggleInterval);

            // Phase 2: Invisible (but check player)
            float timer = 0f;
            while (timer < toggleInterval)
            {
                // If player is on it, FORCE VISIBLE.
                // Otherwise, be INVISIBLE.
                if (isPlayerOn)
                {
                    SetVisibility(true);
                }
                else
                {
                    SetVisibility(false);
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    void SetVisibility(bool visible)
    {
        if (rend != null) rend.enabled = visible;
    }

    // Detect Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayer(collision.gameObject, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CheckPlayer(collision.gameObject, false);
    }
    
    // Support Triggers just in case
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckPlayer(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckPlayer(collision.gameObject, false);
    }

    private void CheckPlayer(GameObject obj, bool state)
    {
        // Check tag or components
        if (obj.CompareTag("Player") || 
            obj.GetComponent<PlayerControllerUpdate>() != null || 
            obj.GetComponent<PlayerHealth>() != null)
        {
            isPlayerOn = state;
        }
    }
}
