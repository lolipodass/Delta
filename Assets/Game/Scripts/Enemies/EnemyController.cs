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

    [Header("Enemy Settings")] public EnemyState currentState;
    public Animator animator;
    public Seeker seeker;
    public Rigidbody2D rb;
    public HealthComponent healthComponent;


    [Header("Patrol Settings")]
    public float patrolSpeed = 1f;
    public Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    public float patrolPointReachedThreshold = 0.2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 2f;
    public float detectionRange = 5f;
    public float losePlayerRange = 7f;
    [Header("Movement & Physics")]
    public float pathUpdateInterval = 0.5f;
    public float nextWaypointDistance = 0.5f;
    public float JumpForce = 4f;
    public float jumpCooldown = 0.5f;
    public float groundRaycastSize = 0.8f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackTimeBeforeStart = 0.3f;
    public float attackAnimBeforeHit = 0.3f;
    public float attackHitboxActiveDuration = 0.2f;
    public float attackAnimTimeBeforeEnd = 0.4f;

    [Header("Stun Settings")]
    public float stunDuration = 0.5f;
    private float stunTimer;
    private bool isStunned = false;
    private bool isPlayerDetected = false;
    private float moveDirection;
    private float jumpCooldownTimer;
    public Collider2D attackHitbox;
    private Path currentPath;
    private int currentWaypoint = 0;
    public Transform playerTransform;
    private float currentAttackCooldownTimer;
    private float distanceToPlayer;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        if (seeker == null)
        {
            Debug.LogError("Seeker component not found on enemy! Please add it.");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is not found on the child object");
            enabled = false;
            return;
        }

        if (seeker == null)
        {
            Debug.LogError("Seeker component not found on enemy! Please add it.");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found on enemy! Please add it.");
            enabled = false;
            return;
        }

        if (healthComponent == null)
        {
            Debug.LogError("Health component not found on enemy! Please add it.");
            enabled = false;
            return;
        }

        healthComponent.OnDamage += OnHealthDamage;
        healthComponent.OnDeath += OnHealthDeath;
    }

    void Start()
    {
        currentState = EnemyState.Patrol;
        if (playerTransform == null)
            playerTransform = GameManager.Instance.Player.transform;


        if (patrolPoints.Length > 0)
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        else
            Debug.LogWarning("No patrol points assigned for enemy: " + gameObject.name +
                             ". Enemy will stand still in Patrol state.");

        if (attackHitbox == null)

            Debug.LogError("Attack Hitbox (Collider2D) not set, enemy will not attack!" + gameObject.name);
        else
            attackHitbox.enabled = false;

        StartCoroutine(UpdatePathCoroutine());
    }

    void FixedUpdate()
    {
        if (currentState != EnemyState.Stunned && currentState != EnemyState.Dead)
        {
            Move();
        }

        UpdateSpriteDirection();
    }

    void Update()
    {
        distanceToPlayer = Mathf.Infinity;
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerDetected = distanceToPlayer < detectionRange;
        }

        UpdateAnimation();

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

    void Move()
    {
        if (currentPath == null)
        {
            moveDirection = 0;
            return;
        }

        Vector2 direction = (Vector2)currentPath.vectorPath[currentWaypoint] - rb.position;
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

        jumpCooldownTimer -= Time.fixedDeltaTime;
        if (direction.y > 1f && IsGrounded() && jumpCooldownTimer <= 0f)
        {
            jumpCooldownTimer = jumpCooldown;
            rb.linearVelocityY = JumpForce;
        }

        float distance = Vector2.Distance(rb.position, currentPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            if (currentPath.vectorPath.Count > currentWaypoint + 1)
                currentWaypoint++;
        }

    }

    private void UpdateAnimation()
    {
        animator.SetFloat("XVelocity", Mathf.Abs(rb.linearVelocityX));
    }

    private IEnumerator UpdatePathCoroutine()
    {
        while (true)
        {
            if (isStunned)
                yield return null;

            if (currentState == EnemyState.Chase)
            {
                RequestPath(playerTransform.position);
                yield return new WaitForSeconds(pathUpdateInterval);
            }
            else if (currentState == EnemyState.Patrol && patrolPoints.Length > 0)
            {
                RequestPath(patrolPoints[currentPatrolPointIndex].position);
                yield return new WaitForSeconds(pathUpdateInterval * 2);
            }
            else
                yield return new WaitForSeconds(pathUpdateInterval / 2);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogError("Pathfinding error: " + p.errorLog);
            currentPath = null;
            return;
        }

        currentPath = p;
        currentWaypoint = 0;
    }

    private void RequestPath(Vector3 targetPosition)
    {
        if (seeker == null) return;
        seeker.StartPath(rb.position, targetPosition, OnPathComplete);
    }

    private void HandlePatrolState()
    {
        if (isPlayerDetected)
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        if (patrolPoints.Length == 0) return;

        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) <
            patrolPointReachedThreshold)
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
        currentAttackCooldownTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange)
        {
            if (currentAttackCooldownTimer <= 0f)
                StartCoroutine(PerformAttackCoroutine());
        }
        else
        {
            if (distanceToPlayer <= losePlayerRange)
                TransitionToState(EnemyState.Chase);
            else
                TransitionToState(EnemyState.Patrol);
        }
    }

    private void HandleStunnedState()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            isStunned = false;
            if (playerTransform != null &&
                Vector2.Distance(transform.position, playerTransform.position) < detectionRange)
            {
                TransitionToState(EnemyState.Chase);
            }
            else
            {
                TransitionToState(EnemyState.Patrol);
            }
        }
    }

    private void TransitionToState(EnemyState newState)
    {
        if (currentState == newState) return;
        if (currentState == EnemyState.Dead) return;

        Debug.Log($"Enemy transitioning from {currentState} to {newState}");
        currentState = newState;


        currentPath = null;
        currentWaypoint = 0;
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
                break;
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, groundRaycastSize).collider != null;
    }

    private void OnHealthDeath()
    {
        TransitionToState(EnemyState.Dead);

        animator.SetTrigger("death");
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator PerformAttackCoroutine()
    {
        currentAttackCooldownTimer = attackCooldown;
        yield return new WaitForSeconds(attackTimeBeforeStart);
        if (distanceToPlayer <= attackRange)
        {
            animator.SetFloat("attack", UnityEngine.Random.Range(0f, 1f));

            yield return new WaitForSeconds(attackAnimBeforeHit);

            attackHitbox.enabled = true;

            yield return new WaitForSeconds(attackHitboxActiveDuration);

            attackHitbox.enabled = false;

            yield return new WaitForSeconds(attackAnimTimeBeforeEnd);
            animator.SetFloat("attack", 0f);
        }
        else TransitionToState(EnemyState.Chase);
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            Tween.Alpha(spriteRenderer, endValue: 0f, duration: 0.5f)
                .OnComplete(target: transform, ui => Destroy(gameObject));
    }

    private void OnHealthDamage(int value)
    {
        Debug.Log("damage");
        TransitionToState(EnemyState.Stunned);

        animator.SetTrigger("hurt");
    }

    private void UpdateSpriteDirection()
    {
        bool isFacingRight = transform.rotation.y > 0;
        if ((moveDirection < -0.1f && isFacingRight) || (moveDirection > 0.1f && !isFacingRight))
        {
            transform.Rotate(0, 180, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
        Gizmos.color = Color.rosyBrown;
        Gizmos.DrawWireSphere(transform.position, attackRange);


        Debug.DrawRay(transform.position, Vector2.down * groundRaycastSize, IsGrounded() ? Color.green : Color.red);

        Gizmos.color = Color.blue;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(patrolPoints[i].position, 0.1f);
            if (i < patrolPoints.Length - 1)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }

        if (patrolPoints.Length > 1)
        {
            Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
        }

        if (currentPath != null)
        {
            Gizmos.color = Color.green;
            for (int i = currentWaypoint; i < currentPath.vectorPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1]);
            }
        }
    }
}