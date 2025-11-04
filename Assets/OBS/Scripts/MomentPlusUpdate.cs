using UnityEngine;
//ruch z przyspieszeniem i hamowaniem,
//sprint przytrzymujac Shift,
//podwojny skok,
//obrot sprite w kierunku ruchu,
//ograniczenie maksymalnej predkosci.
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
    private bool sprint;
    private bool isDoubleJump;
    private bool jumpLock;
    private bool setGround;
    private bool isWall;
    private bool isSlide;

   
    private float jumpBufferTime = 0.15f;
    private float lastJumpPressedTime;
    private float slideBuffer = 0.1f;
    private float lastSlideTime;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public GroundChecker groundChecker;
    public Animator animator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpOst = jumpMax;
        animator = GetComponent<Animator>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastJumpPressedTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !jumpLock)
        {
            jumpPressed = true;
            jumpLock = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpLock = false;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", groundChecker != null && groundChecker.isGrounded);
            animator.SetBool("IsDoubleJump", isDoubleJump);
            animator.SetFloat("YVelocity", rb.linearVelocity.y);
        }

        sprint = Input.GetKey(KeyCode.LeftShift);

        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0;
        }
    }


    void FixedUpdate()
    {
        Vector2 wallCheckDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, wallCheckDir, wallCheckDis, WallLayer);
        isWall = hit.collider != null;

        Debug.DrawRay(transform.position, wallCheckDir * wallCheckDis, Color.cyan);

        bool pressingTowardWall = (spriteRenderer.flipX && moveInput < 0) || (!spriteRenderer.flipX && moveInput > 0);

        if (isWall && !groundChecker.isGrounded && rb.linearVelocity.y < 0 && pressingTowardWall)
        {
            isSlide = true;
            lastSlideTime = Time.time;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -slideSpeed));

            if ((spriteRenderer.flipX && moveInput > 0) || (!spriteRenderer.flipX && moveInput < 0))
            {
                rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            }

            if (animator != null)
            {
                animator.SetBool("isSlide", true);
            }
        }
        else
        {
            if (Time.time - lastSlideTime > slideBuffer)
            {
                isSlide = false;
                if (animator != null)
                {
                    animator.SetBool("isSlide", false);
                }
            }
        }

        bool groundedNow = groundChecker != null && groundChecker.isGrounded;

        if (groundedNow)
        {
            lastGroundedTime = Time.fixedTime;
        }

        bool grand = (Time.fixedTime - lastGroundedTime) <= coyoteTime;

        if (groundedNow)
        {
            jumpOst = jumpMax;

            if (isDoubleJump)
            {
                isDoubleJump = false;
                if (animator != null)
                {
                    animator.SetBool("IsDoubleJump", false);
                }
            }

            if (animator != null)
            {
                animator.SetBool("IsGrounded", true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("IsGrounded", false);
            }
        }

        if ((Time.time - lastJumpPressedTime) <= jumpBufferTime && jumpOst > 0 && (grand || !groundedNow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (animator != null)
            {
                animator.SetBool("IsGrounded", false);
            }

            jumpOst--;

            if (!groundedNow && jumpOst == 0)
            {
                isDoubleJump = true;
                if (animator != null)
                    animator.SetBool("IsDoubleJump", true);
            }

            lastJumpPressedTime = -1;
        }

        float currentSpeedX = rb.linearVelocity.x;
        bool grounded = groundChecker != null && groundChecker.isGrounded;
        float coreSpeed = moveInput * (sprint ? moveSpeed * runSpeed : moveSpeed);
        float accel = grounded ? groundFast : airFast;
        float decel = grounded ? groundSlow : airSlow;

        if (Mathf.Abs(coreSpeed) > 0.01f)
        {
            currentSpeedX = Mathf.MoveTowards(rb.linearVelocity.x, coreSpeed, accel * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeedX = Mathf.MoveTowards(rb.linearVelocity.x, 0, decel * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeedX, rb.linearVelocity.y);

        maxVelocity();

        if (!groundChecker.isGrounded && rb.linearVelocity.y <= 0 && !jumpPressed)
        {
            rb.AddForce(Vector2.down * 3f);
        }

        setGround = grand;
        jumpPressed = false;
    }



    public void maxVelocity()
    {
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
    }
}
