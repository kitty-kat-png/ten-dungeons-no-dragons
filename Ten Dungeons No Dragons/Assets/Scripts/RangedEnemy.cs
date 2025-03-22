using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangedEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform arrowEmitter;

    public UnityEvent OnRanged;

    protected override void Attack()
    {
        OnRanged.Invoke();
        GameObject projectile = Instantiate(projectilePrefab, arrowEmitter.position, arrowEmitter.rotation);
    }

    protected override void UpdateDirectionVector()
    {
        base.UpdateDirectionVector();

        arrowEmitter.up = directionVector;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(directionVector * attackRange));
    }
}
