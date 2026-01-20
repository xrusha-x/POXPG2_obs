using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Animation Settings")]
    [Tooltip("How high the door should move up when opening.")]
    public float openHeight = 3f;
    [Tooltip("The speed at which the door opens.")]
    public float openSpeed = 2f;

    private bool isOpening = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void OpenDoor()
    {
        if (!isOpening)
        {
            StartCoroutine(AnimateDoorOpen());
        }
    }

    private System.Collections.IEnumerator AnimateDoorOpen()
    {
        isOpening = true;

        Vector3 targetPosition = initialPosition + new Vector3(0, openHeight, 0);

        while (transform.position.y < targetPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
    }
}
