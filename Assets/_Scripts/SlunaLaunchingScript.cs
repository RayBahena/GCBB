using System.Collections;
using UnityEngine;

public class SlunaLaunchingScript : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Launch Settings")]
    public float detectionRange = 5f;
    public float launchForce = 10f;
    public float arcHeight = 2f; // How high the arc goes
    public float launchCooldown = 2f;
    public int damage = 1;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Vector2 startPosition;
    private bool movingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private GameObject player;
    private bool isGrounded = true;
    private bool canLaunch = true;
    private float launchTimer = 0f;

    private enum EnemyState { Patrolling, Launching, Airborne, Landed }
    private EnemyState currentState = EnemyState.Patrolling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;
        
        // Set initial animation state
        animator.SetBool("isWalking", true);
        animator.SetBool("isLaunching", false);
    }

    void Update()
    {
        CheckGrounded();
        
        // Update cooldown timer
        if (!canLaunch)
        {
            launchTimer += Time.deltaTime;
            if (launchTimer >= launchCooldown)
            {
                canLaunch = true;
                launchTimer = 0f;
            }
        }

        // Check if player is in range and we can launch
        if (currentState == EnemyState.Patrolling && canLaunch && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                LaunchAtPlayer();
            }
        }

        // If we've landed after launching, return to patrol
        if (currentState == EnemyState.Airborne && isGrounded)
        {
            currentState = EnemyState.Landed;
            StartCoroutine(ReturnToPatrol());
        }
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Patrolling)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float distanceFromStart = transform.position.x - startPosition.x;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            sr.flipX = false;

            if (distanceFromStart >= patrolDistance)
                movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            sr.flipX = true;

            if (distanceFromStart <= -patrolDistance)
                movingRight = true;
        }
    }

    void LaunchAtPlayer()
    {
        if (player == null) return;

        currentState = EnemyState.Launching;
        canLaunch = false;

        // Set animations
        animator.SetBool("isWalking", false);
        animator.SetBool("isLaunching", true);

        // Calculate direction to player
        Vector2 direction = (player.transform.position - transform.position).normalized;
        
        // Calculate launch velocity with arc
        float distance = Vector2.Distance(transform.position, player.transform.position);
        float angle = 45f; // Launch angle in degrees
        
        // Physics calculation for projectile motion
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float radianAngle = angle * Mathf.Deg2Rad;
        
        // Calculate required velocity to reach player position with arc
        float velocityMagnitude = Mathf.Sqrt((distance * gravity) / Mathf.Sin(2 * radianAngle));
        
        // Apply velocity in the direction of player with upward arc
        Vector2 launchVelocity = new Vector2(
            direction.x * velocityMagnitude * Mathf.Cos(radianAngle),
            velocityMagnitude * Mathf.Sin(radianAngle)
        );

        rb.linearVelocity = launchVelocity;
        
        // Flip sprite to face launch direction
        sr.flipX = direction.x < 0;
        
        currentState = EnemyState.Airborne;
    }

    void CheckGrounded()
    {
        // Raycast downward to check if grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    IEnumerator ReturnToPatrol()
    {
        // Wait a moment before returning to patrol
        yield return new WaitForSeconds(0.5f);
        
        // Reset launch animation
        animator.SetBool("isLaunching", false);
        
        // Check if player is still in range
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (distanceToPlayer <= detectionRange && canLaunch)
            {
                // Player still in range, launch again
                LaunchAtPlayer();
            }
            else
            {
                // Return to patrol
                currentState = EnemyState.Patrolling;
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            animator.SetBool("isWalking", true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Deal damage to player
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            // If we hit player while airborne, transition to landed state
            if (currentState == EnemyState.Airborne)
            {
                currentState = EnemyState.Landed;
                animator.SetBool("isLaunching", false);
                StartCoroutine(ReturnToPatrol());
            }
        }

        // Turn around when hitting walls while patrolling
        if (currentState == EnemyState.Patrolling && !collision.gameObject.CompareTag("Ground"))
        {
            movingRight = !movingRight;
        }

        // Die when hit by bullets
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}