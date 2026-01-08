using UnityEngine;
using System.Collections;

public class PlayerControllerUpdate : MonoBehaviour
{
    public LayerMask WallLayer;
    public float moveSpeed = 5;
    public float maxSpeed = 15;
    public float jumpForce = 20;
    public float runSpeed = 1.8f;
    public float groundFast = 11;
    public float groundSlow = 18;
    public float airFast = 5;
    public float airSlow = 9;
    public float wallCheckDis = 0.05f;
    public float slideSpeed = 2f;

    private float moveInput = 0;
    private float coyoteTime = 0.1f;
    private float lastGroundedTime;
    private int jumpMax = 2;
    private int jumpOst;
    private bool jumpPressed;
    private bool isDoubleJump;
    private bool isWall;
    private bool isSlide;

    private float jumpBufferTime = 0.15f;
    private float lastJumpPressedTime;
    private float slideBuffer = 0.1f;
    private float lastSlideTime;

    private bool firstFrame = true;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public GroundChecker groundChecker;
    public Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.linearVelocity = Vector2.zero;
        Physics2D.queriesStartInColliders = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        jumpOst = jumpMax;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Create and apply zero friction material to prevent sticking to walls/platforms
        PhysicsMaterial2D zeroFriction = new PhysicsMaterial2D("ZeroFriction");
        zeroFriction.friction = 0f;
        zeroFriction.bounciness = 0f;
        
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach(var c in cols)
        {
            c.sharedMaterial = zeroFriction;
        }
        if (rb != null)
        {
            rb.sharedMaterial = zeroFriction;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastJumpPressedTime = Time.time;
            jumpPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
            jumpPressed = false;

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", groundChecker != null && groundChecker.isGrounded);
            animator.SetBool("IsDoubleJump", isDoubleJump);
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && groundChecker.isGrounded)
        {
            if (groundChecker.groundCollider != null)
            {
                PlatformEffector2D effector = groundChecker.groundCollider.GetComponent<PlatformEffector2D>();
                if (effector != null)
                    StartCoroutine(DisableEffector(effector));
            }
        }
    }

    void FixedUpdate()
    {
        if (firstFrame)
        {
            rb.linearVelocity = Vector2.zero;
            firstFrame = false;
            return;
        }

        Vector2 wallCheckDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, wallCheckDir, wallCheckDis, WallLayer);
        isWall = hit.collider != null;

        bool groundedNow = groundChecker != null && groundChecker.isGrounded;
        if (groundedNow)
            lastGroundedTime = Time.fixedTime;

        bool coyote = (Time.fixedTime - lastGroundedTime) <= coyoteTime;
        bool pressingTowardWall = (spriteRenderer.flipX && moveInput < 0) || (!spriteRenderer.flipX && moveInput > 0);

        if (isWall && !groundedNow && rb.linearVelocity.y < 0 && pressingTowardWall)
        {
            isSlide = true;
            lastSlideTime = Time.time;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -slideSpeed));

            float hor = moveInput * moveSpeed;
            if ((spriteRenderer.flipX && moveInput > 0) || (!spriteRenderer.flipX && moveInput < 0))
                rb.linearVelocity = new Vector2(hor, rb.linearVelocity.y);

            if (animator != null)
                animator.SetBool("isSlide", true);
        }
        else
        {
            if (Time.time - lastSlideTime > slideBuffer)
            {
                isSlide = false;
                if (animator != null)
                    animator.SetBool("isSlide", false);
            }
        }

        if (groundedNow)
        {
            jumpOst = jumpMax;
            isDoubleJump = false;
        }

        bool canJump = (Time.time - lastJumpPressedTime <= jumpBufferTime) && (coyote || jumpOst > 0);
        if (canJump && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (!groundedNow)
            {
                jumpOst--;
                if (jumpOst <= 0)
                    isDoubleJump = true;
            }

            jumpPressed = false;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? moveSpeed * runSpeed : moveSpeed;
        float targetSpeed = moveInput * currentSpeed;

        float accel = groundedNow
            ? (Mathf.Abs(targetSpeed) > 0.1f ? groundFast : groundSlow)
            : (Mathf.Abs(targetSpeed) > 0.1f ? airFast : airSlow);

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        newX = Mathf.Clamp(newX, -maxSpeed, maxSpeed);

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    IEnumerator DisableEffector(PlatformEffector2D effector)
    {
        effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.4f);
        effector.rotationalOffset = 0f;
    }
}   
