using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Animator))]
public class PlayerSFM : MonoBehaviour
{

    #region Unity Variables
    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private BoxCollider2D standBoxCollider;
    [SerializeField] private BoxCollider2D crouchBoxCollider;
    [SerializeField] private BoxCollider2D wallBoxCollider;


    [Header("Timers")]
    [field: SerializeField] public float AttackTime { get; private set; } = 0.7f;
    [field: SerializeField] public float AttackCooldown { get; private set; } = 0.5f;
    [field: SerializeField] public float InvincibilityAfterHit { get; private set; } = 0.5f;



    [Header("Movement Settings")]
    [field: SerializeField] public float MaxSpeed { get; private set; } = 4f;
    [field: SerializeField] public float AirControlFactor { get; private set; } = 0.98f;
    [field: SerializeField] public float WallSlideSpeed { get; private set; } = 0.5f;
    [field: SerializeField] public float MaxFallSpeed { get; private set; } = 12f;
    [field: SerializeField] public bool HasDash { get; private set; } = true;
    [field: SerializeField] public float DashTime { get; private set; } = 1f;
    [field: SerializeField] public float DashForce { get; private set; } = 1f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float airJumpForce = 10f;

    [SerializeField] private float wallJumpYForce = 8f;
    [SerializeField] private float wallJumpXForce = 5f;
    [field: SerializeField] public float coyoteTime { get; private set; } = 0.1f;
    [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.1f;
    [field: SerializeField] public float MinimalJumpTime { get; private set; } = 0.1f;

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
    private bool isWallInFront = false;
    private bool isTouchWallAnimation = false;
    #endregion

    #region Animations Properties
    public bool AnimationDash = false;
    public bool AnimationCrouch = false;
    #endregion

    #region Properties
    public bool isFacingRight
    { get; private set; } = true;
    public float ButtonMoveInput { get; private set; }
    public bool ButtonJump { get; private set; }
    public bool ButtonCrouch { get; private set; }
    public bool ButtonDash { get; private set; }
    public bool ButtonAttack { get; private set; }

    public bool IsGrounded { get; private set; }
    public bool IsTouchWall { get; private set; }
    public bool IsTouchBackWall { get; private set; }
    public int ExtraJumpCountLeft { get; private set; }
    public float YVelocity { get; private set; }
    public float XVelocity { get; private set; }

    public bool CanDash => timeDashCooldown <= 0f;
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
    public DashState dashState;
    public AttackState attackState;
    public HurtState hurtState;
    public DeathState deathState;
    #endregion
    public const float maxJumpTime = 0.4f;

    #region Timers
    public float TimeLastGrounded { get; private set; }
    public float TimeLastWallTouch { get; private set; }
    public float TimeLastJumpPressed { get; private set; }
    public float TimeJump { get; private set; }
    public float timeDashCooldown = 0f;
    #endregion


    void Start()
    {
        ExtraJumpCountLeft = ExtraJumpCount;
    }
    void OnEnable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDamage += HurtCallback;
            healthComponent.OnDeath += DeathCallback;
        }
    }
    void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDamage -= HurtCallback;
            healthComponent.OnDeath -= DeathCallback;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        StateMachine = new PlayerStateMachine(this);
        idleState = new IdleState(this, StateMachine);
        moveState = new MoveState(this, StateMachine);
        jumpState = new JumpState(this, StateMachine);
        jumpCutState = new JumpCutState(this, StateMachine);
        fallState = new FallState(this, StateMachine);
        wallSlideState = new WallSlideState(this, StateMachine);
        crouchState = new CrouchState(this, StateMachine);
        dashState = new DashState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);
        hurtState = new HurtState(this, StateMachine);
        deathState = new DeathState(this, StateMachine);

        StateMachine.InitializeState(idleState);
    }

    void Update()
    {
        HandleTimers();
        StateMachine.CurrentState.LogicUpdate();
        if (isDebug)
            Debug.Log(StateMachine.CurrentState);

        UpdateAnimations();
        if (StateMachine.CurrentState.CanRotate)
            Rotate();
    }

    void FixedUpdate()
    {
        UpdatePhysicsChecks();
        StateMachine.CurrentState.PhysicsUpdate();
    }

    void UpdatePhysicsChecks()
    {
        IsGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask) && YVelocity > -0.1f;

        if (IsGrounded)
        {
            ExtraJumpCountLeft = ExtraJumpCount;
            TimeLastGrounded = coyoteTime;
        }


        IsTouchBackWall = Physics2D.OverlapBox(wallCheckPosBack.position, wallCheckSizeBack, 0f, wallMask);
        isTouchWallAnimation = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallMask);
        IsTouchWall = IsTouchBackWall || isTouchWallAnimation;
        if (IsTouchWall)
        {
            isWallInFront = IsTouchBackWall ? !isFacingRight : isFacingRight;
            TimeLastWallTouch = coyoteTime;
        }
    }

    public void AirJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = 0f;
        ExtraJumpCountLeft--;
        rb.linearVelocityY = airJumpForce;
    }
    public void GroundJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = 0f;
        TimeLastGrounded = 0f;
        rb.linearVelocityY = jumpForce;

    }
    public void WallJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = -1f;
        XVelocity = isWallInFront ? -wallJumpXForce : wallJumpXForce;

        rb.linearVelocity = new Vector2(XVelocity, wallJumpYForce);
    }

    public bool CanStandUp() =>
        !Physics2D.OverlapBox(crouchCheckPos.position, crouchCheckSize, 0f, crouchMask);

    public void ToggleCrouch(bool isCrouching)
    {
        standBoxCollider.enabled = !isCrouching;
        crouchBoxCollider.enabled = isCrouching;
    }
    public void ReleaseJumpButton() =>
        ButtonJump = false;

    private void HandleTimers()
    {
        TimeLastGrounded -= Time.deltaTime;
        TimeLastWallTouch -= Time.deltaTime;
        TimeLastJumpPressed -= Time.deltaTime;
        TimeJump += Time.deltaTime;
        timeDashCooldown -= Time.deltaTime;
    }
    void Rotate()
    {
        if (Mathf.Abs(ButtonMoveInput) > 0.1f)
        {
            bool shouldFaceRight = ButtonMoveInput > 0;
            if (shouldFaceRight != isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                isFacingRight = shouldFaceRight;
            }
        }
    }

    #region Input
    public void MoveCallback(InputAction.CallbackContext context)
    {
        ButtonMoveInput = context.ReadValue<Vector2>().x;
    }
    public void JumpCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ButtonJump = true;
            TimeLastJumpPressed = JumpBufferTime;
        }
        else if (context.canceled)
            ButtonJump = false;
    }

    public void CrouchCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
            ButtonCrouch = true;
        else if (context.canceled)
            ButtonCrouch = false;
    }

    public void DashCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
            ButtonDash = true;
        else if (context.canceled)
            ButtonDash = false;
    }

    public void AttackCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
            ButtonAttack = true;
        else if (context.canceled)
            ButtonAttack = false;
    }
    #endregion

    #region CallBacks
    private void HurtCallback(int amount)
    {
        StateMachine.ChangeState(hurtState);
    }

    private void DeathCallback()
    {
        StateMachine.ChangeState(deathState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            healthComponent.TakeDamage(damageDealer.damageAmount);
        }
    }
    #endregion

    #region Animation
    private static readonly int MagnitudeHash = Animator.StringToHash("magnitude");
    private static readonly int YVelocityHash = Animator.StringToHash("yVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("onGround");
    private static readonly int WallHash = Animator.StringToHash("onWall");
    private static readonly int CrouchHash = Animator.StringToHash("isCrouching");
    private static readonly int DashHash = Animator.StringToHash("isDashing");

    void UpdateAnimations()
    {
        animator.SetFloat(MagnitudeHash, rb.linearVelocity.sqrMagnitude);
        animator.SetFloat(YVelocityHash, rb.linearVelocity.y);
        animator.SetBool(GroundedHash, IsGrounded);
        animator.SetBool(WallHash, isTouchWallAnimation);
        animator.SetBool(CrouchHash, AnimationCrouch);
        animator.SetBool(DashHash, AnimationDash);
    }
    #endregion
    public void OnDrawGizmos()
    {
        if (isDebug)
            OnDrawGizmosSelected();
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
