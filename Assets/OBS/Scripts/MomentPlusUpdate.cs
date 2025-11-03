using UnityEngine;
//ruch z przyspieszeniem i hamowaniem,
//sprint przytrzymujac Shift,
//podwojny skok,
//obrot sprite w kierunku ruchu,
//ograniczenie maksymalnej predkosci.
public class PlayerControllerUpdate : MonoBehaviour
{
    public float moveSpeed = 5;
    public float maxSpeed = 15;
    public float jumpForce = 20;
    public float runSpeed = 1.8f;
    public float groundFast = 11;
    public float groundSlow = 18;
    public float airFast = 5;
    public float airSlow = 9;
    private float moveInput = 0;
    private int jumpMax = 2;
    private int jumpOst;
    private bool jumpPressed;
    private bool sprint;
    private bool isDoubleJump;
    private bool jumpLock;
    private bool setGround;




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
    }


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

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




        bool grand = groundChecker != null && groundChecker.isGrounded;

        if (grand && !setGround)
        {
            jumpOst = jumpMax;
            isDoubleJump = false;
            Debug.Log(" Приземление зарегистрировано, IsGrounded = true");

            if (animator != null)
            {
                animator.SetBool("IsDoubleJump", false);
                animator.SetBool("IsGrounded", true);
            }
        }



        if (jumpPressed && jumpOst > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (animator != null)
            {
                animator.SetBool("IsGrounded", false);
            }

            jumpOst--;

            if (jumpOst == 0)
            {
                isDoubleJump = true;
                if (animator != null)
                    animator.SetBool("IsDoubleJump", true);
            }
            else
            {
                isDoubleJump = false;
                if (animator != null)
                    animator.SetBool("IsDoubleJump", false);
            }
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
        
        setGround = grand;
        jumpPressed = false;    
    }



    public void maxVelocity()
    {
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
    }
}