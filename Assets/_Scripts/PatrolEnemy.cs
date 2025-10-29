using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;

    private Vector2 startPosition;
    private bool movingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        Patrol();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Turn around when hitting a wall or obstacle
        if (!collision.gameObject.CompareTag("Ground"))
        {
            movingRight = !movingRight;
        }
    }
}
