using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyBehaviour
{
    Patrol,
    Chase,
    Attack,
    Idle
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour, IHittable
{
    public float patrolSpeed = 5f;
    public float chaseSpeed = 10f;
    public int health = 2;
    public float postAttackIdleTime = 2f;
    public float chaseRange = 4f;
    public float attackRange = 1f;
    public List<Transform> patrolPoints;

    [SerializeField]
    protected NavMeshAgent navMeshAgent;
    [SerializeField]
    protected EnemyBehaviour currentBehaviour = EnemyBehaviour.Patrol;

    protected Transform playerTransform;
    protected int currentPatrolIndex = 0;
    protected Vector3 currentDestination;
    protected float idleTimer = 0;
    protected Vector2 directionVector = Vector2.up;
    protected bool dead = false;


    public UnityEvent OnDie;
    public UnityEvent OnHit;

    protected virtual void Awake()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = patrolSpeed;
        if (patrolPoints.Count > 0)
            currentDestination = patrolPoints[0].position;
        else
            currentDestination = transform.position;
    }

    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        UpdateDirectionVector();

        switch (currentBehaviour)
        {
            case EnemyBehaviour.Patrol:
                UpdatePatrol();
                break;
            case EnemyBehaviour.Chase:
                UpdateChase();
                break;
            case EnemyBehaviour.Attack:
                UpdateAttack();
                break;
            case EnemyBehaviour.Idle:
                UpdateIdle();
                break;
            default:
                Debug.LogError("EnemyBehaviour invalid");
                break;
        }
    }

    protected virtual void UpdateDirectionVector()
    {
        directionVector = (playerTransform.position - transform.position).normalized;
    }

    protected virtual void UpdatePatrol()
    {
        navMeshAgent.speed = patrolSpeed;

        if (Vector2.Distance(transform.position, currentDestination) < .01f)
        {
            // destination reached
            if (patrolPoints.Count > 0)
            {
                currentPatrolIndex = (++currentPatrolIndex % patrolPoints.Count);
                currentDestination = patrolPoints[currentPatrolIndex].position;
            }
        }

        if(CheckChaseRange())
        {
            currentBehaviour = EnemyBehaviour.Chase;
            return;
        }

        navMeshAgent.SetDestination(currentDestination);
    }

    protected virtual void UpdateChase()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);

        if(CheckAttackRange())
        {
            currentBehaviour = EnemyBehaviour.Attack;
            return;
        }
    }

    protected virtual void UpdateAttack()
    {
        //do attack
        Attack();

        //wait for attack animation

        currentBehaviour = EnemyBehaviour.Idle;
    }

    protected virtual void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > postAttackIdleTime)
        {
            currentBehaviour = CheckAttackRange() ? EnemyBehaviour.Attack : EnemyBehaviour.Chase;
            idleTimer = 0;
        }
    }

    public virtual void Die()
    {
        dead = true;
        OnDie.Invoke();
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        // do stuff over time before destroying
        // gonna do this for now
        transform.Find("Audio").SetParent(transform.parent);
        Destroy(gameObject);
        yield return null;
    }

    protected virtual void Attack()
    {
        Debug.Log(gameObject.name + "Attacking");
    }

    protected virtual bool CheckChaseRange()
    {
        return Vector2.Distance(transform.position, playerTransform.position) < chaseRange;        
    }

    protected virtual bool CheckAttackRange()
    {
        return Vector2.Distance(transform.position, playerTransform.position) < attackRange;
    }

    public virtual void Hit(int damage)
    {
        health -= damage;
        OnHit.Invoke();
        if(health <= 0 && !dead)
        {
            OnDie.Invoke();
            Die();
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if(currentDestination != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentDestination);
        }

        Gizmos.color = Color.blue;
        for (int i=0; i<patrolPoints.Count; i++)
        {
            Transform p1 = patrolPoints[i], p2 = patrolPoints[(i+1) % patrolPoints.Count];
            if(p1 != null && p2 != null)
            {
                Gizmos.DrawLine(p1.position, p2.position);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
