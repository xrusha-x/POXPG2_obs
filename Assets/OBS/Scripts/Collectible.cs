using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10;
    public GameObject pickupEffect; // Optional particle prefab

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug log to see what is colliding
        Debug.Log($"Collectible {gameObject.name} collided with {collision.gameObject.name} (Tag: {collision.tag})");

        // Check for Player tag OR specific components that identify the player (including on parent)
        if (collision.CompareTag("Player") || 
            collision.GetComponent<PlayerControllerUpdate>() != null || 
            collision.GetComponentInParent<PlayerControllerUpdate>() != null ||
            collision.GetComponent<PlayerHealth>() != null ||
            collision.GetComponentInParent<PlayerHealth>() != null ||
            collision.name.Contains("Player"))
        {
            Debug.Log("Player detected! collecting item...");
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue);
            }
            else
            {
                Debug.LogError("ScoreManager instance is null!");
            }
            
            // Optional: Play sound or effect here
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
