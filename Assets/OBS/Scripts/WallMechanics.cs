using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapWall : MonoBehaviour
{
    public Transform targetPosition; 
    public float moveSpeed = 2f;     

    private Vector3 startPosition;   
    private bool shouldMoveUp = false;
    private bool shouldMoveDown = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        
        if (shouldMoveUp)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetPosition.position.y, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);

            if (Mathf.Abs(transform.position.y - targetPosition.position.y) < 0.01f)
            {
                shouldMoveUp = false;
            }
        }

        if (shouldMoveDown)
        {
            float newY = Mathf.MoveTowards(transform.position.y, startPosition.y, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);

            if (Mathf.Abs(transform.position.y - startPosition.y) < 0.01f)
            {
                shouldMoveDown = false;
            }
        }
    }

    public void ActivateWall()
    {
        shouldMoveDown = false;
        shouldMoveUp = true;
    }

    public void DeactivateWall()
    {
        shouldMoveUp = false;
        shouldMoveDown = true;
    }
}
