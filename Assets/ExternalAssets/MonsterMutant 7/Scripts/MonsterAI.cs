using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Target & Movement")]
    public Transform playerTarget;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float wanderDelay = 3f;
    public float attackBoundary = 3f;

    private bool isDeath = false;

    public float hp = 15f;

    // Animator parameters
    private readonly int AnimAttack = Animator.StringToHash("Attack");
    private readonly int AnimMoveSpeed = Animator.StringToHash("moveSpeed");

    private bool isAttacking = false;
    private Vector3 initialPosition;
    private Coroutine currentRoutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        agent.stoppingDistance = attackRange;
        animator.applyRootMotion = false;
    }

    void Start()
    {
        currentRoutine = StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (agent == null || animator == null) return;
        if (isDeath) return;
        
        if (hp <= 0 && !isDeath)
        {
            isDeath = true;
            animator.SetTrigger("Death");
            agent.enabled = false;
            return;
        }

        // Use the NavMeshAgent’s current speed (normalized 0–1)
        float speed = agent.velocity.magnitude / agent.speed;

        // Update the animator float
        animator.SetFloat("moveSpeed", speed);

        // Optional: keep facing direction consistent
        if (speed > 0.1f && !isAttacking)
        {
            animator.applyRootMotion = false;
        }
    }




    // WALK ROUTINE

    IEnumerator PatrolRoutine()
    {
        agent.isStopped = false;

        while (true)
        {
            if (isDeath) break;
            if (CheckForPlayer())
            {
                SwitchRoutine(ChaseRoutine());
                yield break;
            }

            // If idle, pick new destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + initialPosition;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
                {
                    yield return new WaitForSeconds(wanderDelay);
                    agent.SetDestination(hit.position);
                }
            }

            yield return null;
        }
    }
    // CHASE ROUTINE

    IEnumerator ChaseRoutine()
    {
        agent.isStopped = false;

        while (true)
        {
            if (isDeath) break;
            if (isAttacking)
            {
                yield return null;
                continue;
            }
            if (playerTarget == null)
            {
                SwitchRoutine(PatrolRoutine());
                yield break;
            }

            float distance = Vector3.Distance(playerTarget.position, transform.position);

            if (distance <= attackRange)
            {
                if (!isAttacking)
                    StartCoroutine(AttackRoutine());
            }
            else if (distance <= detectionRange)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
            }
            else
            {
                // Player out of range → go back to walk
                SwitchRoutine(PatrolRoutine());
                yield break;
            }

            yield return null;
        }
    }

    // ATTACK ROUTINE
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.SetDestination(transform.position);
        

        // Stop moving + face player
        if (playerTarget != null)
        {
            Vector3 lookPos = playerTarget.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }

        animator.SetTrigger(AnimAttack);
        
        yield return new WaitForSeconds(3f);
        
        agent.SetDestination(playerTarget.position);

        isAttacking = false;
        agent.isStopped = false;
    }

    bool CheckForPlayer()
    {
        if (playerTarget == null) return false;
        return Vector3.Distance(playerTarget.position, transform.position) <= detectionRange;
    }

    void SwitchRoutine(IEnumerator newRoutine)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(newRoutine);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void CheckAttackBoundary()
    {
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < attackBoundary)
        {
            BattleManager.Instance.TakeDamage(13);
        }
    }
}
