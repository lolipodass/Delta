using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    [Label("Rigidbody")][SerializeField] private Rigidbody2D rb;
    [Label("Animator")][SerializeField] private Animator animator;
    [Label("Stand Box collider")][SerializeField] private BoxCollider2D standBoxCollider;
    [Label("Crouch Box collider")][SerializeField] private BoxCollider2D crouchBoxCollider;



    [Header("Movement Settings")]
    [Label("Max speed")][SerializeField] private float maxSpeed = 4f;
    [Label("Air Control")][SerializeField][Range(0f, 1f)] private float airControlFactor = 0.98f;
    [Label("Wall slide speed")][SerializeField] private float wallSlideSpeed = 2f;
    [Label("Ground Acceleration")][SerializeField] private float groundAcceleration = 20f;
    [Label("Air Acceleration")][SerializeField] private float airAcceleration = 20f;



    [Header("Jump Settings")]
    [Label("Jump Force")][SerializeField] private float jumpForce = 10f;
    [Label("Wall Jump Y Force")][SerializeField] private float wallJumpYForce = 8f;
    [Label("Wall Jump X Force")][SerializeField] private float wallJumpXForce = 5f;
    [Label("Coyote Time")][Range(0f, 1f)][SerializeField] private float coyoteTime = 0.1f;
    [Label("Jump Buffer Time")][Range(0f, 1f)][SerializeField] private float jumpBufferTime = 0.1f;
    [Label("Minimal Jump time")][Range(0f, 0.4f)][SerializeField] private float minimalJumpTime = 0.1f;
    [Label("Extra jump count")][Range(0, 10)][SerializeField] private int extraJumpCount = 1;
    [Label("Has Wall Jump")][SerializeField] private bool hasWallJump = true;
    [Label("Has Wall Slide")][SerializeField] private bool hasWallSlide = true;



    [Foldout("Ground check")]
    [SerializeField] private Transform groundCheckPos;
    [Foldout("Ground check")]
    [SerializeField] private Vector2 groundCheckSize = new(0.5f, 0.5f);
    [Foldout("Ground check")]
    [SerializeField] private LayerMask groundMask;

    [Foldout("Wall check")]
    [SerializeField] private Transform wallCheckPos;
    [Foldout("Wall check")]
    [SerializeField] private Vector2 wallCheckSize = new(0.5f, 0.5f);
    [Foldout("Wall check")]
    [SerializeField] private LayerMask wallMask;

    [Foldout("Crouch check")]
    [SerializeField] private Transform crouchCheckPos;
    [Foldout("Crouch check")]
    [SerializeField] private Vector2 crouchCheckSize = new(0.5f, 0.5f);
    [Foldout("Crouch check")]
    [SerializeField] private LayerMask crouchMask;

    [Header("Debug")]
    public bool isDebug = false;

    [Button]
    void addForce()
    {
        rb.AddForce(Vector2.up * 20f, ForceMode2D.Impulse);
    }

    static private float maxJumpTime = 0.4f;

    //state
    private int extraJumpCountLeft;
    private bool isFacingRight = true;
    private float moveInput;
    private bool isGrounded;
    private bool isTouchWall;
    private bool isCrouching = false;
    private bool isWallJumping = false;
    private bool isJumping = false;
    private bool isInJumpState = false;
    // private bool isWallSliding = false;
    private bool isHoldJumpButton = false;
    private bool isHoldCrouchButton = false;
    private float xVelocity = 0f;
    private float yVelocity = 0f;

    private static readonly int MagnitudeHash = Animator.StringToHash("magnitude");
    private static readonly int YVelocityHash = Animator.StringToHash("yVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("onGround");
    private static readonly int WallHash = Animator.StringToHash("onWall");
    private static readonly int CrouchHash = Animator.StringToHash("isCrouching");

    //timers
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float jumpTime;

    void Start()
    {
        extraJumpCountLeft = extraJumpCount;
    }

    void Update()
    {
        HandleTimers();
        UpdateAnimations();
        Rotate();
    }

    void FixedUpdate()
    {
        UpdatePhysicsChecks();
        HandleJumping();
        HandleMovement();
        HandleCrouching();
    }

    public void MoveCallback(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }
    public void JumpCallback(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            isHoldJumpButton = true;


            if (CanWallJump())
            {
                isWallJumping = true;
            }
            else if (CanGroundJump())
            {
                GroundJump();
            }
            else if (CanAirJump())
            {
                AirJump();
            }
            else
            {
                lastJumpPressedTime = jumpBufferTime;
            }
        }
        else if (context.canceled)
        {
            isHoldJumpButton = false;
        }

    }

    public void CrouchCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isHoldCrouchButton = true;
        }
        else if (context.canceled)
        {

            isHoldCrouchButton = false;
        }
    }


    void HandleJumping()
    {
        //jump buffer
        if (lastJumpPressedTime > 0)
        {
            if (CanGroundJump())
            {
                GroundJump();
                lastJumpPressedTime = 0;
            }
            else if (CanWallJump())
            {
                isWallJumping = true;
                lastJumpPressedTime = 0;
            }
        }

        yVelocity = isJumping ? jumpForce : rb.linearVelocity.y;

        if (CanJumpCut())
        {
            yVelocity /= 2;
        }

        //wall jump
        if (isWallJumping)
        {
            WallJump();
        }
        isJumping = false;
    }

    void HandleMovement()
    {
        //slide
        if (CanWallSlide())
        {
            yVelocity = Mathf.Max(yVelocity, -wallSlideSpeed);
        }

        float targetXVelocity = moveInput * maxSpeed;


        xVelocity = InterpolateVelocity(targetXVelocity, xVelocity);

        if (!isGrounded)
        {
            xVelocity *= airControlFactor;
        }
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        xVelocity = rb.linearVelocity.x;
    }

    void HandleCrouching()
    {
        if (isHoldCrouchButton)
        {
            standBoxCollider.enabled = false;
            crouchBoxCollider.enabled = true;
            isCrouching = true;
        }
        else
        {
            if (!Physics2D.OverlapBox(crouchCheckPos.position, crouchCheckSize, 0f, crouchMask))
            {
                standBoxCollider.enabled = true;
                crouchBoxCollider.enabled = false;
                isCrouching = false;
            }
        }
    }

    float InterpolateVelocity(float targetVelocity, float currentVelocity)
    {
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        return Mathf.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    void UpdatePhysicsChecks()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask);

        if (isGrounded)
        {
            extraJumpCountLeft = extraJumpCount;
            lastGroundedTime = coyoteTime;
            isInJumpState = false;
        }

        isTouchWall = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallMask);
        if (isTouchWall && yVelocity < 0f)
        {
            isInJumpState = false;
        }
    }

    void AirJump()
    {
        isJumping = true;
        isInJumpState = true;
        jumpTime = 0f;
        extraJumpCountLeft--;
    }
    void GroundJump()
    {
        isJumping = true;
        isInJumpState = true;
        jumpTime = 0f;
        lastGroundedTime = 0f;
    }
    void WallJump()
    {
        yVelocity = wallJumpYForce;
        xVelocity = xVelocity < 0f ? wallJumpXForce : -wallJumpXForce;
        isWallJumping = false;
    }
    private bool CanGroundJump()
    {
        return lastGroundedTime > 0 && !isJumping && isHoldJumpButton;
    }
    private bool CanAirJump()
    {
        return extraJumpCountLeft > 0 && !isJumping;
    }
    private bool CanWallJump()
    {
        return isTouchWall && hasWallJump && !isJumping && !isGrounded && isHoldJumpButton;
    }
    private bool CanWallSlide()
    {
        return isTouchWall && !isJumping && yVelocity < 0f && hasWallSlide && !isGrounded;
    }
    private bool CanJumpCut()
    {
        return !isHoldJumpButton && yVelocity > 0f && isInJumpState && jumpTime > minimalJumpTime && jumpTime < maxJumpTime && !isGrounded;
    }
    private void HandleTimers()
    {
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;
        jumpTime += Time.deltaTime;
    }
    void UpdateAnimations()
    {
        animator.SetFloat(MagnitudeHash, rb.linearVelocity.sqrMagnitude);
        animator.SetFloat(YVelocityHash, rb.linearVelocity.y);
        animator.SetBool(GroundedHash, isGrounded);
        animator.SetBool(WallHash, isTouchWall);
        animator.SetBool(CrouchHash, isCrouching);
    }
    void Rotate()
    {
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            bool shouldFaceRight = moveInput > 0;
            if (shouldFaceRight != isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                isFacingRight = shouldFaceRight;
            }
        }
    }
    public void OnDrawGizmos()
    {
        if (isDebug)
        {
            OnDrawGizmosSelected();
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(crouchCheckPos.position, crouchCheckSize);
        // Gizmos.DrawWireCube(crouchBoxCollider.bounds.center, crouchBoxCollider.bounds.size);
    }
}
