using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10.0f;

    public float jumpForce = 500.0f;

    Rigidbody2D rb;

    public bool isGrounded = false;

    Animator animator;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime);

        // animate!
        if (horizontalInput > 0)
        {
            animator.SetBool("isRunning", true);
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            animator.SetBool("isRunning", true);
            spriteRenderer.flipX = true;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // jumpy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //push the rigidbody UP
            rb.AddForce(transform.up * jumpForce);

            // animate!
            animator.SetBool("isJumping", true);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        isGrounded = true;
        // animate!
        animator.SetBool("isJumping", false);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isGrounded = false;
    }
}