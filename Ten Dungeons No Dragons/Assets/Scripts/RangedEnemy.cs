using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform arrowEmitter;

    protected override void Attack()
    {
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
