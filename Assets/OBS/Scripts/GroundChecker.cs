using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;
    public BoxCollider2D colliderS;
    
    // Use a list to track all colliders we are currently touching
    private List<Collider2D> currentGroundColliders = new List<Collider2D>();

    // Property to maintain compatibility with PlayerControllerUpdate
    public Collider2D groundCollider 
    {
        get 
        {
            if (currentGroundColliders.Count > 0)
                return currentGroundColliders[currentGroundColliders.Count - 1];
            return null;
        }
        // Setter kept for serialization/inspector debug, but logic should rely on triggers
        set 
        { 
            // no-op or clear list? Better to just ignore set for now or clear/add.
            // But original script set it in OnTriggerEnter.
        }
    }

    void Start()
    {
        colliderS = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return; // Ignore triggers

        if (!currentGroundColliders.Contains(collision))
        {
            currentGroundColliders.Add(collision);
        }
        isGrounded = currentGroundColliders.Count > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentGroundColliders.Contains(collision))
        {
            currentGroundColliders.Remove(collision);
        }
        isGrounded = currentGroundColliders.Count > 0;
    }
}
