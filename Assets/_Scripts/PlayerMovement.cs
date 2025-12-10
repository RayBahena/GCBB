using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats Movestats;
    [SerializeField] private Collider2D _feetcoll;
    [SerializeField] private Collider2D _bodycoll;

    private Rigidbody2D _rb;

    //movement vars
    public float HorizontalVelocity { get; private set; }
    private bool _isFacingRight = true;
    private Animator animator;

    //collision Check vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _lastWallhit;
    private bool _isGrounded;
    private bool _bumpedHead;
    private bool _isTouchingWall;


    //jump vars
    public float VerticalVelocity { get; private set;}
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    //coyote time vars
    private float _coyoteTimer;

    //Wall slide vars
    private bool _isWallSliding;
    private bool _isWallSlidingFalling;

    //Wall jump vars
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;
    private float _wallJumpFastFallingTime;
    private float _wallJumpFastFallReleaseSpeed;

    private float _wallJumpPostBufferTimer;

    private float _wallJumpApexPoint;
    private float _timePastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;

    //dash vars
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isdashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update ()
    {
        CountTimers();
        JumpChecks();
        LandCheck();
        WallJumpCheck();
        DashCheck();

        WallSlideCheck();
        
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        Fall();
        WallSlide();
        WallJump();
        Dash();
        
        if (_isGrounded)
        {
            Move(Movestats.GroundAcceleration, Movestats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            //wall jumping
            if (_useWallJumpMoveStats)
            {
                Move(Movestats.WallJumpMoveAcceleration, Movestats.WallJumpMoveDeceleration, InputManager.Movement);
            }

            //airborne
            else
            {
                Move(Movestats.AirAcceleration, Movestats.AirDeceleration, InputManager.Movement);
            }
        }
        ApplyVelocity();

        // Horizontal movement
        animator.SetBool("isWalking", Mathf.Abs(_rb.linearVelocity.x) > 0.1f);

        // Vertical movement / jumping
        animator.SetBool("isJumping", VerticalVelocity > 0.1f); // rising
        animator.SetBool("inAir", !_isGrounded); // any airborne state

    }

    private void ApplyVelocity()
    {
        //CLAMP FALL SPEED
        if (!_isDashing)
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -Movestats.MaxFallSpeed, 50f);
        }

        else
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -50f, 50f);
        }
        _rb.linearVelocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    private void OnDrawGizmos()
    {
        if (Movestats.ShowWalkJumpArc)
        {
            DrawJumpArc(Movestats.MaxWalkSpeed, Color.white);
        }
        if (Movestats.ShowRunJumpArc)
        {
            DrawJumpArc(Movestats.MaxRunSpeed, Color.red);
        }
    }

    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
       if (!_isDashing)
        {
            if (Mathf.Abs(moveInput.x) >= Movestats.MoveThreshold)
            {
                TurnCheck(moveInput);
                float targetVelocity = 0f;
                if (InputManager.RunIsHeld)
                {
                    targetVelocity = moveInput.x * Movestats.MaxRunSpeed;
                }
                else
                {
                    targetVelocity = moveInput.x* Movestats.MaxWalkSpeed;
                }

                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            
            }
            else if (Mathf.Abs(moveInput.x) < Movestats.MoveThreshold)
            {
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
            }
        }
    }

    private void TurnCheck(Vector2 moveInput)
{
    if (moveInput.x > 0 && !_isFacingRight)
        Turn(true);

    else if (moveInput.x < 0 && _isFacingRight)
        Turn(false);
}

private void Turn(bool faceRight)
{
    _isFacingRight = faceRight;

    // Flip by scaling instead of rotating
    Vector3 scale = transform.localScale;
    scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
    transform.localScale = scale;
}

    #endregion

    #region Land/Fall

    private void LandCheck()
    {
        if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlidingFalling || _isWallSliding || _isdashFastFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
         ResetJumpValues();
         StopWallSlide();
         ResetWallJumpValues();
         ResetDashes();
         _numberOfJumpsUsed = 0;
         VerticalVelocity = Physics2D.gravity.y;

        if (_isdashFastFalling && _isGrounded)
            {
                ResetDashValues();
                return;
            }
        ResetDashValues();
        }
    }

        private void Fall()
    {
        if (!_isGrounded && !_isJumping && !_isDashing && !_isWallJumping && !_isWallSliding && !_isdashFastFalling)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += Movestats.Gravity * Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Jump

    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
    }

    private void JumpChecks()
    {
        if (InputManager.JumpWasPressed)
        {
            if (_isWallSlidingFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }

            else if (_isWallSliding || (_isTouchingWall && !_isGrounded)) {
                return;
            }
            _jumpBufferTimer = Movestats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            // Only allow upward cancel on the FIRST jump, not mid-air jumps
            bool isFirstJump = _numberOfJumpsUsed <= 1;

            if (isFirstJump && _isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = Movestats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // Grounded or coyote jump
        if (_jumpBufferTimer > 0f && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        // Mid-air jumps
        else if (_jumpBufferTimer > 0f && !_isGrounded && _numberOfJumpsUsed < Movestats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            _isPastApexThreshold = false;
            InitiateJump(1);
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        ResetWallJumpValues();

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = Movestats.InitialJumpVelocity;
    }

    private void Jump()
    {
        if (_isJumping)
        {
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }
            if ( VerticalVelocity >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(Movestats.InitialJumpVelocity, 0f, VerticalVelocity);
                if (_apexPoint > Movestats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }
                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < Movestats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                else
                {
                    // Apply upward movement gravity
                    VerticalVelocity += Movestats.Gravity * Time.fixedDeltaTime;
                }
            }
            else if (_isFastFalling)
            {
                VerticalVelocity += Movestats.Gravity * Time.fixedDeltaTime;
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                }
            }
        }

        //GRAVITY ON DESCENDING
        else if (!_isFastFalling)
        {
            VerticalVelocity += Movestats.Gravity * Movestats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }
        else if (VerticalVelocity < 0f) {
            if (_isFalling)
            {
                _isFalling = true;
            }
            
        }

        //JUMP CUT
        if (_isFastFalling)
        {
            if (_fastFallTime >= Movestats.TimeForUpwardsCancel)
            {
                VerticalVelocity += Movestats.Gravity * Movestats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < Movestats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / Movestats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

    }

    #endregion

    #region Wall Slide

    private void WallSlideCheck ()
    {
        if (_isTouchingWall && !_isGrounded && !_isDashing)
        {
            if (VerticalVelocity < 0f && !_isWallSliding) 
            {
                ResetJumpValues();
                ResetWallJumpValues();
                ResetDashValues();

                if (Movestats.ResetDashOnWallSlide)
                {
                    ResetDashes();
                }

                _isWallSlidingFalling = false;
                _isWallSliding = true;

                if (Movestats.ResetJumpsOnWallSlide)
                {
                    _numberOfJumpsUsed =0;
                }
            }
        }

        else if (_isWallSliding && !_isTouchingWall && !_isWallSlidingFalling)
        {
            _isWallSlidingFalling = true;
            StopWallSlide();
        }
        else {StopWallSlide();}
    }

    private void StopWallSlide ()
    {
        if (_isWallSliding)
        {
            _numberOfJumpsUsed++;
            _isWallSliding = false;
        }
    }

    private void WallSlide ()
    {
        if (_isWallSliding)
        {
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, -Movestats.WallSlideSpeed, Movestats.WallSlideDecelerationSpeed * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Wall Jump 

    private void WallJumpCheck()
    {
        //if (ShouldApplyPostWallJumpBuffer())
        //{
            //_wallJumpPostBufferTimer = Movestats.WallJumpPostBufferTime;
        //}

        //wall jump fast falling
        if (InputManager.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping)
        {
            if (VerticalVelocity > 0f)
            {
                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallingTime = Movestats.TimeForUpwardsCancel;

                    VerticalVelocity = 0f;
                }

                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //actual jump with post wall jump buffer time
        if (InputManager.JumpWasPressed && _wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();
        }
    }   

    private void InitiateWallJump()
    {
        if (_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        VerticalVelocity = Movestats.InitialWallJumpVelocity;
        int dirMultiplier = 0;
        Vector2 hitPoint = _lastWallhit.collider.ClosestPoint(_bodycoll.bounds.center);

        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier =-1;
        }
        else
        {
            dirMultiplier =1;
        }

        HorizontalVelocity = Mathf.Abs(Movestats.WallJumpDirection.x) * dirMultiplier;

    }


    private void WallJump()
    {
        //APPLY WALL JUMP GRAVITY
        if (_isWallJumping)
        {
            _wallJumpTime += Time.fixedDeltaTime;
            if (_wallJumpTime >= Movestats.TimeTillJumpApex)
            {
                _useWallJumpMoveStats = false;
            }
        }

        //HIT HEAD
        if (_bumpedHead)
        {
            _isWallJumpFastFalling = true;
            _useWallJumpMoveStats = false;
        }

        //GRAVITY IN ASCENDING
        if (VerticalVelocity >= 0f)
        {
            //APEX CONTROLS
            _wallJumpApexPoint = Mathf.InverseLerp(Movestats.WallJumpDirection.y, 0f, VerticalVelocity);
            if (_wallJumpApexPoint > Movestats.ApexThreshold)
            {
                if (!_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = true;
                    _timePastWallJumpApexThreshold = 0f;
                }

                if (_isPastWallJumpApexThreshold)
                {
                    _timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                    if (_timePastWallJumpApexThreshold < Movestats.ApexHangTime)
                    {
                        VerticalVelocity = 0f;
                    }
                }
                else
                {
                    VerticalVelocity = -0.01f;
                }
            } 

            //GRAVITY IN ASCENDING BUT NOT APEX THRESHOLD
            else if (_isWallJumpFastFalling)
            {
                VerticalVelocity = Movestats.WallJumpGravity * Time.fixedDeltaTime;

                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                }
            }           
        }
        else if (!_isWallJumpFastFalling)
        {
            VerticalVelocity = Movestats.WallJumpGravity * Time.fixedDeltaTime;
        }
        else if (VerticalVelocity < 0f)
        {
            if (!_isWallJumpFalling)
            {
                _isWallJumpFalling = true;
            }
        }

        //HANDLE WALL JUMP CUT TIME

        if (_isWallJumpFastFalling)
        {
            if (_wallJumpFastFallingTime >= Movestats.TimeForUpwardsCancel)
            {
                VerticalVelocity += Movestats.WallJumpGravity * Movestats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_wallJumpFastFallingTime < Movestats.TimeForUpwardsCancel) {
                VerticalVelocity = Mathf.Lerp (_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallingTime / Movestats.TimeForUpwardsCancel));
            }

            _wallJumpFastFallingTime += Time.fixedDeltaTime;
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if (!_isGrounded && (_isTouchingWall || _isWallSliding))
        {
            return true;
        }
        else {
            return false;
        }
    }
    private void ResetWallJumpValues()
    {
        _isWallSlidingFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping = false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallingTime = 0f;
        _wallJumpTime = 0f;
    }

    #endregion

    #region Dash

    private void DashCheck ()
    {
        if (InputManager.DashWasPressed)
        {
            //ground dash
            if (_isGrounded && _dashOnGroundTimer < 0 && !_isDashing)
            {
                InitiateDash();

            }
            //air dash
            else if (!_isGrounded && !_isDashing && _numberOfDashesUsed < Movestats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();

                if (_wallJumpPostBufferTimer > 0f)
                {
                    _numberOfJumpsUsed--;
                    if (_numberOfJumpsUsed < 0f)
                    {
                        _numberOfJumpsUsed = 0;
                    }
                }
            }
        }
    }

    private void InitiateDash()
    {
        _dashDirection = InputManager.Movement;
        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, Movestats.DashDirections[0]);

        //skip if we hit
        for (int i = 0; i < Movestats.DashDirections.Length; i++)
{
    float distance = Vector2.Distance(_dashDirection, Movestats.DashDirections[i]);

    // check if it's diagonal
    bool isDiagonal =
        (Mathf.Abs(Movestats.DashDirections[i].x) == 1 &&
         Mathf.Abs(Movestats.DashDirections[i].y) == 1);

    if (isDiagonal)
    {
        distance -= Movestats.DashDiagonallyBias;
    }

    if (distance < minDistance)
    {
        minDistance = distance;
        closestDirection = Movestats.DashDirections[i];
    }
}

        //handle direction w/o input

        if (closestDirection == Vector2.zero)
        {
            if(_isFacingRight)
            {
                closestDirection = Vector2.right;

            }
            else
            {
                closestDirection = Vector2.left;
            }
            
        }
        _dashDirection = closestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = 0f;

        ResetJumpValues();
        ResetWallJumpValues();
        StopWallSlide()
;
    }

    private void Dash()
    {
        if(_isDashing)
        {
            //stop the dash after the timer;
            _dashTimer += Time.fixedDeltaTime;
            if (_dashTimer >= Movestats.DashTime)
            {
                if ( _isGrounded)
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = true;

                if (!_isJumping && !_isWallJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if(!_isGrounded)
                    {
                        _isdashFastFalling = true;
                    }
                }

                return;
            }
            HorizontalVelocity = Movestats.DashSpeed * _dashDirection.x;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                VerticalVelocity = Movestats.DashSpeed * _dashDirection.y;
            }
           
        }

        //handle cut time
        else if(_isdashFastFalling)
        {
            if (VerticalVelocity > 0f)
            {
                VerticalVelocity = Mathf.Lerp (_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / Movestats.DashTimeForUpwardsCancel));

            }
            else if (_dashFastFallTime >= Movestats.DashTimeForUpwardsCancel)
            {
                VerticalVelocity = Movestats.Gravity * Movestats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;

            }
            _dashFastFallTime += Time.fixedDeltaTime;
        }

        else
        {
            VerticalVelocity += Movestats.Gravity * Movestats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }

    }

     private void ResetDashValues ()
    {
        _isdashFastFalling = false;
        _dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes ()
    {
        _numberOfDashesUsed = 0;
    }

    #endregion

    #region Collision Checks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetcoll.bounds.center.x, _feetcoll.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetcoll.bounds.size.x, Movestats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, Movestats.GroundDetectionRayLength, Movestats.GroundLayer);

        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }
    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetcoll.bounds.center.x, _bodycoll.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetcoll.bounds.size.x, Movestats.GroundDetectionRayLength);

        _headHit = Physics2D.BoxCast (boxCastOrigin, boxCastSize, 0f, Vector2.up, Movestats.GroundDetectionRayLength, Movestats.GroundLayer);
        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }
    }

    private void IsTouchingWall()
    {
        float originEndPoint = 0f;
        if (_isFacingRight)
        {
            originEndPoint = _bodycoll.bounds.max.x;
        }
        else { originEndPoint = _bodycoll.bounds.min.x;}

        float adjustedHeight = _bodycoll.bounds.size.y * Movestats.WallDetectionRayHeighMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, _bodycoll.bounds.center.y);
        Vector2 boxCastSize = new Vector2(Movestats.WallDetectionRayLength, adjustedHeight);

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, Movestats.WallDetectionRayLength, Movestats.GroundLayer);

        if (_wallHit.collider != null)
        {
            _lastWallhit = _wallHit;
            _isTouchingWall = true;
        }
        else {_isTouchingWall = false;}
    }
    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
        IsTouchingWall();

    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        //jump buffer
        _jumpBufferTimer -= Time.deltaTime;

        //jump coyote time
        if (_isGrounded)
        {
            _coyoteTimer = Movestats.JumpCoyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        //wall jump buffer timer
        //if (!ShouldApplyPostWallJumpBuffer())
        //{
            //_wallJumpPostBufferTimer -= Time.fixedDeltaTime; 
       // }

        //dash timer
        if (_isGrounded)
        {
            _dashOnGroundTimer += Time.fixedDeltaTime;

        }
        
    }

    #endregion

    private void DrawJumpArc (float moveSpeed, Color gizmocolor)
    {
        Vector2 startPosition = new Vector2 (_feetcoll.bounds.center.x, _feetcoll.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (Movestats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed;}
        Vector2 velocity = new Vector2(speed, Movestats.InitialJumpVelocity);
        Gizmos.color = gizmocolor;
        float timeStep = 2 * Movestats.TimeTillJumpApex / Movestats.ArcResolution;

        for (int i = 0; i < Movestats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < Movestats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2 (0, Movestats.Gravity) * simulationTime * simulationTime;
            }

            else if (simulationTime < Movestats.TimeTillJumpApex + Movestats.ApexHangTime)
            {
                float apexTime = simulationTime - (Movestats.TimeTillJumpApex + Movestats.ApexHangTime);
                displacement = velocity * Movestats.TimeTillJumpApex + 0.5f * new Vector2(0, Movestats.Gravity) * Movestats.TimeTillJumpApex * Movestats.TimeTillJumpApex;
                displacement += new Vector2 (speed, 0) * apexTime;
            }

            else
            {
                float descendTime = simulationTime - (Movestats.TimeTillJumpApex + Movestats.ApexHangTime);
                displacement = velocity * Movestats.TimeTillJumpApex + 0.5f * new Vector2(0, Movestats.Gravity) * Movestats.TimeTillJumpApex * Movestats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * Movestats.ApexHangTime;
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, Movestats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;
            
            if (Movestats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), Movestats.GroundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
}