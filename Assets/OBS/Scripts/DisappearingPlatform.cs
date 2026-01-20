using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    private Renderer rend;
    private bool isPlayerOn = false;
    
    [Header("Settings")]
    public float toggleInterval = 5f;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            rend = GetComponentInChildren<Renderer>();
        }
        
        StartCoroutine(ToggleRoutine());
    }

    IEnumerator ToggleRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, toggleInterval));

        while (true)
        {
            SetVisibility(true);
            
            yield return new WaitForSeconds(toggleInterval);

            float timer = 0f;
            while (timer < toggleInterval)
            {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayer(collision.gameObject, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CheckPlayer(collision.gameObject, false);
    }
    
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
        if (obj.CompareTag("Player") || 
            obj.GetComponent<PlayerControllerUpdate>() != null || 
            obj.GetComponent<PlayerHealth>() != null)
        {
            isPlayerOn = state;
        }
    }
}
