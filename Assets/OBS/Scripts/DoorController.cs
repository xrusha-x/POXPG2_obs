using UnityEngine;

/// <summary>
/// Attach this script to any GameObject that should function as a door.
/// It provides a public method to disable the object, making it "vanish".
/// </summary>
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

    /// <summary>
    /// Deactivates the door GameObject, effectively making it disappear.
    /// </summary>
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
        Debug.Log($"Door '{gameObject.name}' has started opening.");

        Vector3 targetPosition = initialPosition + new Vector3(0, openHeight, 0);

        while (transform.position.y < targetPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the door reaches the exact target position
        transform.position = targetPosition;
        Debug.Log($"Door '{gameObject.name}' has finished opening.");

        // Optionally, you can disable the door after it has opened and moved out of sight
        // gameObject.SetActive(false);
    }
}
