using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public float lineupTolerance = .3f;

    private float xDistance = 1f;
    private float yDistance = 1f;

    protected override bool CheckAttackRange()
    {
        return (CheckXLineup() || CheckYLineup()) && Vector2.Distance(playerTransform.position, transform.position) < attackRange;
    }

    private bool CheckXLineup()
    {
        return (xDistance = Mathf.Abs(playerTransform.position.x - transform.position.x)) < lineupTolerance;
    }

    private bool CheckYLineup()
    {
        return (yDistance = Mathf.Abs(playerTransform.position.y - transform.position.y)) < lineupTolerance;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(lineupTolerance, attackRange * 2, 1f));
        Gizmos.DrawCube(transform.position, new Vector3(attackRange * 2, lineupTolerance, 1f));
    }
}
