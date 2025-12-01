using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;
    public BoxCollider2D colliderS;
    public Collider2D groundCollider;

    void Start()
    {
        colliderS = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        groundCollider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (groundCollider == collision)
            groundCollider = null;

        isGrounded = false;
    }
}
