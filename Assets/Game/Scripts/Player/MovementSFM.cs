using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementSFM : MonoBehaviour
{

    #region Unity Variables
    [Header("References")]
    [Label("Rigidbody")][SerializeField] private Rigidbody2D rb;
    [Label("Animator")][SerializeField] private Animator animator;
    [Label("Stand Box collider")][SerializeField] private BoxCollider2D standBoxCollider;
    [Label("Crouch Box collider")][SerializeField] private BoxCollider2D crouchBoxCollider;



    [Header("Movement Settings")]
    [Label("Max speed")][SerializeField] private float maxSpeed = 4f;
    [field: SerializeField] public float AirControlFactor { get; private set; } = 0.98f;
    [field: SerializeField] public float WallSlideSpeed { get; private set; } = 0.5f;
    [SerializeField] private float MaxFallSpeed = 12f;



    [Header("Jump Settings")]
    [Label("Jump Force")][SerializeField] private float jumpForce = 10f;
    [Label("Air Jump Force")][SerializeField] private float airJumpForce = 10f;

    [Label("Wall Jump Y Force")][SerializeField] private float wallJumpYForce = 8f;
    [Label("Wall Jump X Force")][SerializeField] private float wallJumpXForce = 5f;
    [field: SerializeField] public float coyoteTime { get; private set; } = 0.1f;
    [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.1f;
    [field: SerializeField] public float MinimalJumpTime { get; private set; } = 0.1f;
    // [Range(0f, 0.4f)][SerializeField] float minimalJumpTime = 0.1f;

    [Range(0, 10)][SerializeField] private int ExtraJumpCount = 1;
    [field: SerializeField] public bool HasWallJump { get; private set; } = true;
    [field: SerializeField] public bool HasWallSlide { get; private set; } = true;



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
    [SerializeField] private Transform wallCheckPosBack;
    [Foldout("Wall check")]
    [SerializeField] private Vector2 wallCheckSizeBack = new(0.5f, 0.5f);
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

    #endregion

    #region Private Variables
    private bool isFacingRight = true;
    private bool isCrouching = false;
    private bool isWallInFront = false;
    private bool isTouchWallAnimation = false;
    #endregion

    #region Properties
    public bool IsHoldJumpButton { get; private set; }
    public bool IsHoldCrouchButton { get; private set; }
    public float MoveInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsTouchWall { get; private set; }
    public int ExtraJumpCountLeft { get; private set; }
    public float YVelocity { get; private set; }
    public float XVelocity { get; private set; }
    public bool IsTouchBackWall { get; private set; }
    #endregion

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public JumpCutState jumpCutState;
    public FallState fallState;
    public WallSlideState wallSlideState;
    public CrouchState crouchState;
    #endregion
    public const float maxJumpTime = 0.4f;

    #region Timers
    public float LastGroundedTime { get; private set; }
    public float LastWallTouchTime { get; private set; }
    public float LastJumpPressedTime { get; private set; }
    public float JumpTime { get; private set; }
    #endregion


    void Start()
    {
        ExtraJumpCountLeft = ExtraJumpCount;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StateMachine = new PlayerStateMachine(this);
        idleState = new IdleState(this, StateMachine);
        moveState = new MoveState(this, StateMachine);
        jumpState = new JumpState(this, StateMachine);
        jumpCutState = new JumpCutState(this, StateMachine);
        fallState = new FallState(this, StateMachine);
        wallSlideState = new WallSlideState(this, StateMachine);
        crouchState = new CrouchState(this, StateMachine);
        StateMachine.InitializeState(idleState);
    }

    void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        if (isDebug)
        {
            Debug.Log(StateMachine.CurrentState);
        }
        HandleTimers();
        UpdateAnimations();
        Rotate();
    }

    void FixedUpdate()
    {
        UpdatePhysicsChecks();
        StateMachine.CurrentState.PhysicsUpdate();
        HandleMovement();
    }

    public void MoveCallback(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>().x;
    }
    public void JumpCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsHoldJumpButton = true;
            LastJumpPressedTime = JumpBufferTime;
        }
        else if (context.canceled)
        {
            IsHoldJumpButton = false;
        }
    }

    public void CrouchCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsHoldCrouchButton = true;
        }
        else if (context.canceled)
        {
            IsHoldCrouchButton = false;
        }
    }

    void HandleMovement()
    {
        YVelocity = rb.linearVelocity.y;

        if (!StateMachine.CurrentState.CanMoveHorizontal)
        {
            return;//?? if only vertical movement
        }

        float targetXVelocity = MoveInput * maxSpeed * StateMachine.CurrentState.HorizontalSpeedMultiplayer;
        XVelocity = InterpolateVelocity(targetXVelocity, XVelocity);
        YVelocity = Mathf.Max(YVelocity, -MaxFallSpeed * StateMachine.CurrentState.VerticalSpeedMultiplayer);
        rb.linearVelocity = new Vector2(XVelocity, YVelocity);
    }

    public void SetXVelocity(float value) => XVelocity = value;
    public void SetYVelocity(float value) => YVelocity = value;

    float InterpolateVelocity(float targetVelocity, float currentVelocity)
    {
        return Mathf.Lerp(currentVelocity, targetVelocity, 10f * Time.fixedDeltaTime);
    }

    void UpdatePhysicsChecks()
    {
        IsGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask) && YVelocity > -0.1f;

        if (IsGrounded)
        {
            ExtraJumpCountLeft = ExtraJumpCount;
            LastGroundedTime = coyoteTime;
        }


        IsTouchBackWall = Physics2D.OverlapBox(wallCheckPosBack.position, wallCheckSizeBack, 0f, wallMask);
        isTouchWallAnimation = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallMask);
        IsTouchWall = IsTouchBackWall || isTouchWallAnimation;
        if (IsTouchWall)
        {
            isWallInFront = IsTouchBackWall ? !isFacingRight : isFacingRight;
            LastWallTouchTime = coyoteTime;
        }
    }

    public void AirJump()
    {
        LastJumpPressedTime = 0f;
        JumpTime = 0f;
        ExtraJumpCountLeft--;
        rb.linearVelocityY = airJumpForce;
    }
    public void GroundJump()
    {
        LastJumpPressedTime = 0f;
        JumpTime = 0f;
        LastGroundedTime = 0f;
        rb.linearVelocityY = jumpForce;

    }
    public void WallJump()
    {
        LastJumpPressedTime = 0f;
        JumpTime = -1f;
        XVelocity = isWallInFront ? -wallJumpXForce : wallJumpXForce;

        rb.linearVelocityY = wallJumpYForce;
    }
    public void JumpCut()
    {
        rb.linearVelocityY /= 2f;
    }
    public bool CanStandUp() => !Physics2D.OverlapBox(crouchCheckPos.position, crouchCheckSize, 0f, crouchMask);
    public void ToggleCrouch(bool isCrouching)
    {
        standBoxCollider.enabled = !isCrouching;
        crouchBoxCollider.enabled = isCrouching;
        this.isCrouching = isCrouching;
    }
    public void ReleaseJumpButton()
    {
        IsHoldJumpButton = false;
    }
    private void HandleTimers()
    {
        LastGroundedTime -= Time.deltaTime;
        LastWallTouchTime -= Time.deltaTime;
        LastJumpPressedTime -= Time.deltaTime;
        JumpTime += Time.deltaTime;
    }
    void Rotate()
    {
        if (Mathf.Abs(MoveInput) > 0.1f)
        {
            bool shouldFaceRight = MoveInput > 0;
            if (shouldFaceRight != isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                isFacingRight = shouldFaceRight;
            }
        }
    }

    private static readonly int MagnitudeHash = Animator.StringToHash("magnitude");
    private static readonly int YVelocityHash = Animator.StringToHash("yVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("onGround");
    private static readonly int WallHash = Animator.StringToHash("onWall");
    private static readonly int CrouchHash = Animator.StringToHash("isCrouching");

    void UpdateAnimations()
    {
        animator.SetFloat(MagnitudeHash, rb.linearVelocity.sqrMagnitude);
        animator.SetFloat(YVelocityHash, rb.linearVelocity.y);
        animator.SetBool(GroundedHash, IsGrounded);
        animator.SetBool(WallHash, isTouchWallAnimation);
        animator.SetBool(CrouchHash, isCrouching);
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
        Gizmos.DrawWireCube(wallCheckPosBack.position, wallCheckSizeBack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(crouchCheckPos.position, crouchCheckSize);
        // Gizmos.DrawWireCube(crouchBoxCollider.bounds.center, crouchBoxCollider.bounds.size);
    }
}
