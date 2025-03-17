using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IHittable
{
    public float moveSpeed = 10f;
    public int health = 2;
    public float postAttackIdleTime = 2f;
    public float chaseRange = 4f;
    public float AttackRange = 1f;
    public List<Transform> patrolPoints;

    protected Transform playerTransform;
    protected int currentPatrolIndex = 0;
    protected Vector3 currentDestination;

    public void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void Attack()
    {

    }

    protected bool CheckChaseRange()
    {
        return false;
    }

    protected bool CheckAttackRange()
    {
        return false;
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
