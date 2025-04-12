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

    [Header("Jump Settings")]
    [Label("Jump Force")][SerializeField] private float jumpForce = 10f;

    [Label("Extra jump count")][SerializeField] private int extraJumpCount = 1;
    [Label("Coyote Time")][Range(0f, 1f)][SerializeField] private float coyoteTime = 0.1f;
    [Label("Jump Buffer Time")][Range(0f, 1f)][SerializeField] private float jumpBufferTime = 0.1f;
    private int extraJumpCountLeft;



    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new(0.5f, 0.5f);
    [SerializeField] private LayerMask groundMask;


    //state
    private bool isFacingRight = true;
    private float moveInput;
    private bool isGrounded;
    private bool isJumping = false;
    private bool isHoldingJump = false;
    private float speed = 0f;


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

        Debug.Log(rb.linearVelocity.sqrMagnitude);
        animator.SetFloat("magnitude", rb.linearVelocity.sqrMagnitude);
    }

    void FixedUpdate()
    {
        GroundCheck();
        Movement();
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
            if (CanJump())
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

    void Movement()
    {
        //jump buffering
        if (lastJumpPressedTime > 0 && CanJump())
        {
            Jump();
            lastJumpPressedTime = 0;
        }

        float yVelocity = isJumping ? jumpForce : rb.linearVelocity.y;
        isJumping = false;

        if (!isHoldingJump && yVelocity > 0f)
        {
            yVelocity /= 2;
            // yVelocity = 0f;
        }

        float targetVelocity = moveInput * maxSpeed;
        // float currentVelocity = rb.linearVelocity.x;
        speed = targetVelocity;

        // float acceleration = isGrounded ? 20f : 10f;
        // speed = Mathf.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        if (!isGrounded)
        {
            speed *= airControlFactor;
        }

        rb.linearVelocity = new Vector2(speed, yVelocity);

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


    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask);

        if (isGrounded)
        {
            extraJumpCountLeft = extraJumpCount;
            lastGroundedTime = coyoteTime;
        }
    }

    private void HandleTimers()
    {
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
