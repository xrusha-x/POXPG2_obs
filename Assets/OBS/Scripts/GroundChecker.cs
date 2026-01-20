using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;
    public BoxCollider2D colliderS;

    private List<Collider2D> currentGroundColliders = new List<Collider2D>();

    public Collider2D groundCollider 
    {
        get 
        {
            if (currentGroundColliders.Count > 0)
                return currentGroundColliders[currentGroundColliders.Count - 1];
            return null;
        }
        set 
        { 
        }
    }

    void Start()
    {
        colliderS = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

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
