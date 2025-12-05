using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats Movestats;
    [SerializeField] private Collider2D _feetcoll;
    [SerializeField] private Collider2D _bodycoll;

    private Rigidbody2D _rb;

    //movement vars
    private Vector2 _moveVelocity;
    private bool _isFacingRight = true;
    private bool _isGrounded;

    private RaycastHit2D _groundHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CollisionChecks();

        float acceleration = _isGrounded ? Movestats.GroundAcceleration : Movestats.AirAcceleration;
        float deceleration = _isGrounded ? Movestats.GroundDeceleration : Movestats.AirDeceleration;

        Move(acceleration, deceleration, InputManager.Movement);
    }

    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            float speed = InputManager.RunIsHeld ? Movestats.MaxRunSpeed : Movestats.MaxWalkSpeed;

            Vector2 targetVelocity = new Vector2(moveInput.x * speed, _rb.linearVelocity.y);

            _moveVelocity = Vector2.Lerp(
                new Vector2(_rb.linearVelocity.x, 0f),
                new Vector2(targetVelocity.x, 0f),
                acceleration * Time.fixedDeltaTime
            );

            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
        else
        {
            // Smooth stop
            _moveVelocity = Vector2.Lerp(
                new Vector2(_rb.linearVelocity.x, 0f),
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );

            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        _isFacingRight = turnRight;
        transform.Rotate(0f, 180f, 0f);
    }
    #endregion

    #region Collision Checks
    private void CheckGround()
    {
        Vector2 origin = new Vector2(_feetcoll.bounds.center.x, _feetcoll.bounds.min.y);
        Vector2 size = new Vector2(_feetcoll.bounds.size.x, Movestats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            0f,
            Movestats.GroundLayer
        );

        _isGrounded = _groundHit.collider != null;
    }

    private void CollisionChecks()
    {
        CheckGround();
    }
    #endregion
}
