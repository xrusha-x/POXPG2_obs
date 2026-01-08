using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this to a button object. When the player touches it,
/// it will open the linked door after showing it with camera.
/// </summary>
public class ButtonController : MonoBehaviour
{
    [Header("Door to Control")]
    [Tooltip("Drag the door GameObject you want this button to open here.")]
    public DoorController targetDoor;

    [Header("Button Visuals")]
    [Tooltip("The sprite to show when the button is pressed.")]
    public Sprite pressedSprite;
    private Sprite originalSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Camera Settings")]
    [Tooltip("Time to show the door with camera before opening it.")]
    public float cameraShowTime = 1.5f;
    [Tooltip("How fast the camera moves to the door.")]
    public float cameraMoveSpeed = 8f;
    [Tooltip("Custom camera position to move to before opening door (optional). If not set, camera will focus on the door itself.")]
    public Transform customCameraTarget;

    private bool isPressed = false;
    private Camera mainCamera;
    private Transform originalCameraTarget;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
        
        mainCamera = FindObjectOfType<Camera>();
        if (mainCamera != null)
        {
            originalCameraTarget = mainCamera.player;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ensure the button hasn't already been pressed and the collider is the player
        if (isPressed || !IsPlayer(collision.gameObject)) return;

        PressButton();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Support for trigger-based buttons
        if (isPressed || !IsPlayer(collision.gameObject)) return;

        PressButton();
    }

    private bool IsPlayer(GameObject obj)
    {
        // Helper function to check if the object is the player
        return obj.CompareTag("Player") || obj.GetComponent<PlayerControllerUpdate>() != null;
    }

    private void PressButton()
    {
        isPressed = true;
        Debug.Log($"Button '{gameObject.name}' pressed.");

        // Change sprite to pressed state
        if (spriteRenderer != null && pressedSprite != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }

        // Start camera sequence
        StartCoroutine(CameraSequence());
    }

    private IEnumerator CameraSequence()
    {
        // Only proceed if we have a target door and camera
        if (targetDoor == null || mainCamera == null)
        {
            Debug.LogWarning($"Button '{gameObject.name}' missing target door or camera.");
            yield break;
        }

        // Determine camera target - use custom position if specified, otherwise use door
        Transform cameraTarget = customCameraTarget != null ? customCameraTarget : targetDoor.transform;
        
        // Switch camera to show the target
        mainCamera.player = cameraTarget;
        Debug.Log($"Camera switching to show: {cameraTarget.name}");

        // Wait 0.5 seconds before opening the door
        yield return new WaitForSeconds(0.5f);

        // Open the door while camera is still looking
        targetDoor.OpenDoor();

        // Wait for the rest of the show time (ensure we don't wait negative time)
        float remainingTime = Mathf.Max(0f, cameraShowTime - 0.5f);
        yield return new WaitForSeconds(remainingTime);

        // Return camera to player
        if (originalCameraTarget != null)
        {
            mainCamera.player = originalCameraTarget;
            Debug.Log("Camera returned to player.");
        }
    }

    /// <summary>
    /// Sets a custom camera target position for the button press sequence.
    /// </summary>
    /// <param name="target">Transform to move camera to before opening door</param>
    public void SetCustomCameraTarget(Transform target)
    {
        customCameraTarget = target;
    }

    /// <summary>
    /// Clears the custom camera target, making the camera focus on the door itself.
    /// </summary>
    public void ClearCustomCameraTarget()
    {
        customCameraTarget = null;
    }
}
