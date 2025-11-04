using UnityEngine;

public class TilemapWallTrigger : MonoBehaviour
{
    public TilemapWall tilemapWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tilemapWall.ActivateWall();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tilemapWall.DeactivateWall();
        }
    }
}
