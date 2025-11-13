using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float jumpForce = 12.0f;

    private Rigidbody2D rb;
    private bool isGrounded = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // flip sprite based on direction
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // animation logic
            animator.SetBool("isJumping", true);
            animator.SetBool("inAir", true);
            isGrounded = false;
        }

        // update animations depending on state
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("inAir", false);
            animator.SetBool("isWalking", Mathf.Abs(horizontalInput) > 0);
        }
        else
        {
            // airborne animations stay as "inAir"
            animator.SetBool("isWalking", false);
        }
    }

    void FixedUpdate()
    {
        // Allow movement in the air as well as on ground
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGrounded)
        {
            isGrounded = true;

            animator.SetBool("isJumping", false);
            animator.SetBool("inAir", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isGrounded = false;
        animator.SetBool("inAir", true);
        animator.SetBool("isWalking", false);
    }
}
