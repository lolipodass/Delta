using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    [Label("Rigidbody")]
    [SerializeField] private Rigidbody2D rb;

    [Label("Speed")]
    [SerializeField] private float speed = 4f;

    [Label("Jump Force")]
    [SerializeField] private float jumpForce = 10f;

    [Label("Extra jump count")]
    [SerializeField] private int extraJumpCount = 1;
    private int extraJumpCountLeft;


    //ground check
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundMask;

    private bool isJumping = false;
    private bool isHoldingJump = false;
    private bool isGrounded;


    private bool isFacingRight = true;

    private float moveInput;


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

    private bool CanJump()
    {
        return (lastGroundedTime > 0 || extraJumpCountLeft > 0) && !isJumping;
    }
    public void JumpCallback(InputAction.CallbackContext context)
    {

        // Debug.Log(isGrounded);
        if (context.performed)
        {
            if (CanJump())
            {
                isJumping = true;
                isHoldingJump = true;
                if (!isGrounded)
                {
                    extraJumpCountLeft--;
                }
            }
            else
            {
                lastJumpPressedTime = 0.2f;
            }
        }
        else if (context.canceled)
        {
            isHoldingJump = false;
        }

    }

    void Movement()
    {
        if (lastJumpPressedTime > 0 && CanJump())
        {
            isJumping = true;
            isHoldingJump = true;
            extraJumpCountLeft--;
            lastJumpPressedTime = 0;
        }

        float yVelocity = isJumping ? jumpForce : rb.linearVelocity.y;
        isJumping = false;

        if (!isHoldingJump && yVelocity > 0f)
        {
            yVelocity /= 2;
            // yVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, yVelocity);

    }

    void Rotate()
    {
        float xVelocity = rb.linearVelocity.x;
        if ((xVelocity < 0 && isFacingRight) || (xVelocity > 0 && !isFacingRight))
        {
            transform.Rotate(0, 180, 0);
            isFacingRight = !isFacingRight;
        }

    }


    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask);

        if (isGrounded)
        {
            extraJumpCountLeft = extraJumpCount;
            lastGroundedTime = 0.2f;
        }
    }

    private void HandleTimers()
    {
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
