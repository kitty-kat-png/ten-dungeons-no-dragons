using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void Awake()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = patrolSpeed;
        currentDestination = patrolPoints[0].position;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        switch(currentBehaviour)
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

    protected void UpdatePatrol()
    {
        navMeshAgent.speed = patrolSpeed;

        if (Vector2.Distance(transform.position, currentDestination) < .01f)
        {
            // destination reached

            currentPatrolIndex = (++currentPatrolIndex % patrolPoints.Count);
            currentDestination = patrolPoints[currentPatrolIndex].position;
        }

        if(CheckChaseRange())
        {
            currentBehaviour = EnemyBehaviour.Chase;
            return;
        }

        navMeshAgent.SetDestination(currentDestination);
    }

    protected void UpdateChase()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);

        if(CheckAttackRange())
        {
            currentBehaviour = EnemyBehaviour.Attack;
            return;
        }
    }

    protected void UpdateAttack()
    {
        //do attack

        //wait for attack animation

        currentBehaviour = EnemyBehaviour.Idle;
    }

    protected void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > postAttackIdleTime)
        {
            currentBehaviour = CheckAttackRange() ? EnemyBehaviour.Attack : EnemyBehaviour.Chase;
            idleTimer = 0;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void Attack()
    {
        Debug.Log(gameObject.name + "Attacking");
    }

    protected bool CheckChaseRange()
    {
        return Vector2.Distance(transform.position, playerTransform.position) < chaseRange;        
    }

    protected bool CheckAttackRange()
    {
        return Vector2.Distance(transform.position, playerTransform.position) < attackRange;
    }

    public void Hit(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }
}
