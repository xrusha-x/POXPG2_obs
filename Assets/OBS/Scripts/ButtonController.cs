using UnityEngine;
using System.Collections;

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
        if (isPressed || !IsPlayer(collision.gameObject)) return;

        PressButton();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPressed || !IsPlayer(collision.gameObject)) return;

        PressButton();
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj == null) return false;

        if (obj.CompareTag("Player")) return true;

        if (obj.GetComponent<PlayerControllerUpdate>() != null) return true;
        if (obj.GetComponentInParent<PlayerControllerUpdate>() != null) return true;

        Transform root = obj.transform != null ? obj.transform.root : null;
        if (root != null && root.CompareTag("Player")) return true;

        return obj.name.Contains("Player");
    }

    private void PressButton()
    {
        isPressed = true;

        if (spriteRenderer != null && pressedSprite != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }

        StartCoroutine(CameraSequence());
    }

    private IEnumerator CameraSequence()
    {
        if (targetDoor == null || mainCamera == null)
        {
            yield break;
        }

        Transform cameraTarget = customCameraTarget != null ? customCameraTarget : targetDoor.transform;
        
        mainCamera.player = cameraTarget;

        yield return new WaitForSeconds(0.5f);

        targetDoor.OpenDoor();

        float remainingTime = Mathf.Max(0f, cameraShowTime - 0.5f);
        yield return new WaitForSeconds(remainingTime);

        if (originalCameraTarget != null)
        {
            mainCamera.player = originalCameraTarget;
        }
    }

    public void SetCustomCameraTarget(Transform target)
    {
        customCameraTarget = target;
    }

    public void ClearCustomCameraTarget()
    {
        customCameraTarget = null;
    }
}
