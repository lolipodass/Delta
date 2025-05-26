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
    public PlayerStats PlayerStats { get; private set; }
    public PlayerInput Input { get; private set; }
    [SerializeField] private BoxCollider2D standBoxCollider;
    [SerializeField] private BoxCollider2D crouchBoxCollider;
    [field: SerializeField] public PlayerAttackConfig StandAttackConfig { get; private set; }
    [field: SerializeField] public PlayerAttackConfig DashAttackConfig { get; private set; }
    [field: SerializeField] public EffectData HurtEffect { get; private set; }

    #region Masks

    [Foldout("Ground check")][SerializeField] private Transform groundCheckPos;
    [Foldout("Ground check")][SerializeField] private Vector2 groundCheckSize = new(0.5f, 0.5f);
    [Foldout("Ground check")][SerializeField] private LayerMask groundMask;

    [Foldout("Wall check")][SerializeField] private Transform wallCheckPos;
    [Foldout("Wall check")][SerializeField] private Vector2 wallCheckSize = new(0.5f, 0.5f);
    [Foldout("Wall check")][SerializeField] private Transform wallCheckPosBack;

    [Foldout("Wall check")][SerializeField] private Vector2 wallCheckSizeBack = new(0.5f, 0.5f);
    [Foldout("Wall check")][SerializeField] private LayerMask wallMask;

    [Foldout("Crouch check")][SerializeField] private Transform crouchCheckPos;
    [Foldout("Crouch check")][SerializeField] private Vector2 crouchCheckSize = new(0.5f, 0.5f);
    [Foldout("Crouch check")][SerializeField] private LayerMask crouchMask;

    [Header("Attack Settings")]
    [field: SerializeField] public Transform AttackCheckPos { get; private set; }
    [field: SerializeField] public Transform DashAttackCheckPos { get; private set; }


    #endregion

    #endregion

    #region Private Variables
    private bool isWallInFront = false;
    private bool isTouchWallAnimation = false;
    #endregion

    #region Animations Properties
    [HideInInspector] public bool AnimationDash = false;
    [HideInInspector] public bool AnimationCrouch = false;
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
    public bool IsTouchFrontWall { get; private set; }
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
    public UpToFallState upToFallState;
    public JumpCutState jumpCutState;
    public FallState fallState;
    public WallSlideState wallSlideState;
    public CrouchState crouchState;
    public DashState dashState;
    public AttackState attackState;
    public HurtState hurtState;
    public DeathState deathState;
    public DashAttackState dashAttackState;
    public SaveState saveState;
    #endregion
    public const float maxJumpTime = 0.4f;

    #region Timers
    public float TimeLastGrounded { get; private set; }
    public float TimeLastWallTouch { get; private set; }
    public float TimeLastJumpPressed { get; private set; }
    public float TimeJump { get; private set; }
    [HideInInspector] public float timeDashCooldown = 0f;
    #endregion

    [Header("Debug")]
    public bool isDebug = false;
    public bool ShowAttackCheck = false;


    void Start()
    {
        ExtraJumpCountLeft = PlayerStats.Stats.ExtraJumpCount;
    }
    void OnEnable()
    {
        if (PlayerStats.Health != null)
        {
            PlayerStats.Health.OnDamage += OnHurt;
            PlayerStats.Health.OnDeath += OnDeath;
        }
    }
    void OnDisable()
    {
        if (PlayerStats.Health != null)
        {
            PlayerStats.Health.OnDamage -= OnHurt;
            PlayerStats.Health.OnDeath -= OnDeath;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!StateMachine.CurrentState.CanHurt) return;

        if (collision.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            PlayerStats.Health.TakeDamage(damageDealer.damageAmount);
        }
    }

    void Awake()
    {
        GameManager.Instance.SetPlayer(gameObject);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        PlayerStats = GetComponent<PlayerStats>();
        Input = GetComponent<PlayerInput>();
        SubscribeInput();
        StateMachine = new PlayerStateMachine(this);
        idleState = new IdleState(this, StateMachine);
        moveState = new MoveState(this, StateMachine);
        jumpState = new JumpState(this, StateMachine);
        upToFallState = new UpToFallState(this, StateMachine);
        jumpCutState = new JumpCutState(this, StateMachine);
        fallState = new FallState(this, StateMachine);
        wallSlideState = new WallSlideState(this, StateMachine);
        crouchState = new CrouchState(this, StateMachine);
        dashState = new DashState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);
        hurtState = new HurtState(this, StateMachine);
        deathState = new DeathState(this, StateMachine);
        dashAttackState = new DashAttackState(this, StateMachine);
        saveState = new SaveState(this, StateMachine);
        StateMachine.InitializeState(idleState);
    }

    void OnDestroy()
    {
        UnsubscribeInput();
    }
    void UnsubscribeInput()
    {
        var actions = Input.actions;
        actions.FindAction("Move").performed -= MoveCallback;
        actions.FindAction("Move").canceled -= MoveCallback;
        actions.FindAction("Jump").performed -= JumpCallback;
        actions.FindAction("Jump").canceled -= JumpCallback;
        actions.FindAction("Crouch").performed -= CrouchCallback;
        actions.FindAction("Crouch").canceled -= CrouchCallback;
        actions.FindAction("Dash").performed -= DashCallback;
        actions.FindAction("Dash").canceled -= DashCallback;
        actions.FindAction("Attack").performed -= AttackCallback;
        actions.FindAction("Attack").canceled -= AttackCallback;
    }

    void SubscribeInput()
    {
        var actions = Input.actions;
        actions.FindAction("Move").performed += MoveCallback;
        actions.FindAction("Move").canceled += MoveCallback;
        actions.FindAction("Jump").performed += JumpCallback;
        actions.FindAction("Jump").canceled += JumpCallback;
        actions.FindAction("Crouch").performed += CrouchCallback;
        actions.FindAction("Crouch").canceled += CrouchCallback;
        actions.FindAction("Dash").performed += DashCallback;
        actions.FindAction("Dash").canceled += DashCallback;
        actions.FindAction("Attack").performed += AttackCallback;
        actions.FindAction("Attack").canceled += AttackCallback;
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
        YVelocity = rb.linearVelocity.y;
        XVelocity = rb.linearVelocity.x;
    }
    void UpdatePhysicsChecks()
    {
        IsGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundMask) && YVelocity > -0.1f;

        if (IsGrounded)
        {
            ExtraJumpCountLeft = PlayerStats.Stats.ExtraJumpCount;
            TimeLastGrounded = PlayerStats.Stats.CoyoteTime;
        }


        //possible move this into state machine
        IsTouchBackWall = Physics2D.OverlapBox(wallCheckPosBack.position, wallCheckSizeBack, 0f, wallMask);
        isTouchWallAnimation = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallMask);
        IsTouchFrontWall = IsTouchBackWall || isTouchWallAnimation;
        if (IsTouchFrontWall)
        {
            isWallInFront = IsTouchBackWall ? !isFacingRight : isFacingRight;
            TimeLastWallTouch = PlayerStats.Stats.CoyoteTime;
        }
    }

    public void AirJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = 0f;
        ExtraJumpCountLeft--;
        rb.linearVelocityY = PlayerStats.Stats.AirJumpForce;
    }
    public void GroundJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = 0f;
        TimeLastGrounded = 0f;
        rb.linearVelocityY = PlayerStats.Stats.JumpForce;

    }
    public void WallJump()
    {
        TimeLastJumpPressed = 0f;
        TimeJump = -1f;
        XVelocity = isWallInFront ? -PlayerStats.Stats.WallJumpXForce : PlayerStats.Stats.WallJumpXForce;

        rb.linearVelocity = new Vector2(XVelocity, PlayerStats.Stats.WallJumpYForce);
    }

    public void Restart()
    {
        StateMachine.ChangeState(idleState);
        PlayerStats.Restart();
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
            TimeLastJumpPressed = PlayerStats.Stats.JumpBufferTime;
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
    private void OnHurt(int amount)
    {
        Debug.Log("onHurt");
        StateMachine.ChangeState(hurtState);
        if (HurtEffect != null)
            EffectManager.Instance.PlayEffect(HurtEffect);
    }

    private void OnDeath()
    {
        Debug.Log("OnDeath");
        StateMachine.ChangeState(deathState);
    }
    public void AnimationEvent_Attack()
    {
        if (StateMachine.CurrentState is IAttackHandler attackHandler)
        {
            attackHandler.Attack();
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
        if (ShowAttackCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(AttackCheckPos.position, StandAttackConfig.Size);
            Gizmos.DrawWireCube(DashAttackCheckPos.position, DashAttackConfig.Size);
        }
    }
}
