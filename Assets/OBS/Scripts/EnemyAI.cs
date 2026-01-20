using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public class PatrolPoint
    {
        public Vector3 position;
        public float waitTime;
    }

    public PatrolPoint[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float returnSpeed = 3f;
    public float chaseRange = 6f;
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float attackCooldown = 0.8f;
    public AudioClip attackSound;
    public GameObject attackVFX;
    public LayerMask groundMask;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public Vector2 groundCheckOffset = new Vector2(0f, -0.6f);
    public float groundFast = 11f;
    public float groundSlow = 18f;
    public float airFast = 5f;
    public float airSlow = 9f;
    public float maxSpeed = 15f;
    public float attackWindUp = 0.2f;
    public float attackActive = 0.1f;
    public float attackRecovery = 0.3f;

    enum State { Patrolling, Pursuing, Returning }
    State state = State.Patrolling;

    int patrolIndex = 0;
    Transform player;
    float nextAttackTime = 0f;
    Animator animator;
    SpriteRenderer sr;
    Rigidbody2D rb;
    Collider2D[] cols;
    BoxCollider2D mainCollider;
    float waitTimer = 0f;
    bool grounded = false;
    int returnIndex = -1;
    bool isAttacking = false;

    void Awake()
    {
        var ph = FindObjectOfType<PlayerHealth>();
        player = ph != null ? ph.transform : null;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cols = GetComponents<Collider2D>();
        mainCollider = GetComponent<BoxCollider2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        PhysicsMaterial2D zeroFriction = new PhysicsMaterial2D("ZeroFrictionEnemy");
        zeroFriction.friction = 0f;
        zeroFriction.bounciness = 0f;
        foreach (var c in cols) c.sharedMaterial = zeroFriction;
        rb.sharedMaterial = zeroFriction;
    }

    void Update()
    {
        if (player == null) state = State.Patrolling;
        else
        {
            float dist = Vector2.Distance(transform.position, player.position);
            bool inChase = dist <= chaseRange;
            if (state == State.Pursuing && !inChase) state = State.Returning;
            else if (inChase) state = State.Pursuing;
            if (animator != null) animator.SetBool("IsChasing", inChase);
            if (inChase && dist <= attackRange && Time.time >= nextAttackTime) Attack();
        }
        grounded = CheckGrounded();
    }

    void FixedUpdate()
    {
        if (state == State.Pursuing)
        {
            MoveTowardsX(player != null ? player.position.x : transform.position.x, isAttacking ? 0f : chaseSpeed);
            returnIndex = GetNearestPatrolIndex(transform.position);
            if (animator != null) animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
        else if (state == State.Returning)
        {
            if (returnIndex < 0) returnIndex = GetNearestPatrolIndex(transform.position);
            Vector3 target = patrolPoints != null && patrolPoints.Length > 0 ? patrolPoints[returnIndex].position : transform.position;
            MoveTowardsX(target.x, isAttacking ? 0f : returnSpeed);
            if (Mathf.Abs(transform.position.x - target.x) < 0.05f)
            {
                state = State.Patrolling;
                patrolIndex = returnIndex;
                waitTimer = patrolPoints[patrolIndex].waitTime;
                returnIndex = -1;
            }
            if (animator != null) animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
        else
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            if (waitTimer > 0f)
            {
                MoveTowardsX(transform.position.x, 0f);
                waitTimer -= Time.fixedDeltaTime;
                return;
            }
            Vector3 target = patrolPoints[patrolIndex].position;
            MoveTowardsX(target.x, isAttacking ? 0f : patrolSpeed);
            if (Mathf.Abs(transform.position.x - target.x) < 0.05f)
            {
                waitTimer = patrolPoints[patrolIndex].waitTime;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }
            if (animator != null) animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void MoveTowardsX(float targetX, float speed)
    {
        float dir = Mathf.Sign(targetX - transform.position.x);
        float targetSpeed = dir * speed;
        float accel = grounded
            ? (Mathf.Abs(targetSpeed) > 0.1f ? groundFast : groundSlow)
            : (Mathf.Abs(targetSpeed) > 0.1f ? airFast : airSlow);
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        newX = Mathf.Clamp(newX, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        if (sr != null) sr.flipX = dir < 0f;
    }

    void Attack()
    {
        if (isAttacking) return;
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");
        }
        if (attackSound != null)
        {
            var au = GetComponent<AudioSource>();
            if (au == null) au = gameObject.AddComponent<AudioSource>();
            au.PlayOneShot(attackSound);
        }
        if (attackWindUp > 0f) yield return new WaitForSeconds(attackWindUp);
        if (attackVFX != null) Instantiate(attackVFX, transform.position, Quaternion.identity);
        var ph = player != null ? player.GetComponent<PlayerHealth>() : null;
        if (ph != null) ph.TakeDamage(attackDamage);
        if (attackActive > 0f) yield return new WaitForSeconds(attackActive);
        if (attackRecovery > 0f) yield return new WaitForSeconds(attackRecovery);
        if (animator != null) animator.SetBool("IsAttacking", false);
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    bool CheckGrounded()
    {
        Vector2 center = (Vector2)transform.position + groundCheckOffset;
        var hit = Physics2D.OverlapBox(center, groundCheckSize, 0f, groundMask);
        return hit != null;
    }

    int GetNearestPatrolIndex(Vector3 pos)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return -1;
        int idx = 0;
        float best = float.MaxValue;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector2.Distance(pos, patrolPoints[i].position);
            if (d < best)
            {
                best = d;
                idx = i;
            }
        }
        return idx;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Vector3 p = patrolPoints[i].position;
                Gizmos.DrawSphere(p, 0.1f);
                Vector3 np = patrolPoints[(i + 1) % patrolPoints.Length].position;
                Gizmos.DrawLine(p, np);
            }
        }
    }
}
