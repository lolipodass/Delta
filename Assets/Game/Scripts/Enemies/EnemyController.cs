using UnityEngine;
using Pathfinding;
using System.Collections;
using System;
using PrimeTween;

public class EnemyController : MonoBehaviour
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
    [SerializeField] private EnemyState currentState = EnemyState.Patrol;
    [SerializeField] private Animator animator;
    [SerializeField] private Seeker seeker;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private HealthComponent healthComponent;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private PatrolGroup patrolGroup;
    private Transform[] patrolPoints;
    [SerializeField] private float patrolPointReachedThreshold = 0.2f;
    private int currentPatrolPointIndex = 0;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float losePlayerRange = 7f;

    [Header("Movement & Physics")]
    [SerializeField] private float pathUpdateInterval = 0.5f;
    [SerializeField] private float nextWaypointDistance = 0.5f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float groundRaycastSize = 0.8f;
    [SerializeField] private LayerMask groundLayerMask = -1;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackTimeBeforeStart = 0.3f;
    [SerializeField] private float attackAnimBeforeHit = 0.3f;
    [SerializeField] private float attackHitboxActiveDuration = 0.2f;
    [SerializeField] private float attackAnimTimeBeforeEnd = 0.4f;
    [SerializeField] private Collider2D attackHitbox;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.5f;

    // Private fields
    private float stunTimer;
    private bool isStunned = false;
    private bool isPlayerDetected = false;
    private float moveDirection;
    private Vector2 direction;
    private float jumpCooldownTimer;
    private Path currentPath;
    private int currentWaypoint = 0;
    private Transform playerTransform;
    private float currentAttackCooldownTimer;
    private float distanceToPlayer = Mathf.Infinity;
    private Coroutine pathUpdateCoroutine;
    private Coroutine attackCoroutine;
    private SpriteRenderer spriteRenderer;

    // Cached for performance
    private readonly WaitForSeconds pathUpdateWait = new(0.5f);
    private readonly WaitForSeconds patrolUpdateWait = new(1f);
    private readonly WaitForSeconds shortWait = new(0.25f);

    // Public read-only properties
    public EnemyState CurrentState => currentState;
    public bool IsPlayerDetected => isPlayerDetected;

    void Awake()
    {
        InitializeComponents();
        ValidateComponents();
        SetupHealthEvents();
    }

    void Start()
    {
        InitializeEnemy();
    }

    void FixedUpdate()
    {
        if (CanMove())
        {
            Move();
        }
        UpdateSpriteDirection();
    }

    void Update()
    {
        UpdatePlayerDetection();
        UpdateTimers();
        UpdateAnimation();
        HandleCurrentState();
    }

    void OnDestroy()
    {
        CleanupCoroutines();
        UnsubscribeHealthEvents();
    }

    #region Initialization
    private void InitializeComponents()
    {
        if (seeker == null) seeker = GetComponent<Seeker>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPoints = patrolGroup.Points;
    }

    private void ValidateComponents()
    {
        if (seeker == null)
        {
            Debug.LogError($"Seeker component not found on {gameObject.name}! Please add it.", this);
            enabled = false;
            return;
        }

        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D not found on {gameObject.name}!", this);
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError($"Animator component not found on {gameObject.name}! Please add it.", this);
            enabled = false;
            return;
        }

        if (healthComponent == null)
        {
            Debug.LogError($"Health component not found on {gameObject.name}! Please add it.", this);
            enabled = false;
            return;
        }

        if (attackHitbox == null)
        {
            Debug.LogError($"Attack Hitbox (Collider2D) not set on {gameObject.name}, enemy will not attack!", this);
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


        if (attackHitbox != null)
            attackHitbox.enabled = false;

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning($"No patrol points assigned for enemy: {gameObject.name}. Enemy will stand still in Patrol state.", this);
        }
        else
        {
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        }

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<DamageDealer>(out var dealer))
            {
                dealer.damageAmount = attackDamage;
            }
        }

        pathUpdateCoroutine = StartCoroutine(UpdatePathCoroutine());
    }
    #endregion

    #region Update Methods
    private void UpdatePlayerDetection()
    {
        distanceToPlayer = Mathf.Infinity;
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerDetected = distanceToPlayer < detectionRange;
        }
    }

    private void UpdateTimers()
    {
        jumpCooldownTimer -= Time.fixedDeltaTime;
        currentAttackCooldownTimer -= Time.deltaTime;
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("XVelocity", Mathf.Abs(rb.linearVelocityX));
        }
    }

    private bool CanMove()
    {
        return currentState != EnemyState.Stunned && currentState != EnemyState.Dead && !isStunned;
    }
    #endregion

    #region Movement
    void Move()
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
        float x;
        if (direction.x > 0.1f)
            x = 1;
        else if (direction.x < -0.1f)
            x = -1;
        else
            x = 0;


        float currentSpeed = (currentState == EnemyState.Chase) ? chaseSpeed : patrolSpeed;
        moveDirection = x * currentSpeed;
        rb.linearVelocityX = moveDirection;

        // Handle jumping
        if (direction.y > 0.5f && IsGrounded() && jumpCooldownTimer <= 0f)
        {
            jumpCooldownTimer = jumpCooldown;
            rb.linearVelocityY = jumpForce;
        }

        // Check if reached current waypoint
        float distance = Vector2.Distance(rb.position, currentPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance && currentWaypoint < currentPath.vectorPath.Count - 1)
        {
            currentWaypoint++;
        }
    }

    public bool IsGrounded()
    {
        var groundCheck = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastSize, groundLayerMask);
        return groundCheck.collider != null;
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
        if (isPlayerDetected)
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

        if (distanceToPlayer < attackRange)
        {
            TransitionToState(EnemyState.Attack);
            return;
        }
    }

    private void HandleAttackState()
    {
        if (currentAttackCooldownTimer <= 0f && attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(PerformAttackCoroutine());
        }
    }

    private void HandleStunnedState()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            isStunned = false;

            if (isPlayerDetected)
                TransitionToState(EnemyState.Chase);
            else
                TransitionToState(EnemyState.Patrol);
        }
    }
    #endregion

    #region State Transitions
    private void TransitionToState(EnemyState newState)
    {
        if (currentState == newState) return;
        if (currentState == EnemyState.Dead) return;

        Debug.Log($"Enemy {gameObject.name} transitioning from {currentState} to {newState}");

        ExitCurrentState();
        currentState = newState;
        EnterNewState(newState);
    }

    private void ExitCurrentState()
    {
        // Clean up current state
        currentPath = null;
        currentWaypoint = 0;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            if (animator != null)
                animator.SetFloat("attack", 0f);
        }
    }

    private void EnterNewState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Patrol:
                if (patrolPoints.Length > 0)
                    RequestPath(patrolPoints[currentPatrolPointIndex].position);
                else
                    rb.linearVelocityX = 0;
                break;

            case EnemyState.Chase:
                if (distanceToPlayer > losePlayerRange)
                {
                    TransitionToState(EnemyState.Patrol);
                    return;
                }
                else if (distanceToPlayer <= attackRange)
                {
                    TransitionToState(EnemyState.Attack);
                    return;
                }
                if (playerTransform != null)
                    RequestPath(playerTransform.position);
                break;

            case EnemyState.Attack:
                rb.linearVelocityX = 0;
                break;

            case EnemyState.Stunned:
                rb.linearVelocityX = 0;
                stunTimer = stunDuration;
                isStunned = true;
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
            if (isStunned)
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

    private void RequestPath(Vector3 targetPosition)
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
            Debug.LogError($"Pathfinding error on {gameObject.name}: {p.errorLog}", this);
            currentPath = null;
            return;
        }

        currentPath = p;
        currentWaypoint = 0;
    }
    #endregion

    #region Combat
    private IEnumerator PerformAttackCoroutine()
    {
        currentAttackCooldownTimer = attackCooldown;

        yield return new WaitForSeconds(attackTimeBeforeStart);
        CheckPlayerRange();

        if (CheckPlayerRange()) yield break;

        if (animator != null)
        {
            animator.SetFloat("attack", UnityEngine.Random.Range(0f, 1f));
        }

        yield return new WaitForSeconds(attackAnimBeforeHit);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
            yield return new WaitForSeconds(attackHitboxActiveDuration);
            attackHitbox.enabled = false;
        }

        yield return new WaitForSeconds(attackAnimTimeBeforeEnd);

        if (animator != null)
        {
            animator.SetFloat("attack", 0f);
        }

        if (distanceToPlayer > attackRange)
            TransitionToState(EnemyState.Chase);
        else
            TransitionToState(EnemyState.Patrol);

        attackCoroutine = null;
    }

    private bool CheckPlayerRange()
    {
        if (distanceToPlayer >= attackRange)
        {
            TransitionToState(EnemyState.Chase);
            attackCoroutine = null;
            if (animator != null)
                animator.SetFloat("attack", 0f);
            return true;
        }
        return false;
    }
    #endregion

    #region Health Events
    private void OnHealthDamage(int damage, Vector2 position)
    {
        Debug.Log($"Enemy {gameObject.name} took {damage} damage");
        TransitionToState(EnemyState.Stunned);

        if (animator != null)
        {
            animator.SetTrigger("hurt");
        }
    }

    private void OnHealthDeath(Vector2 position)
    {
        TransitionToState(EnemyState.Dead);

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (spriteRenderer != null)
        {
            Tween.Alpha(spriteRenderer, endValue: 0f, duration: 0.5f)
                .OnComplete(target: transform, _ => Destroy(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Utility
    private void UpdateSpriteDirection()
    {
        bool isFacingRight = transform.rotation.y > 0;
        if ((moveDirection < -0.1f && isFacingRight) || (moveDirection > 0.1f && !isFacingRight))
        {
            transform.Rotate(0, 180, 0);
        }
    }

    private void CleanupCoroutines()
    {
        if (pathUpdateCoroutine != null)
        {
            StopCoroutine(pathUpdateCoroutine);
            pathUpdateCoroutine = null;
        }

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Lose player range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);

        // Attack range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Ground check
        Debug.DrawRay(transform.position, Vector2.down * groundRaycastSize, IsGrounded() ? Color.green : Color.red);

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