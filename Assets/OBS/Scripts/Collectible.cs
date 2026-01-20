using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10;
    public GameObject pickupEffect;
    private bool collected = false;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || 
            collision.GetComponent<PlayerControllerUpdate>() != null || 
            collision.GetComponentInParent<PlayerControllerUpdate>() != null ||
            collision.GetComponent<PlayerHealth>() != null ||
            collision.GetComponentInParent<PlayerHealth>() != null ||
            collision.name.Contains("Player"))
        {
            if (collected) return;
            collected = true;
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue);
            }
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player") ||
            other.GetComponent<PlayerControllerUpdate>() != null ||
            other.GetComponentInParent<PlayerControllerUpdate>() != null ||
            other.GetComponent<PlayerHealth>() != null ||
            other.GetComponentInParent<PlayerHealth>() != null ||
            other.name.Contains("Player"))
        {
            OnTriggerEnter2D(other.GetComponent<Collider2D>());
        }
    }
}
