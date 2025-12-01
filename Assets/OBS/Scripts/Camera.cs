using UnityEngine;

public class Camera : MonoBehaviour
{
    public enum CameraMode
    {
        SimpleFollow,
        SmoothFollow,
        DeadZone,
        LookAhead
    }

    public CameraMode mode = CameraMode.SimpleFollow;
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothTime = 0.3f;
    public Vector2 deadZone = new Vector2(1f, 1f);
    public float lookAheadDistance = 2f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastPlayerPos;
    private Vector3 lookAheadPos = Vector3.zero;

    void Start()
    {
        if (player != null)
            lastPlayerPos = player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        switch (mode)
        {
            case CameraMode.SimpleFollow:
                transform.position = player.position + offset;
                break;

            case CameraMode.SmoothFollow:
                transform.position = Vector3.SmoothDamp(transform.position, player.position + offset, ref velocity, smoothTime);
                break;

            case CameraMode.DeadZone:
                Vector3 newPos = transform.position;
                if (Mathf.Abs(player.position.x - transform.position.x) > deadZone.x)
                    newPos.x = player.position.x - Mathf.Sign(player.position.x - transform.position.x) * deadZone.x;
                if (Mathf.Abs(player.position.y - transform.position.y) > deadZone.y)
                    newPos.y = player.position.y - Mathf.Sign(player.position.y - transform.position.y) * deadZone.y;
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
                break;

            case CameraMode.LookAhead:
                float xMove = player.position.x - lastPlayerPos.x;
                float direction = Mathf.Sign(xMove);

                if (Mathf.Abs(xMove) > 0.01f)
                {
                    lookAheadPos = Vector3.Lerp(
                        lookAheadPos,
                        new Vector3(direction * lookAheadDistance, 0, 0),
                        Time.deltaTime * 5f
                    );
                }
                else
                {
                    lookAheadPos = Vector3.Lerp(
                        lookAheadPos,
                        Vector3.zero,
                        Time.deltaTime * 5f
                    );
                }

                Vector3 targetPos = player.position + offset + lookAheadPos;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

                lastPlayerPos = player.position;
                break;
        }
    }
}
