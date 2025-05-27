using System.Collections;
using Pathfinding;
using PrimeTween;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HealthComponent))]
public abstract class BaseEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Stunned,
        Dead
    }

    [Header("Enemy Settings")]
    [SerializeField] protected EnemyState currentState = EnemyState.Patrol;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Seeker seeker;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected HealthComponent healthComponent;
    [SerializeField] protected float RestartWait = 25f;

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolSpeed = 1f;
    [SerializeField] protected PatrolGroup patrolGroup;
    protected Transform[] patrolPoints;
    [SerializeField] protected float patrolPointReachedThreshold = 0.2f;
    protected int currentPatrolPointIndex = 0;

    [Header("Chase Settings")]
    [SerializeField] protected float chaseSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float losePlayerRange = 7f;

    [Header("Movement & Physics")]
    [SerializeField] protected float pathUpdateInterval = 0.5f;
    [SerializeField] protected float nextWaypointDistance = 0.5f;

    [Header("Stun Settings")]
    [SerializeField] protected float stunDuration = 0.5f;

    // Protected fields for inheritance
    protected float stunTimer;
    protected bool isPlayerDetected = false;
    protected float moveDirection;
    protected Vector2 direction;
    protected Path currentPath;
    protected int currentWaypoint = 0;
    protected Transform playerTransform;
    protected float distanceToPlayer = Mathf.Infinity;
    protected Coroutine pathUpdateCoroutine;
    protected SpriteRenderer spriteRenderer;


    // Cached waits
    protected readonly WaitForSeconds pathUpdateWait = new(0.5f);
    protected readonly WaitForSeconds patrolUpdateWait = new(1f);
    protected readonly WaitForSeconds shortWait = new(0.25f);

    // Public properties
    public EnemyState CurrentState => currentState;
    public bool IsPlayerDetected => isPlayerDetected;

    private Vector3 initialPosition;

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        InitializeComponents();
        ValidateComponents();
        SetupHealthEvents();
        InitializeSpecific();
    }

    protected virtual void Start()
    {
        InitializeEnemy();
    }

    protected virtual void FixedUpdate()
    {
        if (CanMove())
        {
            Move();
            HandleMovementSpecific();
        }
        UpdateSpriteDirection();
    }

    protected virtual void Update()
    {
        UpdatePlayerDetection();
        UpdateTimers();
        UpdateAnimation();
        HandleCurrentState();
    }

    protected virtual void OnDestroy()
    {
        CleanupCoroutines();
        UnsubscribeHealthEvents();
        OnDestroySpecific();
    }
    #endregion

    #region Abstract/Virtual Methods for Inheritance
    protected abstract void InitializeSpecific();
    protected abstract void HandleMovementSpecific();
    protected abstract void OnDestroySpecific();
    protected abstract bool CanAttack();
    protected abstract bool CanChase();
    protected abstract float GetAttackRange();
    protected abstract void HandleAttackState();
    protected virtual bool IsGrounded() => true;
    #endregion

    #region Core Initialization
    private void InitializeComponents()
    {
        if (seeker == null) seeker = GetComponent<Seeker>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPoints = patrolGroup.Points;
        initialPosition = transform.position;
    }

    protected virtual void ValidateComponents()
    {
        if (seeker == null)
        {
            Debug.LogError($"Seeker component not found on {gameObject.name}!");
            enabled = false;
            return;
        }

        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D not found on {gameObject.name}!");
            enabled = false;
            return;
        }
    }

    private void SetupHealthEvents()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDamage += OnHealthDamage;
            healthComponent.OnDeath += OnHealthDeath;
        }
    }

    private void UnsubscribeHealthEvents()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDamage -= OnHealthDamage;
            healthComponent.OnDeath -= OnHealthDeath;
        }
    }

    private void InitializeEnemy()
    {
        playerTransform = GameManager.Instance.Player.transform;

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning($"No patrol points assigned for enemy: {gameObject.name}");
        }
        else
        {
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        }

        pathUpdateCoroutine = StartCoroutine(UpdatePathCoroutine());
    }
    #endregion

    #region Core Update Methods
    private void UpdatePlayerDetection()
    {
        distanceToPlayer = Mathf.Infinity;
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerDetected = distanceToPlayer < detectionRange;
        }
    }

    protected virtual void UpdateTimers()
    {
        // Base timers - can be extended in derived classes
    }

    protected virtual void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("XVelocity", Mathf.Abs(rb.linearVelocityX));
        }
    }

    protected bool CanMove()
    {
        return currentState != EnemyState.Stunned && currentState != EnemyState.Dead;
    }
    #endregion

    #region Core Movement
    protected virtual void Move()
    {
        if (currentPath == null)
        {
            moveDirection = 0;
            return;
        }

        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            moveDirection = 0;
            return;
        }

        direction = (Vector2)currentPath.vectorPath[currentWaypoint] - rb.position;
        float x = direction.x > 0.05f ? 1 : direction.x < -0.05f ? -1 : 0;

        float currentSpeed = (currentState == EnemyState.Chase) ? chaseSpeed : patrolSpeed;
        moveDirection = x * currentSpeed;
        rb.linearVelocityX = moveDirection;

        UpdatePathPoint();
    }
    #endregion



    #region State Handling
    private void HandleCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
            case EnemyState.Stunned:
                HandleStunnedState();
                break;
            case EnemyState.Dead:
                break;
        }
    }

    private void HandlePatrolState()
    {
        if (CanChase() && isPlayerDetected)
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        if (patrolPoints.Length == 0) return;

        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < patrolPointReachedThreshold)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        }
    }


    private void HandleChaseState()
    {
        if (distanceToPlayer > losePlayerRange)
        {
            TransitionToState(EnemyState.Patrol);
            return;
        }

        if (CanAttack() && distanceToPlayer < GetAttackRange())
        {
            TransitionToState(EnemyState.Attack);
            return;
        }
    }

    private void HandleStunnedState()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            TransitionToState(isPlayerDetected ? EnemyState.Chase : EnemyState.Patrol);
        }
    }
    #endregion

    #region State Transitions
    protected virtual void TransitionToState(EnemyState newState)
    {
        if (currentState == newState || currentState == EnemyState.Dead) return;

        Debug.Log($"Enemy {gameObject.name} transitioning from {currentState} to {newState}");

        ExitCurrentState();
        currentState = newState;
        EnterNewState(newState);
    }

    protected virtual void ExitCurrentState()
    {
        currentPath = null;
        currentWaypoint = 0;
    }

    protected virtual void EnterNewState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Patrol:
                if (patrolPoints.Length > 0)
                    RequestPath(patrolPoints[currentPatrolPointIndex].position);
                break;
            case EnemyState.Chase:
                if (playerTransform != null)
                    RequestPath(playerTransform.position);
                break;
            case EnemyState.Stunned:
                rb.linearVelocityX = 0;
                stunTimer = stunDuration;
                break;
            case EnemyState.Attack:
                rb.linearVelocityX = 0;
                break;
            case EnemyState.Dead:
                rb.linearVelocityX = 0;
                CleanupCoroutines();
                break;
        }
    }
    #endregion

    #region Pathfinding
    private IEnumerator UpdatePathCoroutine()
    {
        while (currentState != EnemyState.Dead)
        {
            if (currentState == EnemyState.Stunned)
            {
                yield return shortWait;
                continue;
            }

            switch (currentState)
            {
                case EnemyState.Chase:
                    if (playerTransform != null)
                        RequestPath(playerTransform.position);
                    yield return pathUpdateWait;
                    break;
                case EnemyState.Patrol:
                    if (patrolPoints.Length > 0)
                        RequestPath(patrolPoints[currentPatrolPointIndex].position);
                    yield return patrolUpdateWait;
                    break;
                default:
                    yield return shortWait;
                    break;
            }
        }
    }

    protected void UpdatePathPoint()
    {
        float distance = Vector2.Distance(rb.position, currentPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance && currentWaypoint < currentPath.vectorPath.Count - 1)
        {
            currentWaypoint++;
        }
    }

    protected void RequestPath(Vector3 targetPosition)
    {
        if (seeker != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetPosition, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogError($"Pathfinding error on {gameObject.name}: {p.errorLog}");
            currentPath = null;
            return;
        }

        currentPath = p;
        currentWaypoint = 0;
    }
    #endregion

    #region Health Events
    private void OnHealthDamage(int damage)
    {
        Debug.Log($"Enemy {gameObject.name} took {damage} damage");
        TransitionToState(EnemyState.Stunned);

        if (animator != null)
        {
            animator.SetTrigger("hurt");
        }
    }

    private async void OnHealthDeath()
    {
        TransitionToState(EnemyState.Dead);

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        await DeathTask();
    }

    private async UniTask DeathTask()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));


        if (spriteRenderer != null)
        {
            await Tween.Alpha(spriteRenderer, endValue: 0f, duration: 0.5f);

            gameObject.SetActive(false);

            await UniTask.Delay(System.TimeSpan.FromSeconds(RestartWait));

            await Restart();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual async Task Restart()
    {
        gameObject.SetActive(true);
        await Tween.Alpha(spriteRenderer, endValue: 1f, duration: 0.5f);
        transform.position = initialPosition;
        EnterNewState(EnemyState.Patrol);
        healthComponent.ResetHealth();
    }
    #endregion

    #region Utility
    protected virtual void UpdateSpriteDirection()
    {
        bool isFacingRight = transform.rotation.y > 0;
        if ((moveDirection < -0.1f && isFacingRight) || (moveDirection > 0.1f && !isFacingRight))
        {
            transform.Rotate(0, 180, 0);
        }
    }

    protected void CleanupCoroutines()
    {
        if (pathUpdateCoroutine != null)
        {
            StopCoroutine(pathUpdateCoroutine);
            pathUpdateCoroutine = null;
        }
    }
    #endregion

    #region Gizmos
    protected virtual void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Lose player range
        Gizmos.color = Color.rosyBrown;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);

        patrolGroup.OnDrawGizmosSelected();

        // Current path
        if (currentPath != null)
        {
            Gizmos.color = Color.green;
            for (int i = currentWaypoint; i < currentPath.vectorPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1]);
            }
        }
    }
    #endregion

}
