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
    [Label("Extra jump count")][SerializeField] private int extraJumpCount = 1;
    [Label("Coyote Time")][Range(0f, 1f)][SerializeField] private float coyoteTime = 0.1f;
    [Label("Jump Buffer Time")][Range(0f, 1f)][SerializeField] private float jumpBufferTime = 0.1f;
    [Label("Has Wall Jump")][SerializeField] private bool hasWallJump = true;

    private int extraJumpCountLeft;


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

    [Header("Debug")]
    public bool isDebug = false;

    //state
    private bool isFacingRight = true;
    private float moveInput;
    private bool isGrounded;
    private bool isTouchWall;
    private bool isJumping = false;
    private bool isHoldingJump = false;
    private bool isWallJumping = false;
    // private bool isWallSliding = false;
    private float xVelocity = 0f;
    private float yVelocity = 0f;

    //timers
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    void Start()
    {
        extraJumpCountLeft = extraJumpCount;
    }

    void Update()
    {
        Rotate();
        HandleTimers();
        Animate();

    }

    void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        ComputeJumping();
        ComputeMovement();
    }

    public void MoveCallback(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }
    public void JumpCallback(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            lastJumpPressedTime = jumpBufferTime;

            if (CanWallJump())
            {
                isWallJumping = true;
            }
            else if (CanJump())
            {
                Jump();
                if (!isGrounded && lastGroundedTime < 0)
                {
                    extraJumpCountLeft--;
                }
            }
        }
        else if (context.canceled)
        {
            isHoldingJump = false;
        }

    }

    void ComputeJumping()
    {
        //jump buffering
        if (lastJumpPressedTime > 0 && CanJump())
        {
            Jump();
            lastJumpPressedTime = 0;
        }

        yVelocity = isJumping ? jumpForce : rb.linearVelocity.y;

        if (!isHoldingJump && yVelocity > 0f)
        {
            yVelocity /= 2;
        }

        //wall jump
        if (isWallJumping)
        {
            Debug.Log("wall jump");
            yVelocity = wallJumpYForce;
            xVelocity = xVelocity < 0f ? wallJumpXForce : -wallJumpXForce;
            isWallJumping = false;
            Debug.Log("x" + xVelocity);
            Debug.Log("y" + yVelocity);
        }
        isJumping = false;
    }

    void ComputeMovement()
    {
        //slide
        if (isTouchWall && yVelocity < 0f && !isGrounded)
        {
            yVelocity = Mathf.Max(yVelocity, -wallSlideSpeed);
        }

        if (isWallJumping)
        {
            Debug.Log("x compute" + xVelocity);
            Debug.Log("y compute" + yVelocity);
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

    float InterpolateVelocity(float targetVelocity, float currentVelocity)
    {
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        return Mathf.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask);

        if (isGrounded)
        {
            extraJumpCountLeft = extraJumpCount;
            lastGroundedTime = coyoteTime;
        }
    }
    void WallCheck()
    {
        isTouchWall = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallMask);
    }

    void Jump()
    {
        isJumping = true;
        isHoldingJump = true;
    }
    private bool CanJump()
    {
        return (lastGroundedTime > 0 || extraJumpCountLeft > 0) && !isJumping;
    }
    private bool CanWallJump()
    {
        return isTouchWall && hasWallJump && !isJumping;
    }
    private void HandleTimers()
    {
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;
    }
    void Animate()
    {
        animator.SetFloat("magnitude", rb.linearVelocity.sqrMagnitude);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("onGround", isGrounded);
        animator.SetBool("onWall", isTouchWall);
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
    }
}
