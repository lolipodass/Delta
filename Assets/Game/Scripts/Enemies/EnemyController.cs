using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("Enemy Settings")]
    public EnemyState currentState;
    public Animator animator;
    public Seeker seeker;
    public Rigidbody2D rb;



    [Header("Patrol Settings")]
    public float patrolSpeed = 1f;
    public Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    public float patrolPointReachedThreshold = 0.2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 2f;
    public float detectionRange = 5f;
    public float losePlayerRange = 7f;
    public string playerTag = "Player";

    [Header("Movement & Physics")]

    public float pathUpdateInterval = 0.5f;
    public float nextWaypointDistance = 0.5f;
    public float JumpForce = 4f;
    public float jumpCooldown = 0.5f;
    public float groundRaycastSize = 0.8f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackActiveDuration = 0.2f;
    public float attackAnimHitTime = 0.3f;
    public float attackAnimEndTime = 0.4f;
    private bool isPlayerDetected = false;
    private float moveDirection;
    private float jumpCooldownTimer;
    public Collider2D attackHitbox;
    private Path currentPath;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    public Transform playerTransform;
    private float currentAttackCooldownTimer;


    void Awake()
    {

        seeker = GetComponent<Seeker>();

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

    }
    void Start()
    {
        currentState = EnemyState.Patrol;
        playerTransform = GameManager.Instance.Player.transform;
        if (playerTransform == null)
            Debug.LogWarning($"Player with tag '{playerTag}' not found! Make sure your player has this tag.");

        if (patrolPoints.Length > 0)
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        else
            Debug.LogWarning("No patrol points assigned for enemy: " + gameObject.name + ". Enemy will stand still in Patrol state.");

        if (attackHitbox == null)

            Debug.LogError("Attack Hitbox (Collider2D) not set, enemy will not attack!" + gameObject.name);
        else
            attackHitbox.enabled = false;

        StartCoroutine(UpdatePathRoutine());
    }

    void FixedUpdate()
    {
        Move();
        UpdateSpriteDirection();
    }

    void Update()
    {
        float distanceToPlayer = Mathf.Infinity;
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerDetected = distanceToPlayer < detectionRange;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState(distanceToPlayer);
                break;
            case EnemyState.Attack:
                HandleAttackState(distanceToPlayer);
                break;
        }
    }

    IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (currentState == EnemyState.Chase && playerTransform != null)
            {
                RequestPath(playerTransform.position);
                yield return new WaitForSeconds(pathUpdateInterval);
            }
            else if (currentState == EnemyState.Patrol && patrolPoints.Length > 0 && !reachedEndOfPath)
            {
                RequestPath(patrolPoints[currentPatrolPointIndex].position);
                yield return new WaitForSeconds(pathUpdateInterval * 2);
            }
            else
                yield return new WaitForSeconds(pathUpdateInterval / 2);
        }
    }
    IEnumerator PerformAttackCoroutine()
    {
        currentAttackCooldownTimer = attackCooldown;

        animator.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(attackAnimHitTime);

        attackHitbox.enabled = true;


        yield return new WaitForSeconds(attackActiveDuration);

        attackHitbox.enabled = false;

        yield return new WaitForSeconds(attackAnimEndTime);

    }

    void OnPathComplete(Path p)
    {

        if (!p.error)
        {
            currentPath = p;
            currentWaypoint = 0;
            reachedEndOfPath = false;
        }
        else
        {
            Debug.LogError("Pathfinding error: " + p.errorLog);
            currentPath = null;
        }
    }

    void RequestPath(Vector3 targetPosition)
    {
        if (seeker == null) return;
        seeker.StartPath(rb.position, targetPosition, OnPathComplete);
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, groundRaycastSize).collider != null;

    }
    void Move()
    {
        if (currentPath == null)
        {
            moveDirection = 0;
            rb.linearVelocity = Vector2.zero;
            return;
        }


        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            reachedEndOfPath = true;
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

        if (direction.y > 1f && IsGrounded() && jumpCooldownTimer <= 0f)
        {
            jumpCooldownTimer = jumpCooldown;
            rb.linearVelocityY = JumpForce;
        }

        rb.linearVelocityX = moveDirection;

        float distance = Vector2.Distance(rb.position, currentPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        jumpCooldownTimer -= Time.fixedDeltaTime;
    }


    void HandlePatrolState()
    {
        if (isPlayerDetected)
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        if (patrolPoints.Length == 0) return;

        if (reachedEndOfPath && Vector2.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < patrolPointReachedThreshold)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            RequestPath(patrolPoints[currentPatrolPointIndex].position);
        }

    }

    void HandleChaseState(float distanceToPlayer)
    {
        if (playerTransform == null)
        {
            TransitionToState(EnemyState.Patrol);
            return;
        }

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

    void HandleAttackState(float distanceToPlayer)
    {
        currentAttackCooldownTimer -= Time.deltaTime;
        if (playerTransform == null)
        {
            TransitionToState(EnemyState.Patrol);
            return;
        }

        if (currentAttackCooldownTimer <= 0f)
        {
            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(PerformAttackCoroutine());
            }
            else
            {
                TransitionToState(EnemyState.Chase);
            }
        }
        else
        {
            if (distanceToPlayer > attackRange && distanceToPlayer <= losePlayerRange)
            {
                TransitionToState(EnemyState.Chase);
            }
            else if (distanceToPlayer > losePlayerRange)
            {
                TransitionToState(EnemyState.Patrol);
            }
        }
    }
    void TransitionToState(EnemyState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Enemy transitioning from {currentState} to {newState}");
        currentState = newState;


        currentPath = null;
        currentWaypoint = 0;
        reachedEndOfPath = false;

        switch (newState)
        {
            case EnemyState.Patrol:
                if (patrolPoints.Length > 0)
                {
                    RequestPath(patrolPoints[currentPatrolPointIndex].position);
                }
                else
                {
                    moveDirection = 0;
                    rb.linearVelocity = Vector2.zero;
                }
                break;
            case EnemyState.Chase:
                RequestPath(playerTransform.position);
                break;
            case EnemyState.Attack:

                moveDirection = 0;
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void UpdateSpriteDirection()
    {
        if (transform == null) return;


        if (moveDirection > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirection < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

    }

    void OnDrawGizmosSelected()
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