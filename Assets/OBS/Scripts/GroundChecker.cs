using UnityEngine;
/*** A script on a child of player character
* It should have a small BoxCollider2D set to trigger to just touch the ground
***/
public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;
    public BoxCollider2D colliderS;
    public void Start()
    {
        colliderS = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        //Empty
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }


}